namespace GnoPatch
{
    public enum OperationType
    {
        /// <summary>
        /// Default; replaces matches or offset with the replacements.
        /// </summary>
        Replace,
        /// <summary>
        /// Appends the replacements after the matches or offset
        /// </summary>
        Append,
        /// <summary>
        /// Prepends the replacements before the matches or offset
        /// </summary>
        Prepend
    }
}