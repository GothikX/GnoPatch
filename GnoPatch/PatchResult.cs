namespace GnoPatch
{
    public class PatchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static PatchResult Fail(string message)
        {
            return new PatchResult() {Success = false, Message = message};
        }

        public static PatchResult Done(string message = "")
        {
            return new PatchResult() {Success = true, Message = message};
        }
    }
}