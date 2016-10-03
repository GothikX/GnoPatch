using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GnoPatch
{
    static class Utils
    {

        //internal static bool Match(SerializableInstructionDef def, Instruction i)
        //{
        //    if (i.OpCode.Code != def.OpCode) return false;

        //    if (i.Operand == null)
        //    {
        //        return string.IsNullOrEmpty(def.OperandType);
        //    }

        //    if (i.Operand.GetType().Name != def.OperandType) return false;

        //    throw new NotImplementedException();
            
        //    // if (OperandTargetType != null && )

        //    return true;
        //}
        
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

        

        internal static MethodInfo GetMethod(Type type, Method kind, string name, IEnumerable<Type> genericArguments,
            IEnumerable<Type> argumentTypes)
        {
            var candidates =
                type.GetMethods(kind == Method.Static
                    ? BindingFlags.Static | BindingFlags.Public
                    : BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name == name);

            return candidates.FirstOrDefault(m =>
            {
                var generic = m.IsGenericMethodDefinition ? m.MakeGenericMethod(genericArguments.ToArray()) : m;

                return generic.GetParameters().Select(p => p.ParameterType).SequenceEqual(argumentTypes);
            });
        }

        internal static MethodDefinition GetMethod(TypeDefinition type, Method kind, string name,
            IEnumerable<Type> genericArguments, IEnumerable<Type> argumentTypes)
        {
            var candidates = type.Methods.Where(m => kind == Method.Static ? m.IsStatic : !m.IsStatic && m.Name == name);

            return
                candidates.FirstOrDefault(
                    m =>
                        m.GenericParameters.Select(p => p.FullName)
                            .SequenceEqual(genericArguments.Select(t => t.FullName)) &&
                        m.Parameters.Select(p => p.ParameterType.FullName)
                            .SequenceEqual(argumentTypes.Select(t => t.FullName)));
        }

    }

    internal enum Method
    {
        Instance,
        Static
    }
}