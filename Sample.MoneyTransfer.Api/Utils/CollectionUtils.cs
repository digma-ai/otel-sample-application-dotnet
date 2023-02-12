namespace Sample.MoneyTransfer.API.Utils;

public static class CollectionUtils
{
    public static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
    {
        foreach (var item in list)
        {
            action(item);
        }
    }
}