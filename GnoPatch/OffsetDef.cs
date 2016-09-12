namespace GnoPatch
{
    public class OffsetDef
    {
        public OffsetDef(int offset, int count)
        {
            Offset = offset;
            Count = count;
        }

        public int Offset { get; set; }
        public int Count { get; set; }

        public static OffsetDef Invalid = new OffsetDef(-1, -1);
    }
}