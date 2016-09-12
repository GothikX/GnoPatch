namespace GnoPatch
{
    public class PatchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public PatchOperation Source { get; set; }

        public static PatchResult Fail(PatchOperation source, string message)
        {
            return new PatchResult() {Success = false, Message = message, Source = source};
        }

        public static PatchResult Done(PatchOperation source, string message = "")
        {
            return new PatchResult() {Success = true, Message = message, Source = source};
        }
    }
}