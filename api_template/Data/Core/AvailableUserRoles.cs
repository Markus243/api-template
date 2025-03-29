namespace api_template.Data.Core
{
    public static class AvailableUserRoles
    {
        public static string Admin => nameof(Admin);

        public static string User => nameof(User);

        public const string AllowedTypes = "Admin|User";

        public static IEnumerable<string> GetAll()
        {
            return [Admin, User];
        }

    }
}
