using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public InputActionReference lookAction;

    // Start is called before the first frame update
    void Start()
    {
        lookAction.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        float horizontalLook = lookInput.x;
        float verticalLook = lookInput.y;

        // Adjust the rotation speed according to your preference
        float rotationSpeed = 1f;

        CinemachineVirtualCamera virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            CinemachineOrbitalTransposer orbitalTransposer = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            if (orbitalTransposer != null)
            {
                // Rotate horizontally
                orbitalTransposer.m_XAxis.Value += horizontalLook * rotationSpeed;

                // Rotate vertically
                CinemachineComposer composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
                if (composer != null)
                {
                    composer.m_TrackedObjectOffset.y += verticalLook * rotationSpeed;
                    composer.m_TrackedObjectOffset.y = Mathf.Clamp(composer.m_TrackedObjectOffset.y, -5f, 5f); // Optional: Limit vertical rotation range
                }
            }
        }
    }


}
