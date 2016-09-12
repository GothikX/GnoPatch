using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GnoPatch
{
    /// <summary>
    /// Performs the actual patching process, based on patch definitions
    /// in a PatchGroup.
    /// </summary>
    public class Patcher
    {

#if DEBUG
        internal static bool DisallowUnsafeActions = false;
#else
        internal static bool DisallowUnsafeActions = true;
#endif

        public Patcher()
        {
        }

        public PatchProcessResult Apply(PatchGroup patches, IEnumerable<string> targetDirectories)
        {
            var resolver = new DefaultAssemblyResolver();
            var searchLocations = targetDirectories.ToList();

            var target = searchLocations.FirstOrDefault(d => File.Exists(Path.Combine(d, patches.Target)));

            if (string.IsNullOrEmpty(target))
            {
                throw new Exception($"Can't find the specified file {patches.Target}; run this from the target folder.");
            }

            resolver.AddSearchDirectory(Path.GetDirectoryName(target));

            var assembly = AssemblyDefinition.ReadAssembly(target, new ReaderParameters() { AssemblyResolver = resolver });

            var results = new List<PatchResult>();

            foreach (var patch in patches.Patches)
            {
                // todo: validate versions

                foreach (var operation in patch.Operations)
                {
                    var result = Apply(operation, assembly);
                    results.Add(result);
                }
            }

            var outFile = Path.GetFileNameWithoutExtension(target) + " patched" + Path.GetExtension(target);

            assembly.Write(Path.Combine(Path.GetDirectoryName(target), outFile));

            return new PatchProcessResult()
            {
                FinalAssembly = outFile,
                Success = results.All(r => r.Success),
                Details = results
            };
        }

        private static PatchResult Apply(PatchOperation operation, AssemblyDefinition target)
        {
            var module = target.MainModule;

            var type = module.Types.FirstOrDefault(t => t.FullName == operation.TypeName);
            if (type == null) return PatchResult.Fail(operation, "Could not find the target type to patch.");

            var method = type.Methods.FirstOrDefault(m => m.Name == operation.Method);
            if (method == null) return PatchResult.Fail(operation, "Could not find the target method to patch.");

            var il = method.Body.GetILProcessor();

            if (operation.Offset != null)
            {
                if (DisallowUnsafeActions && operation.Matches.Any())
                {
                    return
                        PatchResult.Fail(operation,
                            "Both offset and matches were specified. Since using an offset is potentially dangerous, this patch was skipped.");
                }
                return ApplyOffset(operation, il);
            }
            else
            {
                var offset = FindMatches(operation, il);

                if (offset.Offset < 0) return PatchResult.Fail(operation, "Could not find the matching instructions to patch.");

                operation.Offset = offset;

                return ApplyOffset(operation, il);
            }
        }

        private static OffsetDef FindMatches(PatchOperation operation, ILProcessor il)
        {
            var list = il.Body.Instructions;

            var matches = operation.Matches.ToList();

            var matchIndex = 0;
            var matchOffset = -1;

            foreach (var instruction in list)
            {
                if (Utils.Match(matches[matchIndex], instruction))
                {
                    if (matchIndex == 0) matchOffset = instruction.Offset;

                    matchIndex++;
                    if (matchIndex == matches.Count - 1)
                    {
                        return new OffsetDef(matchOffset, matches.Count);
                    }
                }
                else
                {
                    matchIndex = 0;
                    matchOffset = -1;
                }
            }

            return OffsetDef.Invalid;
        }

        private static PatchResult ApplyOffset(PatchOperation operation, ILProcessor il)
        {
            var instructions = il.Body.Instructions;
            var start = instructions.FirstOrDefault(i => i.Offset == operation.Offset.Offset);
            if (start == null) return PatchResult.Fail(operation, "Couldn't find the specified offset.");

            var indexOf = instructions.IndexOf(start);
            if (indexOf + operation.Offset.Count > instructions.Count)
                return PatchResult.Fail(operation, "Specified offset would be out of bounds of the current method.");

            var toRemove = instructions.Skip(indexOf).Take(operation.Offset.Count).ToList();
            
            toRemove.ForEach(il.Remove);

            var replace = operation.Replacements as InstructionDef[] ?? operation.Replacements.ToArray();
            for (var j = 0; j < replace.Length; j++)
            {
                il.Body.Instructions.Insert(indexOf + j, GetInstruction(il, replace[j]));
            }

            return PatchResult.Done(operation);
        }

        private static Instruction GetInstruction(ILProcessor il, InstructionDef i)
        {
            if (i.ActualInstruction != null) return i.ActualInstruction;

            if (i.OpCode == Code.Nop) return il.Create(OpCodes.Nop);

            throw new NotImplementedException("The specified instruction can't be created yet.");
        }

    }
}