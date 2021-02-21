using System;
using UnityEngine;

namespace Astronomical
{
    /// <summary>
    /// Object with the ability to generate a zone, sometimes with the help of other Zone Generators.
    /// </summary>
    [CreateAssetMenu(fileName = "New Zone Generator", menuName = "Astronomical/Zone Generator/Zone Generator", order = 0)]
    public abstract class ZoneGenerator : ScriptableObject
    {
        abstract internal UniversePositionAnchor GenerateZone(UniversePositionAnchor parent = null);
    }
}
