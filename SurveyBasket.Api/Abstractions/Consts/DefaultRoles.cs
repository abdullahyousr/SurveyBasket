namespace SurveyBasket.Api.Abstractions.Consts;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37";
        public const string ConcurrencyStamp = "79A95E4C-425E-41ED-9C5F-9255CCE70200";
    }

    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "DFC22A93-7F04-4F75-AF14-1C8CA3F6AF21";
        public const string ConcurrencyStamp = "7A709DFB-1294-4261-9A45-1D62441F9547";
    }
}
