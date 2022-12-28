using MediaStream.Interfaces.DbContext;

namespace MediaStream.Helpers
{
    public static class DbExtensionHelper
    {
        public static void ReloadDbIfNeeded(this WebApplication webApplication) =>
            webApplication.Services
                          .GetRequiredService<IDbContextFactory>()
                          .CreateContext();
    }
}
