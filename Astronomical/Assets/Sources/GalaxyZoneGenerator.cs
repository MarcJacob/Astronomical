using UnityEngine;

namespace Astronomical
{
    /// <summary>
    /// Creates a large scale zone centered on a Black Hole.
    /// </summary>
    [CreateAssetMenu(fileName = "New Galaxy Zone Generator", menuName = "Astronomical/Zone Generator/Galaxy Generator", order = 0)]
    public class GalaxyZoneGenerator : ZoneGenerator
    {
        [SerializeField]
        private int starCount;

        [SerializeField]
        private int m_maxDistanceInLightYears = 200;
        [SerializeField]
        private int m_minDistanceInLightYears = 1;

        internal override UniverseZone GenerateZone(UniverseZone parent)
        {
            UniverseZone galacticZone = new UniverseZone(parent, 0, Vector3.zero, 1, UniverseZone.ZoneRepresentation.BLACKHOLE, UniverseZone.ScaleUnit.LIGHTYEAR); ;

            if (starCount <= 0) return galacticZone;

            UniverseZone[] starSystemZones = new UniverseZone[starCount];

            for(int starSystemID = 0; starSystemID < starCount; starSystemID++)
            {
                Vector3 direction = Vector3.zero;
                do
                {
                    direction = Random.Range(-1f, 1f) * Vector3.right +
                                Random.Range(-1f, 1f) * Vector3.up +
                                Random.Range(-1f, 1f) * Vector3.forward;
                } while (direction.sqrMagnitude > 1f);

                direction.Normalize();
                uint distance = (uint)Random.Range(m_minDistanceInLightYears, m_maxDistanceInLightYears);

                UniverseZone starSystemZone = new UniverseZone(galacticZone, distance, direction, 1, UniverseZone.ZoneRepresentation.STAR, UniverseZone.ScaleUnit.KILOMETERS);
                starSystemZones[starSystemID] = starSystemZone;

                Debug.Log("Generated star system in direction " + direction + " at distance " + starSystemZone.distanceFromParent + " LY.");
            }

            galacticZone.childZonesList.AddRange(starSystemZones);
            return galacticZone;
        }
    }
}
