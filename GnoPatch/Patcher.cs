using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GnoPatch
{
    
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

        public void Apply(PatchGroup patches, string targetDirectory)
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(targetDirectory);

            var target = Path.Combine(targetDirectory, patches.Target);
            if (!File.Exists(target))
            {
                throw new Exception("Can't find the specified file " + patches.Target);
            }

            var assembly = AssemblyDefinition.ReadAssembly(target, new ReaderParameters() { AssemblyResolver = resolver });

            var results = new List<Tuple<PatchOperation, PatchResult>>();

            foreach (var patch in patches.Patches)
            {
                // todo: validate versions

                foreach (var operation in patch.Operations)
                {
                    var result = Apply(operation, assembly);
                    results.Add(Tuple.Create(operation, result));
                }
            }

            var outFile = Path.GetFileNameWithoutExtension(target) + " patched" + Path.GetExtension(target);

            assembly.Write(Path.Combine(Path.GetDirectoryName(target), outFile));

        }

        private static PatchResult Apply(PatchOperation operation, AssemblyDefinition target)
        {
            var module = target.MainModule;

            var type = module.Types.FirstOrDefault(t => t.FullName == operation.TypeName);
            if (type == null) return PatchResult.Fail("Could not find the target type to patch.");

            var method = type.Methods.FirstOrDefault(m => m.Name == operation.Method);
            if (method == null) return PatchResult.Fail("Could not find the target method to patch.");

            var il = method.Body.GetILProcessor();

            if (operation.Offset != null)
            {
                if (DisallowUnsafeActions && operation.Matches.Any())
                {
                    return
                        PatchResult.Fail(
                            "Both offset and matches were specified. Since using an offset is potentially dangerous, this patch was skipped.");
                }
                return ApplyOffset(operation.Offset, operation.Replacements, il);
            }
            else
            {
                var offset = FindMatches(operation, il);

                if (offset.Offset < 0) return PatchResult.Fail("Could not find the matching instructions to patch.");

                return ApplyOffset(offset, operation.Replacements, il);
            }

            return PatchResult.Done();
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

        private static PatchResult ApplyOffset(OffsetDef offset, IEnumerable<InstructionDef> replacements, ILProcessor il)
        {
            var instructions = il.Body.Instructions;
            var start = instructions.FirstOrDefault(i => i.Offset == offset.Offset);
            if (start == null) return PatchResult.Fail("Couldn't find the specified offset.");

            var indexOf = instructions.IndexOf(start);
            if (indexOf + offset.Count > instructions.Count)
                return PatchResult.Fail("Specified offset would be out of bounds of the current method.");

            var toRemove = instructions.Skip(indexOf).Take(offset.Count).ToList();
            
            toRemove.ForEach(il.Remove);

            var replace = replacements as InstructionDef[] ?? replacements.ToArray();
            for (var j = 0; j < replace.Length; j++)
            {
                il.Body.Instructions.Insert(indexOf + j, GetInstruction(il, replace[j]));
            }

            return PatchResult.Done();
        }

        private static Instruction GetInstruction(ILProcessor il, InstructionDef i)
        {
            if (i.ActualInstruction != null) return i.ActualInstruction;

            if (i.OpCode == Code.Nop) return il.Create(OpCodes.Nop);

            throw new NotImplementedException("The specified instruction can't be created yet.");
        }

    }
}