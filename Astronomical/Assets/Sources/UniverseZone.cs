using System.Collections.Generic;
using UnityEngine;

namespace Astronomical
{
    public class UniverseZone
    {
        public enum ZoneRepresentation
        {
            BLACKHOLE, STAR, PLANET, ASTEROID
        }

        public enum ScaleUnit
        {
            METERS, KILOMETERS, AU, LIGHTYEAR
        }

        public UniverseZone parentZone; // if null, then this is the Root of the Universe.
        public List<UniverseZone> childZonesList;
        
        public int scale; // Roughly how large a single unit of distance from this to a child zone is in meters.
        
        public uint distanceFromParent;
        public Vector3 directionFromParent;

        public ZoneRepresentation representation;
        public ScaleUnit scaleUnit;

        public UniverseZone(UniverseZone parent, uint distanceFromParent, Vector3 directionFromParent, int scale, ZoneRepresentation representation, ScaleUnit scaleUnit)
        {
            parentZone = parent;
            this.scale = scale;
            this.representation = representation;
            this.scaleUnit = scaleUnit;

            this.distanceFromParent = distanceFromParent;
            this.directionFromParent = directionFromParent;

            childZonesList = new List<UniverseZone>();
        }
    }
}
