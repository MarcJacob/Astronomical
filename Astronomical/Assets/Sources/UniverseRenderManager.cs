using UnityEngine;

namespace Astronomical
{
    public class UniverseRenderManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_starPrefab;
        [SerializeField]
        private GameObject m_blackholePrefab;

        [SerializeField]
        private float starLuminosity = 100;

        [SerializeField]
        private int zoneID;

        private UniverseZone m_rootZone;

        public void StartRender(UniverseZone rootZone)
        {
            m_rootZone = rootZone;
        }

        private void Update()
        {
            if (m_rootZone != null)
            {
                UniverseZone observerZone = null;
                if (zoneID < 0 || zoneID >= m_rootZone.childZonesList.Count) observerZone = m_rootZone;
                else
                {
                    observerZone = m_rootZone.childZonesList[zoneID];
                }

                RenderCurrentUniverseView(observerZone);
            }
        }

        private void RenderCurrentUniverseView(UniverseZone observerZone)
        {
            // Render ourselves

            Matrix4x4 renderingMatrix = Matrix4x4.identity;
            RenderZoneWithMatrix(observerZone, renderingMatrix);

            // Render child zones

            foreach(var child in observerZone.childZonesList)
            {
                float apparentBrightness = starLuminosity / (4 * Mathf.PI * Mathf.Pow(child.distanceFromParent, 2));
                float distance = Mathf.Log((1 / apparentBrightness), 1.075f);

                renderingMatrix = Matrix4x4.TRS(distance * child.directionFromParent, Quaternion.identity, Vector3.one);
                RenderZoneWithMatrix(child, renderingMatrix);
            }

            // Render sibling zones

            if (observerZone.parentZone != null)
            {
                foreach (var child in observerZone.parentZone.childZonesList)
                {
                    if (child != observerZone)
                    {
                        float distanceSquared;
                        Vector3 direction;

                        float distDiff = (float)observerZone.distanceFromParent / child.distanceFromParent;
                        direction = (child.directionFromParent - (observerZone.directionFromParent * distDiff));

                        distanceSquared = Mathf.Pow(observerZone.distanceFromParent, 2) + Mathf.Pow(child.distanceFromParent, 2);

                        float apparentBrightness = starLuminosity / (4 * Mathf.PI * distanceSquared);
                        float unityDistance = Mathf.Log((1 / apparentBrightness), 1.075f);

                        renderingMatrix = Matrix4x4.TRS(unityDistance * direction, Quaternion.identity, Vector3.one);
                        RenderZoneWithMatrix(child, renderingMatrix);
                    }

                }

                {
                    // Render parent zone

                    float distanceSquared = Mathf.Pow(observerZone.distanceFromParent, 2);

                    float apparentBrightness = starLuminosity / (4 * Mathf.PI * distanceSquared);
                    float unityDistance = Mathf.Log((1 / apparentBrightness), 1.075f);

                    renderingMatrix = Matrix4x4.TRS(unityDistance * -observerZone.directionFromParent, Quaternion.identity, Vector3.one);
                    RenderZoneWithMatrix(observerZone.parentZone, renderingMatrix);
                }
            }



        }

        private void RenderZoneWithMatrix(UniverseZone observerZone, Matrix4x4 renderingMatrix)
        {
            var appearance = GetZoneAppearance(observerZone);
            var mesh = appearance.GetComponent<MeshFilter>().sharedMesh;
            var material = appearance.GetComponent<MeshRenderer>().sharedMaterial;

            Graphics.DrawMesh(mesh, renderingMatrix, material, 0);
        }

        public GameObject GetZoneAppearance(UniverseZone zone)
        {
            if (zone.representation == UniverseZone.ZoneRepresentation.STAR)
            {
                return m_starPrefab;
            }
            else if (zone.representation == UniverseZone.ZoneRepresentation.BLACKHOLE)
            {
                return m_blackholePrefab;
            }

            return null;
        }
    }
}
