namespace SiteWebJO2.Services
{

    /// <summary>
    /// class to fetch the secure email key
    /// code from 
    /// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-8.0&tabs=visual-studio
    /// </summary>
    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; } = Environment.GetEnvironmentVariable("SendGridKey");
    }
}
