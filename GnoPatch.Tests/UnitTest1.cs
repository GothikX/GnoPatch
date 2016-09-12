using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil.Cil;

namespace GnoPatch.Tests
{

    public static class Harness
    {
        
    }

    public static class Targets
    {
        public const string Assembly = "GnoPatch.Tests.Target.exe";
        public const string DefaultType = "GnoPatch.Tests.Target.Simple";

        private static PatchGroup GetGroup()
        {
            return new PatchGroup()
            {
                Target = Assembly,
                Patches = new[]
                {
                    new Patch()
                }
            };
        }

        public static PatchGroup Get(string method, int offset, int count, IEnumerable<InstructionDef> replacements, string type = DefaultType)
        {
            var group = GetGroup();

            group.Patches.First().Operations = new[]
            {
                new PatchOperation()
                {
                    Method = method,
                    Offset = new OffsetDef(offset, count),
                    Replacements = replacements
                },
            };

            return group;
        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            // todo: refactor the ceremony into a harness to develop the tests faster

            var patches = Targets.Get("Write", 0, 3, new[]
            {
                new InstructionDef(Code.Nop), 
            });

            var patcher = new Patcher();

            var result = patcher.Apply(patches, new[] { Environment.CurrentDirectory });

            // verify the patched copy performs differently; probably use standard output 

            var process = Process.Start(new ProcessStartInfo(result.FinalAssembly)
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true,
            });
        }
    }
}
