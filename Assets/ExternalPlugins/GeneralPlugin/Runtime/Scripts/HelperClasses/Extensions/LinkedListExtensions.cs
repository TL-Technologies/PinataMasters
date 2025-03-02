using System.Collections.Generic;


public static class LinkedListExtensions
{
    public static bool IsNullOrEmpty<T>(this LinkedList<T> list)
    {
        return list == null || list.Count == 0;
    }
    

    public static void AddRangeAsLast<T>(this LinkedList<T> list, IEnumerable<T> range)
    {
        foreach (T item in range)
        {
            list.AddLast(item);
        }
    }
}
