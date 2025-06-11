namespace Imaar.VacancyEvaluations
{
    public static class VacancyEvaluationConsts
    {
        private const string DefaultSorting = "{0}Rate asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "VacancyEvaluation." : string.Empty);
        }

    }
}