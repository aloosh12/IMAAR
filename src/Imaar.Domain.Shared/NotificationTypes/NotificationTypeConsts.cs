namespace Imaar.NotificationTypes
{
    public static class NotificationTypeConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "NotificationType." : string.Empty);
        }

    }
}