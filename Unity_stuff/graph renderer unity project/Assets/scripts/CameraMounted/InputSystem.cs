using UnityEngine;
using System.Collections.Generic;

public class InputSystem : MonoBehaviour
{
    public List<Transform> HoldOnTo;
    public List<Vector3> InitPositions;
    public VarHolder vars;
    public UiSystem YouISystem;

    private float is_boosting = 1;
    private float totalXRotation = 0f;

    private void Awake()
    {
        foreach (Transform current_transform in HoldOnTo)
        {
            InitPositions.Add(current_transform.localPosition);
        }
    }

    void Update()
    {
        // toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            vars.IsPuased = !vars.IsPuased;
        }

        if (!vars.IsPuased)
        {
            // handle physics throttle
            if (vars.SecondsPerPhysicsUpdate <= vars.SlowestPhysicsUpdates)
            {
                vars.SecondsPerPhysicsUpdate += (vars.SlowestPhysicsUpdates - vars.SecondsPerPhysicsUpdate + 1) / 2f * Time.deltaTime;
            }

            if (!vars.IsTypingUrl)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    vars.ColorModeIDX = (vars.ColorModeIDX + 1) % 4;
                    YouISystem.OnColorModeChange();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    vars.ColorModeIDX = (vars.ColorModeIDX + 3) % 4;
                    YouISystem.OnColorModeChange();
                }
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
                    transform.position -= is_boosting * Time.deltaTime * vars.MovementSpeed * transform.right; // re-ordered multiplication so vector is done last (this is a small performance fix, but not a big deal)
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.position += is_boosting * Time.deltaTime * vars.MovementSpeed * transform.right;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    transform.position += is_boosting * Time.deltaTime * vars.MovementSpeed * transform.forward;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.position -= is_boosting * Time.deltaTime * vars.MovementSpeed * transform.forward;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    transform.position += is_boosting * Time.deltaTime * vars.MovementSpeed * transform.up;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    transform.position -= is_boosting * Time.deltaTime * vars.MovementSpeed * transform.up;
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

                totalXRotation = Mathf.Clamp(totalXRotation + mouseY, -90, 90);
                transform.localEulerAngles = new Vector3(totalXRotation, transform.localEulerAngles.y, 0); // this HOPEFULLY fixes the x-axis randomly inverting whilst still preventing the camera from flipping upside down
            }
        }

        // handle mouse / cursor locking
        if (vars.IsTypingUrl || vars.IsPuased)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
