namespace LifeSync.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }
}