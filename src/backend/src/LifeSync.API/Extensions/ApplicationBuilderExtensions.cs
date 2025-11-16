namespace LifeSync.API.Extensions;

public static class ApplicationBuilderExtensions
{
    extension(WebApplication app)
    {
        public WebApplication LifeSync => app;

        public WebApplication UseGlobalErrorHandling()
        {
            app.UseExceptionHandler();

            return app;
        }
    }
}
