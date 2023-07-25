using UnityEngine;

namespace GameCore
{
    public abstract class PoultryProperties<T> : LivestockProperties<T> where T : PoultryProperties<T>, new()
    {
        public abstract string EggID();
    }

    public abstract class Poultry<T> : Livestock<T> where T : PoultryProperties<T>, new()
    {
        
    }
}