using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var instance = new Simple();

            instance.Write();
        }
    }

    public class Simple
    {
        public void Write()
        {
            Console.WriteLine("default message");
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

}
