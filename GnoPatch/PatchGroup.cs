using System.Collections.Generic;

namespace GnoPatch
{
    public class PatchGroup
    {
        public string Description { get; set; }

        public string Target { get; set; }

        public IEnumerable<Patch> Patches { get; set; }
    }
}