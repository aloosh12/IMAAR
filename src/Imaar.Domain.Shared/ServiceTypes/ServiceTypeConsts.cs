namespace Imaar.ServiceTypes
{
    public static class ServiceTypeConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ServiceType." : string.Empty);
        }

    }
}