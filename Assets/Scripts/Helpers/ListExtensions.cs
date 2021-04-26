using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void AddIfNotNull<T>(this List<T> list, T modelToAdd)
    {
        if (modelToAdd == null)
            return;

        list.Add(modelToAdd);
    }
}
