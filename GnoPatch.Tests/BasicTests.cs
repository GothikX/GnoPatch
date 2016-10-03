using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil.Cil;

namespace GnoPatch.Tests
{
    [TestClass]
    public class BasicTests : BaseFixture
    {
        
        const string type = "GnoPatch.Tests.Target.Actor1";

        [TestMethod]
        public void ReplaceReturnValue()
        {
            var patch = Targets.Get(type, "ActInternal", 0, int.MaxValue, new[] {new VariableDef(typeof(string)),},
                new[]
                {
                    new InstructionDef(Instruction.Create(OpCodes.Ldstr, "diggity")),
                    new InstructionDef(Instruction.Create(OpCodes.Ret)),
                });

            var result = Run(patch, type);

            Assert.AreEqual(result, "diggity");
        }

        [TestMethod]
        public void MatchOneInstruction()
        {
            var patch = Targets.Get(type, "Ref",
                new[]
                {
                    new LiteralMatchDef("ldc.i4|1234"), 
                },
                null, // if this is null, variables won't be tampered with
                //new[] { new VariableDef(typeof(int)), new VariableDef(typeof(string)),},
                new[]
                {
                    new InstructionDef(Instruction.Create(OpCodes.Ldc_I4, 5678)),
                }
            );

            var result = ApplyPatch(patch);

            var verify = InvokeModifiedMethod(result, "Actor1", "Ref", new object[0]);

            Assert.AreEqual(verify, "5678");
        }

        [TestMethod]
        public void MatchNotFound_DoesNotApplyPatch()
        {
            var patch = Targets.Get(type, "Ref",
                new[]
                {
                    new LiteralMatchDef("ldc.i4|9876"),
                },
                null, // if this is null, variables won't be tampered with
                      //new[] { new VariableDef(typeof(int)), new VariableDef(typeof(string)),},
                new[]
                {
                    new InstructionDef(Instruction.Create(OpCodes.Ldc_I4, 5678)),
                }
            );

            var result = ApplyPatch(patch);

            Assert.IsFalse(result.Details.Single().Success);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public void MatchMultipleInstructions()
        {
            var patch = Targets.Get(type, "Ref",
                new[]
                {
                    new LiteralMatchDef("nop"),
                    new LiteralMatchDef("ldloca.s|[0]"),
                    new LiteralMatchDef("call|System.String System.Int32::ToString()"),
                    new LiteralMatchDef("stloc.1"),
                    new LiteralMatchDef("br.s|(12)"),
                    new LiteralMatchDef("ldloc.1"),
                },
                null, 
                new[]
                {
                    new InstructionDef(Instruction.Create(OpCodes.Ldstr, "5555")),
                }
            );

            var result = ApplyPatch(patch);

            var verify = InvokeModifiedMethod(result, "Actor1", "Ref", new object[0]);

            Assert.AreEqual(verify, "5555");
        }
    }
}
