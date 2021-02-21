using System.Collections.Generic;
using UnityEngine;

namespace Astronomical
{
    public class UniversePositionAnchor
    {
        public enum ZoneRepresentation
        {
            NONE, BLACKHOLE, STAR, PLANET, ASTEROID
        }

        public enum ScaleUnit
        {
            METERS,
            AU, 
            LIGHTYEAR
        }

        public UniversePositionAnchor parentAnchor; // if null, then this is the Root of the Universe.
        public List<UniversePositionAnchor> childAnchorsList;
        
        public decimal distanceFromParent;
        public Vector3 directionFromParent;

        public ZoneRepresentation representation;
        public ScaleUnit scaleUnit;

        public UniversePositionAnchor(UniversePositionAnchor parent, decimal distanceFromParent, Vector3 directionFromParent, ZoneRepresentation representation, ScaleUnit scaleUnit)
        {
            parentAnchor = parent;
            this.representation = representation;
            this.scaleUnit = scaleUnit;

            this.distanceFromParent = distanceFromParent;
            this.directionFromParent = directionFromParent;

            childAnchorsList = new List<UniversePositionAnchor>();
        }
    }
}
