using Mono.Cecil.Cil;

namespace GnoPatch
{
    public abstract class MatchDef
    {
        public abstract bool Match(Instruction target, ILProcessor body);
    }
}