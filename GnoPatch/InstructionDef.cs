using System;
using System.ComponentModel;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json;

namespace GnoPatch
{

    public class InstructionDef
    {
        public InstructionDef()
        {
        }

        public InstructionDef(Instruction actualInstruction)
        {
            ActualInstruction = actualInstruction;
        }

        public InstructionDef(Func<MethodDefinition, Instruction> instructionFactory)
        {
            InstructionFactory = instructionFactory;
        }

        public virtual Instruction GetInstruction(MethodDefinition md)
        {
            if (ActualInstruction != null) return ActualInstruction;
            if (InstructionFactory != null) return InstructionFactory(md);

            throw new Exception("Invalid patch definition.");
        }

        /// <summary>
        /// Offset to an instruction in the same method code.
        /// If this is specified, the instruction's operand is set to the n-th instruction in the list.
        /// </summary>
        public int? SelfOffset { get; set; }

        /// <summary>
        /// For patches created purely in code, we can populate this 
        /// and it will be used instead of anything else in this instance.
        /// </summary>
        [JsonIgnore]
        public Instruction ActualInstruction { get; set; }

        /// <summary>
        /// For code instructions that need a reference to the parent method, we can use this
        /// </summary>
        [JsonIgnore]
        public Func<MethodDefinition, Instruction> InstructionFactory { get; set; }
    }

    public class SerializableInstructionDef : InstructionDef
    {
        public SerializableInstructionDef()
        {
        }

        public SerializableInstructionDef(Code opCode)
        {
            OpCode = opCode;
        }

        [DefaultValue(-1)]
        public Code OpCode { get; set; }

        public string OperandType { get; set; }
        public string OperandTargetType { get; set; }
        public string OperandFullName { get; set; }
        public bool HasConstant { get; set; }
        public string OperandConstant { get; set; }

        // this is all still highly experimental...
        // in the case of switches, we match this against the array; the offset we will replace will be the offset of the matched child
        // in the case of all others instructions with an instruction operand, we match this against the operand; we won't change the offset in this case
        public InstructionDef ChildInstruction { get; set; }

        public override Instruction GetInstruction(MethodDefinition md)
        {
            // this is actually much more trouble than it's worth
            throw new NotImplementedException();
        }
    }
}