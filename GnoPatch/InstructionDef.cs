using System.ComponentModel;
using Mono.Cecil.Cil;
using Newtonsoft.Json;

namespace GnoPatch
{
    public class InstructionDef
    {
        public InstructionDef()
        {
        }

        public InstructionDef(Code opCode)
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


        /// <summary>
        /// For patches created purely in code, we can populate this 
        /// and it will be used instead of anything else in this instance.
        /// </summary>
        [JsonIgnore]
        public Instruction ActualInstruction { get; set; }
    }
}