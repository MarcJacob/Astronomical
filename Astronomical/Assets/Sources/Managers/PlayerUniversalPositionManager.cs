using System.Collections.Generic;
using UnityEngine;

namespace Astronomical
{
    public class PlayerUniversalPositionManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Distance from the center of the universe in Light Years at start")]
        private float startDistanceFromCenterOfUniverse;
        [SerializeField]
        [Tooltip("Direction from the center of the universe at start")]
        private Vector3 startDirectionFromCenterOfUniverse;

        public UniversePositionAnchor CurrentUniversePositionAnchor { get { return m_dynamicAnchors.Peek(); } }
        public UniversePositionAnchor CurrentUniverseNaturalAnchor { get; private set; }

        private Stack<UniversePositionAnchor> m_dynamicAnchors;

        private void Update()
        {
            UpdatePlayerUniversalPosition();
        }

        internal void UpdatePlayerUniversalPosition()
        {
            var result = Universe.GetBestAnchorForPosition((decimal)startDistanceFromCenterOfUniverse, startDirectionFromCenterOfUniverse);

            Vector3 shipToResult = (result.directionFromParent * (float)result.distanceFromParent - startDistanceFromCenterOfUniverse * startDirectionFromCenterOfUniverse).normalized * 10f;
            Debug.DrawRay(PlayerShipController.PlayerShipPosition, shipToResult);

            // TODO : Try to find if we are close enough to a child anchor of the universe's root to use that as our "natural" anchor instead.

            CurrentUniverseNaturalAnchor = result;

            // Get the "natural scale" we're working with and generate anchors down to the Meter scale.

            UniversePositionAnchor.ScaleUnit scaleUnit = CurrentUniverseNaturalAnchor.scaleUnit;
            m_dynamicAnchors = new Stack<UniversePositionAnchor>();

            // Generate dynamic anchors until we reach the smallest unit of measurement
            while(scaleUnit > 0)
            {
                scaleUnit--;

                UniversePositionAnchor parent;
                decimal distanceFromParent;
                Vector3 directionFromParent;
                if (m_dynamicAnchors.Count == 0)
                {
                    parent = CurrentUniverseNaturalAnchor;
                    distanceFromParent = (decimal)startDistanceFromCenterOfUniverse;
                    directionFromParent = startDirectionFromCenterOfUniverse;
                }
                else
                {
                    parent = m_dynamicAnchors.Peek();
                    distanceFromParent = 0;
                    directionFromParent = Vector3.one;
                }
                UniversePositionAnchor newAnchor = new UniversePositionAnchor(parent, distanceFromParent, directionFromParent, UniversePositionAnchor.ZoneRepresentation.NONE, scaleUnit);
                m_dynamicAnchors.Push(newAnchor);
            }
        }
    }
}
