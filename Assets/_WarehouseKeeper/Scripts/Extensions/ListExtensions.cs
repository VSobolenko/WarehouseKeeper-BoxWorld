using System;
using System.Collections.Generic;

namespace WarehouseKeeper.Extension
{
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)  
    {  
        var rng = new Random();
        var count = list.Count;  
        while (count > 1) {  
            count--;  
            var k = rng.Next(count + 1);  
            var value = list[k];  
            list[k] = list[count];  
            list[count] = value;  
        }  
    }
}
}