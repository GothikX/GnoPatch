using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil.Cil;

namespace GnoPatch.Tests
{
    [TestClass]
    public class BasicTests : BaseFixture
    {
        
        [TestMethod]
        public void ReplaceReturnValue()
        {
            var type = "GnoPatch.Tests.Target.Actor1";
            var patch = Targets.Get(type, "ActInternal", 0, int.MaxValue, new[] {new VariableDef(typeof(string)),},
                new[]
                {
                    new InstructionDef(Instruction.Create(OpCodes.Ldstr, "diggity")),
                    new InstructionDef(Instruction.Create(OpCodes.Ret)),
                });

            var result = Run(patch, type);

            Assert.AreEqual(result, "diggity");
        }
        
    }
}
