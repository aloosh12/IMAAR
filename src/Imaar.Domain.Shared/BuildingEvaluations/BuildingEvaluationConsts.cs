namespace Imaar.BuildingEvaluations
{
    public static class BuildingEvaluationConsts
    {
        private const string DefaultSorting = "{0}Rate asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "BuildingEvaluation." : string.Empty);
        }

    }
}