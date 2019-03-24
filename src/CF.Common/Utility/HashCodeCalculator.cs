namespace CF.Common.Utility
{
    public static class HashCodeCalculator
    {
        public static int Calculate(params object[] values)
        {
            if (values == null)
            {
                return 0;
            }

            int hash = 17;

            foreach (var value in values)
            {
                hash = hash * 23 + value?.GetHashCode() ?? 0;
            }

            return hash;
        }
    }
}
