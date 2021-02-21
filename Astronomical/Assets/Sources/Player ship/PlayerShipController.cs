using UnityEngine;

namespace Astronomical
{
    public class PlayerShipController : MonoBehaviour
    {
        static public Vector3 PlayerShipPosition { get { return Instance.transform.position; } }

        static private PlayerShipController Instance;

        private void Start()
        {
            Instance = this;
        }

        [SerializeField]
        [Tooltip("Meters per second")]
        private float sublightSpeed = 10f;

        private bool m_mouselookEnabled = true;
        private void Update()
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            transform.Translate(vertical * transform.forward + horizontal * transform.right, Space.World);

            if (m_mouselookEnabled)
            {
                float mouseX, mouseY;
                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");

                transform.Rotate(-mouseY, mouseX, 0f, Space.Self);
            }


            if (Input.GetKeyDown(KeyCode.Tab))
            {
                m_mouselookEnabled = !m_mouselookEnabled;
            }
        }
    }
}
