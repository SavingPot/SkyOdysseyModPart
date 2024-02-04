using System;
using System.Collections;
using SP.Tools.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore
{
    public interface IEnemyMovement
    {
        bool isPursuingLastFrame { get; set; }
        bool isPursuing { get; set; }
    }
}