namespace Imaar.BuildingFacades
{
    public static class BuildingFacadeConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "BuildingFacade." : string.Empty);
        }

    }
}