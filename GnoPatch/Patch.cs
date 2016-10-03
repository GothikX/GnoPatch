using System;
using System.Collections.Generic;

namespace GnoPatch
{
    public class Patch
    {
        public Patch()
        {
            Apply = true;
        }

        public string Name { get; set; }

        public string Info { get; set; }

        public string MinVersion { get; set; }

        public string MaxVersion { get; set; }

        public bool Apply { get; set; }

        public IEnumerable<PatchOperation> Operations { get; set; } 
    }
}