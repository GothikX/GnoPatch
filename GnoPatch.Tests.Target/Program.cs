using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GnoPatch.Tests.Interfaces;

namespace GnoPatch.Tests.Target
{
    /// <summary>
    /// This application is intended to be a target for the patcher tests;
    /// i.e. the tests will patch the output assembly and verify that the patches are successful.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class Actor1 : IActor
    {
        public object Act()
        {
            return ActInternal();
        }

        private object ActInternal()
        {
            return 1;
        }
    }

}
