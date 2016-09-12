using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace GnoPatch
{
    class Program
    {

        private static Patch _patch = new Patch()
        {
            Name = "Fix crash when deconstructing steam engine",
            Info = "Base class already nulls mSFX, this removes the extra call that generates a nullref exception.",
            MinVersion = "1.0.0.0",
            MaxVersion = "1.0.0.0",
            Operations = new[]
            {
                new PatchOperation()
                {
                    TypeName = "Game.SteamEngine",
                    Method = "OnDelete",
                    Offset = new OffsetDef(6, 3),
                    Matches = new[]
                    {
                        new InstructionDef()
                        {
                            OpCode = Code.Ldarg_0,
                            OperandType = "type",
                            OperandTargetType = "something something",
                            OperandFullName = "msfx something",
                            HasConstant = false,
                            OperandConstant = null
                        },
                        new InstructionDef()
                        {
                            OpCode = Code.Ldfld,
                            OperandType = "type",
                            OperandTargetType = "something something",
                            OperandFullName = "msfx something",
                            HasConstant = false,
                            OperandConstant = null
                        }
                    },
                    Replacements = new[]
                    {
                        new InstructionDef()
                        {
                            OpCode = Code.Nop,
                            
                        }
                    }
                }
            }
        };

        static void Main(string[] args)
        {
            //SaveFile();

            // var patches = Patches.Load("patches.json");

            var patches = new PatchGroup()
            {
                Target = "Gnomoria - copy.exe",
                Description = "this should patch the steam engine deconstruction crash",
                Patches = new[]
                {
                    new Patch()
                    {
                        Operations = new[]
                        {
                            new PatchOperation()
                            {
                                TypeName = "Game.SteamEngine",
                                Method = "OnDelete",
                                Offset = new OffsetDef(6, 3),
                                Replacements = new[]
                                {
                                    new InstructionDef(Code.Nop), 
                                }
                            }, 
                        }
                    }, 
                }
                
            };

            
            var path = "c:\\program files (x86)\\steam\\steamapps\\common\\gnomoria";
            
            var patcher = new Patcher();
            
            patcher.Apply(patches, path);
        }

        
        static void SaveFile()
        {
            var tests = new PatchGroup()
            {
                Description = "blah",
                Target = "Gnomoria.exe",
                Patches = new[]
                {
                    _patch
                }
            };

            Patches.Save(tests);
        }
    }
}
