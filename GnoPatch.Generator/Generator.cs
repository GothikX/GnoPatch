using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace GnoPatch.Generator
{
    class Generator
    {
        static void Main(string[] args)
        {
            // this will generate IL instructions using our object model from an actual compiled method

            var path = File.ReadAllText(".config");

            var assembly = AssemblyDefinition.ReadAssembly(path);

            var type = assembly.MainModule.GetAllTypes().First(t => t.FullName == "Game.AudioManager");

            var method = type.Methods.First(m => m.Name == "method_6");

            var result = new StringBuilder();

            result.AppendLine("Variables:");

            method.Body.Variables.ToList().ForEach(v => result.AppendLine("new VariableDef(typeof(" + v.VariableType.FullName + ")),"));

            result.AppendLine("IL:");

            var instructions = method.Body.Instructions.ToList();

            foreach (var instruction in instructions)
            {
                string line;

                if (instruction.Operand == null)
                {
                    result.AppendLine($"new InstructionDef(Instruction.Create(OpCodes.{instruction.OpCode.Code})),");
                    continue;
                }

                var opType = instruction.Operand.GetType();
                if (opType.FullName.StartsWith("System"))
                {
                    var value = opType == typeof(string)
                        ? "\"" + instruction.Operand + "\""
                        : instruction.Operand.ToString();
                    line = $"new InstructionDef(Instruction.Create(OpCodes.{instruction.OpCode.Code}, {value})),";
                }
                else if (opType == typeof(MethodReference))
                {
                    var mr = (MethodReference) instruction.Operand;
                    var mtype = mr.HasThis ? "Method.Instance" : "Method.Static";
                    var gparams = mr.GenericParameters.Count == 0
                        ? "Type.EmptyTypes"
                        : "new[] { " + string.Join(", ", mr.GenericParameters.Select(p => "typeof(" + p.FullName + ")")) + "}";
                    var mparams = mr.Parameters.Count == 0
                        ? "Type.EmptyTypes"
                        : "new[] { " +
                          string.Join(", ", mr.Parameters.Select(p => "typeof(" + p.ParameterType.FullName + ")")) + "}";
                    line =
                        $"new InstructionDef(m => Instruction.Create(OpCodes.{instruction.OpCode.Code}, " +
                        $"m.Module.Import(Utils.GetMethod(typeof({mr.DeclaringType}), {mtype}, {gparams}, {mparams})))), ";
                }
                else if (opType == typeof(Instruction))
                {
                    // until I see a case where this doesn't work I'll keep it simple
                    var op = (Instruction) instruction.Operand;
                    var offset = instructions.IndexOf(op);
                    line =
                        $"new InstructionDef(Instruction.Create(OpCodes.{instruction.OpCode.Code}, Instruction.Create(OpcOdes.Nop))) {{ SelfOffset: {offset}  }},";
                }
                else
                {
                    line = $"new InstructionDef(Instruction.Create(OpCodes.{instruction.OpCode.Code})), // unknown operand: {opType.Name}";
                }

                result.AppendLine(line);
            }

        }
    }
}
