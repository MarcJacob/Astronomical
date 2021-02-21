using UnityEngine;

namespace Astronomical
{
    public class Universe
    {
        public const decimal MAX_DISTANCE_FROM_ANCHOR = 200;

        static private Universe Instance;
        static public void Build(UniversePositionAnchor rootAnchor)
        {
            if (Instance != null) throw new System.Exception("Error : Attempted to created another Universe without resetting the app.");

            Universe universe = new Universe(rootAnchor);
            Instance = universe;
        }

        static public UniversePositionAnchor GetBestAnchorForPosition(decimal distance, Vector3 direction)
        {
            return GetBestAnchorForPosition(distance, direction, RootPositionAnchor);
        }
        static public UniversePositionAnchor GetBestAnchorForPosition(decimal distance, Vector3 direction, UniversePositionAnchor originAnchor)
        {
            return Instance.FindBestAnchorForPosition(distance, direction, originAnchor);
        }

        static public UniversePositionAnchor RootPositionAnchor
        {
            get { return Instance.m_rootAnchor; }
        }

        private UniversePositionAnchor m_rootAnchor;
        private Universe(UniversePositionAnchor root)
        {
            m_rootAnchor = root;
        }

        private UniversePositionAnchor FindBestAnchorForPosition(decimal distance, Vector3 direction, UniversePositionAnchor origin)
        {
            UniversePositionAnchor result = null;
            Vector3 position = direction * (float)distance;
            float distanceToOriginSquared = (float)(distance * distance);
            // Look through child anchors
            if (origin.childAnchorsList.Count > 0)
            {
                float shortestDistanceSquared = distanceToOriginSquared;
                UniversePositionAnchor closestChild = null;
                foreach(var child in origin.childAnchorsList)
                {
                    Vector3 childPos = (float)child.distanceFromParent * child.directionFromParent;
                    float distSquared = (position - childPos).sqrMagnitude;
                    if (shortestDistanceSquared > distSquared)
                    {
                        closestChild = child;
                        shortestDistanceSquared = distSquared;
                    }
                }

                if (closestChild != null && shortestDistanceSquared < (float)(MAX_DISTANCE_FROM_ANCHOR * MAX_DISTANCE_FROM_ANCHOR))
                {
                    result = closestChild;
                }
            }
            if (result != null) return result;

            // Look through siblings
            if (origin.parentAnchor != null)
            {
                float shortestDistanceSquared = distanceToOriginSquared;
                UniversePositionAnchor closestSibling = origin;
                foreach (var sibling in origin.parentAnchor.childAnchorsList)
                {
                    if (sibling != origin)
                    {
                        Vector3 siblingPos = (float)sibling.distanceFromParent * sibling.directionFromParent;
                        float distSquared = (position - siblingPos).sqrMagnitude;
                        if (shortestDistanceSquared > distSquared)
                        {
                            closestSibling = sibling;
                            shortestDistanceSquared = distSquared;
                        }
                    }
                }

                if (shortestDistanceSquared < (float)(MAX_DISTANCE_FROM_ANCHOR * MAX_DISTANCE_FROM_ANCHOR))
                {
                    result = closestSibling;
                }
            }
            if (result != null) return result;
            else if (origin.parentAnchor != null) return origin.parentAnchor;
            else return origin;
        }
    }
}
