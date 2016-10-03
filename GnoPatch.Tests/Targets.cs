using System.Collections.Generic;
using System.Linq;

namespace GnoPatch.Tests
{
    public static class Targets
    {
        public const string Assembly = "GnoPatch.Tests.Target.exe";
        public const string DefaultType = "GnoPatch.Tests.Target.Actor1";

        private static PatchGroup GetGroup()
        {
            return new PatchGroup()
            {
                Target = Assembly,
                Patches = new[]
                {
                    new Patch()
                }
            };
        }

        public static PatchGroup Get(string type, string method, int offset, int count, IEnumerable<VariableDef> vars,
            IEnumerable<InstructionDef> replacements)
        {
            var group = GetGroup();

            group.Patches.First().Operations = new[]
            {
                new PatchOperation()
                {
                    TypeName = type,
                    Method = method,
                    Variables = vars,
                    Offset = new OffsetDef(offset, count),
                    Replacements = replacements
                },
            };

            return group;
        }

        public static PatchGroup Get(string type, string method, IEnumerable<MatchDef> matches,
            IEnumerable<VariableDef> vars, IEnumerable<InstructionDef> replacements)
        {
            var group = GetGroup();
            group.Patches.First().Operations = new[]
            {
                new PatchOperation()
                {
                    TypeName = type,
                    Method = method,
                    Variables = vars,
                    Matches = matches,
                    Replacements = replacements
                },
            };

            return group;
        }
    }
}