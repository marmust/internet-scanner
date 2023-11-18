using UnityEngine;

public class CameraRangeSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("node"))
        {
            other.GetComponent<NodePhysicsHandler>().in_camera_physics_range = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("node"))
        {
            other.GetComponent<NodePhysicsHandler>().in_camera_physics_range = false;
        }
    }
}
