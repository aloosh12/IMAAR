namespace Imaar.VacancyAdditionalFeatures
{
    public static class VacancyAdditionalFeatureConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "VacancyAdditionalFeature." : string.Empty);
        }

    }
}