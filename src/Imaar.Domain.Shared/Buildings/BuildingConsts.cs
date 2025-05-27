namespace Imaar.Buildings
{
    public static class BuildingConsts
    {
        private const string DefaultSorting = "{0}MainTitle asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Building." : string.Empty);
        }

    }
}