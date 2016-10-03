using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Newtonsoft.Json;

namespace GnoPatch
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var patches = new PatchGroup()
            {
                Target = "Gnomoria.exe",
                Description = "This is the default set of patches.",
                Patches = new[]
                {
                    new Patch()
                    {
                        Name = "Steam engine crash fix",
                        Info = "Removes a few instructions from the OnDelete method of SteamEngine, which cause a nullref exception by attempting to access the mSFX field after it's been nulled by the base class.",
                        Operations = new[]
                        {
                            new PatchOperation()
                            {
                                TypeName = "Game.SteamEngine",
                                Method = "OnDelete",
                                Offset = new OffsetDef(6, 3),
                            },
                        }
                    },
                    new Patch()
                    {
                        Name = "Linux/Mac song name crash fix (hopefully) (work in progress)",
                        Operations = new[]
                        {
                            new PatchOperation()
                            {
                                TypeName = "Game.AudioManager",
                                Method = "c06da358a87ed5391e142d594fd5cfd5c",
                                Variables = new[]
                                {
                                    new VariableDef(typeof(string)),
                                    new VariableDef(typeof(string)),
                                    new VariableDef(typeof(string)),
                                    new VariableDef(typeof(IEnumerator<string>)),
                                    new VariableDef(typeof(string)),
                                    new VariableDef(typeof(string)),
                                },
                                Offset = new OffsetDef(0, 20), // yes, we're replacing the entire method
                                Replacements = new[]
                                {
                                    new InstructionDef(Instruction.Create(OpCodes.Ldstr, "Content/Audio/Music/")),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldarg_0)),
                                    new InstructionDef(m =>
                                            Instruction.Create(OpCodes.Call,
                                                m.DeclaringType.Methods.First(
                                                    t => t.Name == "ca26f4a689fb8af2e6971169f0c7efdba"))
                                        // folder path based on selected music type
                                    ),
                                    new InstructionDef(m =>
                                        Instruction.Create(OpCodes.Call,
                                            m.Module.Import(Utils.GetMethod(typeof(string), Method.Static,
                                                "Concat", Type.EmptyTypes,
                                                new[] {typeof(string), typeof(string)})))),
                                    new InstructionDef(Instruction.Create(OpCodes.Stloc_0)),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldarg_0)),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldarg_1)),
                                    new InstructionDef(
                                        m =>
                                            Instruction.Create(OpCodes.Call,
                                                m.Module.Import(Utils.GetMethod(m.DeclaringType, Method.Instance,
                                                    "c2b2be5a7423f8c79e8429c239980f044",
                                                    Type.EmptyTypes, new[] {typeof(string)})))),
                                    new InstructionDef(Instruction.Create(OpCodes.Stloc_1)),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldloc_0)),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldloc_1)),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldstr, ".ogg")),
                                    new InstructionDef(m =>
                                        Instruction.Create(OpCodes.Call,
                                            m.Module.Import(Utils.GetMethod(typeof(string), Method.Static,
                                                "Concat", Type.EmptyTypes,
                                                new[] {typeof(string), typeof(string)})))),
                                    new InstructionDef(Instruction.Create(OpCodes.Stloc_2)),
                                    new InstructionDef(Instruction.Create(OpCodes.Ldloc_2)),
                                    new InstructionDef(
                                        m =>
                                            Instruction.Create(OpCodes.Call,
                                                m.Module.Import(Utils.GetMethod(typeof(System.IO.File), Method.Static,
                                                    "Exists", Type.EmptyTypes, new[] {typeof(string)})))),
                                    new InstructionDef(Instruction.Create(OpCodes.Brfalse_S, Instruction.Create(OpCodes.Nop))) { SelfOffset = 19 }, // points to another instruction in this list; the nop is just a placeholder
                                    new InstructionDef(Instruction.Create(OpCodes.Ldloc_2)), 
                                    new InstructionDef(Instruction.Create(OpCodes.Ret)), 
                                    new InstructionDef(m => Instruction.Create(OpCodes.Stloc_S, m.Body.Variables[4])), 

                                }
                            },
                        }
                    },
                }

            };
            
            // I'll get rid of this hardcoded path soon
            var path = "c:\\program files (x86)\\steam\\steamapps\\common\\gnomoria";
            
            var patcher = new Patcher();
            
            var result = patcher.Apply(patches, new[] { path, Environment.CurrentDirectory });

            if (result.Success)
            {
                Console.WriteLine($"File '{result.FinalAssembly}' written to disk.");
                Console.WriteLine("Done! Press any key to exit.");
            }
            else
            {
                Console.WriteLine("Patching failed. Details:");
                result.Details.ForEach(d => Console.WriteLine(d.Message));
                Console.WriteLine("Press any key to exit.");
            }

            Console.Read();
        }

        
    }
}
