using System.Collections.Generic;


public static class ListExtensions
{
    public static T Dequeue<T>(this List<T> list)
    {
        T temp = list[0];
        list.RemoveAt(0);
        return temp;
    }
    
    
    public static void Enqueue<T>(this List<T> list, T obj)
    {
        list.Add(obj);
    }
    
    
    public static T Peek<T>(this List<T> list)
    {
        return list[0];
    }


    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        return list == null || list.Count == 0;
    }
    
    
    public static T Last<T>(this List<T> list)
    {
        return list.IsNullOrEmpty() ? default(T) : list[list.Count - 1];
    }
    
    
    public static T First<T>(this List<T> list)
    {
        return list.IsNullOrEmpty() ? default(T) : list[0];
    }
}
