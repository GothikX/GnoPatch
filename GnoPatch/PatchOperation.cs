using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GnoPatch
{
    public class PatchOperation
    {
        /// <summary>
        /// Fully qualified name of the type to patch.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Name of the target method; we will need a good reliable way to specify 
        /// overloads, but for now just the simple name works, and we select the first matching method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The type of operation to perform.
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// For now we will replace the target method's variables entirely.
        /// Even if we somehow manage to insert a variable in the middle of the collection with our changes,
        /// chances are this will mess up more than a few other things, I suspect.
        /// </summary>
        public IEnumerable<VariableDef> Variables { get; set; }

        /// <summary>
        /// Unconditional patching; when this is specified, matches won't be matched,
        /// instead we'll go to this offset directly and modify whatever is there.
        /// </summary>
        public OffsetDef Offset { get; set; }

        /// <summary>
        /// If these are specified, we'll search the target method for instructions
        /// that match these exactly, in sequence, and replace the matches.
        /// </summary>
        public IEnumerable<MatchDef> Matches { get; set; }

        /// <summary>
        /// The initial goal was to read these from a file, but this may be impossible;
        /// more complicated instructions may need references to fields or variables that may be complicated to serialize 
        /// in any logical matter. If it does work however, we'll be able to just modify the files 
        /// and never worry about changing the application itself.
        /// </summary>
        public IEnumerable<InstructionDef> Replacements { get; set; }
    }
}