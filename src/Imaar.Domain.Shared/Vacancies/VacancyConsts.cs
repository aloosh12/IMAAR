namespace Imaar.Vacancies
{
    public static class VacancyConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Vacancy." : string.Empty);
        }

    }
}