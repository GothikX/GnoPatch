using System;
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

        static void Main(string[] args)
        {
            //SaveFile();

            // var patches = Patches.Load("patches.json");

            // this is the default patch we'll apply for the steam engine patch.
            var patches = new PatchGroup()
            {
                Target = "Gnomoria.exe",
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
