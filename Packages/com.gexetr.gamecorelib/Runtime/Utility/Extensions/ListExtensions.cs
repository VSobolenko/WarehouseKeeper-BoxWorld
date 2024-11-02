using System;
using System.Collections.Generic;

namespace Game.Extensions
{
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)  
    {  
        var random = new Random();
        var count = list.Count;  
        while (count > 1) {  
            count--;  
            var k = random.Next(count + 1);  
            var value = list[k];  
            list[k] = list[count];  
            list[count] = value;  
        }  
    }
    
    public static void Shuffle<T>(this IList<T> list, Random randomInstance)  
    {  
        var count = list.Count;  
        while (count > 1) {  
            count--;  
            var k = randomInstance.Next(count + 1);  
            var value = list[k];  
            list[k] = list[count];  
            list[count] = value;  
        }  
    }
}
}