using System.Collections.Generic;

namespace GnoPatch
{
    public class PatchProcessResult
    {
        /// <summary>
        /// Filename of the assembly that was written to disk.
        /// </summary>
        public string FinalAssembly { get; set; }

        /// <summary>
        /// Set to true if all patches in a PatchGroup have been applied successfully.
        /// </summary>
        public bool Success { get; set; }

        public List<PatchResult> Details { get; set; }
    }
}