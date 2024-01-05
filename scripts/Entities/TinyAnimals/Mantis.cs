using UnityEngine;

namespace GameCore
{
    [EntityBinding(EntityID.Mantis)]
    public class Mantis : TinyAnimal
    {
        public override string Texture() => "ori:mantis_0";
    }
}