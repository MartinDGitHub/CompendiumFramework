namespace CF.Common.Authorization.Requirements
{
    public class RequirementResult
    {
        /// <summary>
        /// Gets whether the requirement is met.
        /// </summary>
        public bool IsMet { get; }

        /// <summary>
        /// Gets a message describing why the requirement was not met.
        /// </summary>
        public string UnmetMessage { get; }

        public RequirementResult(bool isMet, string unmetMessage = null)
        {
            this.IsMet = isMet;
            this.UnmetMessage = unmetMessage;
        }
    }
}
