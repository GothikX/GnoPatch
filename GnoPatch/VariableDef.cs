using System;

namespace GnoPatch
{
    public class VariableDef
    {
        public VariableDef(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}