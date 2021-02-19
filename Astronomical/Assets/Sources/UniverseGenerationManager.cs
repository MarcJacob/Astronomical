using System.Collections;
using UnityEngine;

namespace Astronomical
{
    /// <summary>
    /// Take in a ZoneGenerator as the "root zone generator" to indirectly generate a Universe.
    /// This means the actual scale of the Universe is going to depend on the kind of generator as its root.
    /// </summary>
    public class UniverseGenerationManager : MonoBehaviour
    {
        [SerializeField]
        private ZoneGenerator m_rootZoneGenerator;

        private UniverseZone m_rootZone;

        private void Start()
        {
            m_rootZone = m_rootZoneGenerator.GenerateZone();
            ActivateUniverseManagers();
        }

        private void ActivateUniverseManagers()
        {
            GetComponent<UniverseRenderManager>().StartRender(m_rootZone);
        }
    }
}
