namespace Imaar.ServiceEvaluations
{
    public static class ServiceEvaluationConsts
    {
        private const string DefaultSorting = "{0}Rate asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ServiceEvaluation." : string.Empty);
        }

    }
}