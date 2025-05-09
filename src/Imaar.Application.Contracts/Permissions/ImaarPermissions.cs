namespace Imaar.Permissions;

public static class ImaarPermissions
{
    public const string GroupName = "Imaar";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Categories
    {
        public const string Default = GroupName + ".Categories";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceTypes
    {
        public const string Default = GroupName + ".ServiceTypes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ImaarServices
    {
        public const string Default = GroupName + ".ImaarServices";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
