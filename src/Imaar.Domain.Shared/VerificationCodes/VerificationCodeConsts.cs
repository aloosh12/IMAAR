namespace Imaar.VerificationCodes
{
    public static class VerificationCodeConsts
    {
        private const string DefaultSorting = "{0}PhoneNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "VerificationCode." : string.Empty);
        }

    }
}