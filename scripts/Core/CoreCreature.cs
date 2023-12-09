using UnityEngine;

namespace GameCore
{
    public abstract class CoreCreatureProperties<T> where T : CoreCreatureProperties<T>, new()
    {
        public static T instance = new();
    }
}
