namespace EventHub.Web.Areas.Identity.IdentityConfigurationSettings
{
    public class Settings
    {
        public class IdentitySettings
        {
            public PasswordSettings Password { get; set; } = new();
            public CookieSettings Cookie { get; set; } = new();
            public LockoutSettings Lockout { get; set; } = new();
        }
        public class CookieSettings
        {
            public bool IsExpiration { get; set; }
            public int ExpireMinutes { get; set; }
        }
       
        public class LockoutSettings
        {
            public int MaxFailedAttempts { get; set; }
            public int LockoutMinutes { get; set; }
        }
        public class PasswordSettings
        {
            public int RequiredLength { get; set; }
            public bool RequireDigit { get; set; }
            public bool RequireUppercase { get; set; }
            public bool RequireLowercase { get; set; }
        }
    }
}
