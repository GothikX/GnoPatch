using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GnoPatch
{
    static class Utils
    {

        internal static bool Match(InstructionDef def, Instruction i)
        {
            if (i.Operand.GetType().Name != def.OperandType) return false;
            // if (OperandTargetType != null && )

            return true;
        }

        internal static Type GetOperandType(this Instruction i)
        {
            switch (i.OpCode.OperandType)
            {
                case OperandType.InlineBrTarget:
                    return typeof (Instruction);
                case OperandType.InlineField:
                    return typeof (FieldReference);
                case OperandType.InlineI:
                    return typeof (int);
                case OperandType.InlineI8:
                    return typeof (long);
                case OperandType.InlineMethod:
                    return typeof (MethodReference);
                case OperandType.InlineNone:
                    return null;
                case OperandType.InlinePhi:
                    // dunno what this is yet so we won't support it, probably won't need it anyway
                    throw new NotSupportedException();
                case OperandType.InlineR:
                    return typeof (double); // not sure about this yet
                case OperandType.InlineSig:
                    return typeof (CallSite); // hope we won't need this
                case OperandType.InlineString:
                    return typeof (string);
                case OperandType.InlineSwitch:
                    return typeof (Instruction[]); // dear god
                case OperandType.InlineTok:
                    return typeof (IMetadataTokenProvider);
                case OperandType.InlineType:
                    return typeof (TypeReference);
                case OperandType.InlineVar:
                    return typeof (VariableDefinition);
                case OperandType.InlineArg:
                    return typeof (ParameterDefinition);
                case OperandType.ShortInlineBrTarget:
                    return typeof (Instruction);
                case OperandType.ShortInlineI:
                    return typeof (sbyte); // caution is advised
                case OperandType.ShortInlineR:
                    return typeof (float);
                case OperandType.ShortInlineVar:
                    return typeof (VariableDefinition);
                case OperandType.ShortInlineArg:
                    return typeof (ParameterDefinition);
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), "Unknown operand type.");
            }
        }
    }
}