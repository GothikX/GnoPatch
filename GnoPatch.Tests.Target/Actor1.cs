using GnoPatch.Tests.Interfaces;

namespace GnoPatch.Tests.Target
{
    public class Actor1 : IActor
    {

        private string field = "default";

        public string Property { get; set; }


        public object Act()
        {
            return ActInternal();
        }

        private object ActInternal()
        {
            return 1;
        }

        public string Ref()
        {
            var constant = 1234;
            Property = field;
            return constant.ToString();
        }

        public string Ref2()
        {
            var value = "2";
            return value;
        }
    }
}