using System;
using System.Collections.Generic;

namespace GnoPatch
{
    public class Patch
    {
        public string Name { get; set; }

        public string Info { get; set; }

        public string MinVersion { get; set; }

        public string MaxVersion { get; set; }

        public IEnumerable<PatchOperation> Operations { get; set; } 
    }
}