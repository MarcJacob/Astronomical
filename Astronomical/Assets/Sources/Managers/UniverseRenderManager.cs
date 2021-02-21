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

        private UniversePositionAnchor m_rootAnchor;

        private PlayerUniversalPositionManager m_playerUniversalPositionManagerComponent;

        private void Awake()
        {
            m_playerUniversalPositionManagerComponent = GetComponent<PlayerUniversalPositionManager>();
        }

        public void StartRender()
        {
            m_rootAnchor = Universe.RootPositionAnchor;
        }

        private void Update()
        {
            if (m_rootAnchor != null)
            {
                RenderCurrentUniverseView(m_playerUniversalPositionManagerComponent.CurrentUniversePositionAnchor);
            }
        }

        private void RenderCurrentUniverseView(UniversePositionAnchor observerAnchor)
        {
            // Render ourselves

            Matrix4x4 renderingMatrix = Matrix4x4.identity;
            RenderAnchorWithMatrix(observerAnchor, renderingMatrix, false);

            // Until we reach the root anchor, keep rendering parent & parent siblings
            UniversePositionAnchor currentAnchor = observerAnchor.parentAnchor;
            decimal previousDistanceTravelled = 0;
            while (currentAnchor.parentAnchor != null)
            {
                // Render siblings
                foreach (var anchor in currentAnchor.parentAnchor.childAnchorsList)
                {
                    if (anchor != currentAnchor)
                    {
                        decimal distanceToSiblingSquared;
                        Vector3 directionToSibling;

                        if (anchor.distanceFromParent == 0)
                        {
                            distanceToSiblingSquared = currentAnchor.distanceFromParent * currentAnchor.distanceFromParent;
                            directionToSibling = -currentAnchor.directionFromParent;
                        }
                        else
                        {
                            distanceToSiblingSquared = (decimal)(Mathf.Pow((float)anchor.distanceFromParent, 2) + Mathf.Pow((float)currentAnchor.distanceFromParent, 2));
                            decimal distanceDifferential = currentAnchor.distanceFromParent / anchor.distanceFromParent;

                            directionToSibling = anchor.directionFromParent - (currentAnchor.directionFromParent * (float)distanceDifferential);
                        }

                        float apparentBrightness = starLuminosity / (4 * Mathf.PI * (float)distanceToSiblingSquared);
                        float distance = (50f + Mathf.Log((1 / (float)apparentBrightness), 1.075f));

                        renderingMatrix = Matrix4x4.TRS(distance * directionToSibling, Quaternion.identity, Vector3.one);
                        RenderAnchorWithMatrix(anchor, renderingMatrix);
                    }
                }
                // Render parent
                {
                    if (currentAnchor.distanceFromParent > 0)
                    {
                        previousDistanceTravelled = currentAnchor.distanceFromParent;
                        float apparentBrightness = starLuminosity / (4 * Mathf.PI * Mathf.Pow((float)currentAnchor.distanceFromParent, 2));
                        float distance = 50f + Mathf.Log((1 / apparentBrightness), 1.075f);
                        renderingMatrix = Matrix4x4.TRS(distance * -currentAnchor.directionFromParent, Quaternion.identity, Vector3.one);
                    }
                    else
                    {
                        if (previousDistanceTravelled > 0)
                        {
                            float apparentBrightness = starLuminosity / (4 * Mathf.PI * Mathf.Pow((float)previousDistanceTravelled, 2));
                            float distance = 50f + Mathf.Log((1 / apparentBrightness), 1.075f);
                            renderingMatrix = Matrix4x4.TRS(distance * -currentAnchor.directionFromParent, Quaternion.identity, Vector3.one);
                        }
                        else
                        {
                            renderingMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                        }
                    }
                    RenderAnchorWithMatrix(currentAnchor.parentAnchor, renderingMatrix, previousDistanceTravelled > 0);
                }

                
                currentAnchor = currentAnchor.parentAnchor;
            }
        }

        private void RenderAnchorWithMatrix(UniversePositionAnchor observerZone, Matrix4x4 renderingMatrix, bool adaptToPlayerPosition = true)
        {
            var appearance = GetZoneAppearance(observerZone);

            if (appearance == null) return;
            var mesh = appearance.GetComponent<MeshFilter>().sharedMesh;
            var material = appearance.GetComponent<MeshRenderer>().sharedMaterial;

            if (adaptToPlayerPosition)
            {
                Vector4 position = renderingMatrix.GetColumn(3);
                position += (Vector4)PlayerShipController.PlayerShipPosition;
                renderingMatrix.SetColumn(3, position);
            }


            Graphics.DrawMesh(mesh, renderingMatrix, material, 0);
        }

        public GameObject GetZoneAppearance(UniversePositionAnchor zone)
        {
            if (zone.representation == UniversePositionAnchor.ZoneRepresentation.STAR)
            {
                return m_starPrefab;
            }
            else if (zone.representation == UniversePositionAnchor.ZoneRepresentation.BLACKHOLE)
            {
                return m_blackholePrefab;
            }

            return null;
        }
    }
}
