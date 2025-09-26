namespace LifeSync.Common.Extensions;

public static class ConditionalExtensions
{
    public static void Then(this bool condition, Action run)
    {
        if (condition)
        {
            run();
        }
    }

    public static T? Then<T>(this bool condition, Func<T> run)
        => condition ? run() : default;

    public static async Task ThenAsync(this bool condition, Func<Task> run)
    {
        if (condition)
        {
            await run();
        }
    }

    public static async Task<T?> ThenAsync<T>(this bool condition, Func<Task<T>> run) =>
        condition
            ? await run()
            : await Task.FromResult((T?)default);
}