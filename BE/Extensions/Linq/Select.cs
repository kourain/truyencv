using TruyenCV.Models;

public static partial class Extensions
{
    public static async IAsyncEnumerable<R> ISelectAsync<S, R>(this IEnumerable<S> source, Func<S, Task<R>> selector)
    {
        foreach (var item in source)
        {
            yield return await selector(item);
        }
    }
    public static async Task<IEnumerable<R>> SelectAsync<S, R>(this IEnumerable<S> source, Func<S, Task<R>> selector)
    {
        var results = new List<R>();
        foreach (var item in source)
        {
            results.Add(await selector(item));
        }
        return results;
    }
}