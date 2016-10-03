using Mono.Cecil.Cil;

namespace GnoPatch
{
    public class LiteralMatchDef : MatchDef
    {
        public LiteralMatchDef(string instruction)
        {
            Instruction = instruction;
        }

        /// <summary>
        /// Text representation of an IL instruction; should be equivalent to Instruction.ToMatchString()
        /// </summary>
        public string Instruction { get; set; }

        public override bool Match(Instruction target, ILProcessor body)
        {
            var matchString = MatchString(target, body);
            return matchString == Instruction;
        }

        private string MatchString(Instruction i, ILProcessor body)
        {
            var item = i.Operand as Instruction;
            if (item != null)
            {
                // convention for references to other instructions is similar to what Reflexil uses - index of the instruction in the list, not IL offset
                var index = body.Body.Instructions.IndexOf(item);
                return $"{i.OpCode.Name}|({index})";
            }

            var variable = i.Operand as VariableDefinition;
            if (variable != null)
            {
                // convention for variable references is just [index] for brevity; we'll see if more accurate details are needed
                var index = body.Body.Variables.IndexOf(variable);
                return $"{i.OpCode.Name}|[{index}]";
            }

            // for most other things Operand.ToString will do just fine

            if (i.Operand == null) return i.OpCode.Name;

            return $"{i.OpCode.Name}|{i.Operand}";
        }
    }
}