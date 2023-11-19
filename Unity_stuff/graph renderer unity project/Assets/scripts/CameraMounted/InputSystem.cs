using UnityEngine;
using System.Collections.Generic;

public class InputSystem : MonoBehaviour
{
    public List<Transform> HoldOnTo;
    public List<Vector3> InitPositions;

    private float is_boosting = 1;

    private void Awake()
    {
        foreach (Transform current_transform in HoldOnTo)
        {
            InitPositions.Add(current_transform.localPosition);
        }
    }

    void Update()
    {
        // handle physics throttle
        VarHolder vars = GetComponent<VarHolder>();
        if (vars.SecondsPerPhysicsUpdate <= vars.SlowestPhysicsUpdates)
        {
            vars.SecondsPerPhysicsUpdate += (vars.SlowestPhysicsUpdates - vars.SecondsPerPhysicsUpdate + 1) / 2f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.G))
        {
            if (vars.SecondsPerPhysicsUpdate >= vars.FastestPhysicsUpdates)
            {
                vars.SecondsPerPhysicsUpdate -= 20f * Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            vars.IsTypingUrl = !vars.IsTypingUrl;
        }

        if (!vars.IsTypingUrl)
        {
            // there are some objects that need to be attached to the camera (such as mould object)
            // teleport them here (ik how children work this is so physics objects dont drift off)
            for (int x = 0; x < HoldOnTo.Count; x++)
            {
                HoldOnTo[x].localPosition = InitPositions[x];
            }

            // handle camera movement inputs
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * vars.MovementSpeed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * vars.MovementSpeed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * vars.MovementSpeed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * vars.MovementSpeed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += transform.up * vars.MovementSpeed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.position -= transform.up * vars.MovementSpeed * Time.deltaTime * is_boosting;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                is_boosting = vars.BoostValue;
            }
            else
            {
                is_boosting = 1;
            }

            float mouseX = Input.GetAxis("Mouse X") * vars.RotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * vars.RotationSpeed;

            transform.Rotate(Vector3.up, mouseX, Space.World);
            if (transform.rotation.y < 180 && transform.rotation.y > -180)
            {
                transform.Rotate(Vector3.right, mouseY, Space.Self);
            }

            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
