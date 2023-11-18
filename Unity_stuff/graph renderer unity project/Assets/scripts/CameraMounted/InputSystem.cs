using UnityEngine;
using System.Collections.Generic;

public class InputSystem : MonoBehaviour
{
    public float movement_speed = 5.0f;
    public float rotation_speed = 3.0f;
    public float boost_value = 3.0f;

    public List<Transform> hold_on_to;
    public List<Vector3> init_positions;

    private float is_boosting = 1;

    private void Awake()
    {
        foreach (Transform current_transform in hold_on_to)
        {
            init_positions.Add(current_transform.localPosition);
        }
    }

    void Update()
    {
        // handle physics throttle
        VarHolder vars = GetComponent<VarHolder>();
        if (vars.seconds_per_physics_update <= vars.slowest_physics_updates)
        {
            vars.seconds_per_physics_update += (vars.slowest_physics_updates - vars.seconds_per_physics_update + 1) / 2f * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            vars.typing_url = !vars.typing_url;
        }


        if (Input.GetKey(KeyCode.G))
        {
            if (vars.seconds_per_physics_update >= vars.fastest_physics_updates)
            {
                vars.seconds_per_physics_update -= 20f * Time.deltaTime;
            }
        }

        if (!vars.typing_url)
        {
            // there are some objects that need to be attached to the camera (such as mould object)
            // teleport them here (ik how children work this is so physics objects dontdrift off)
            for (int x = 0; x < hold_on_to.Count; x++)
            {
                hold_on_to[x].localPosition = init_positions[x];
            }

            // handle camera movement inputs
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * movement_speed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * movement_speed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * movement_speed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * movement_speed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += transform.up * movement_speed * Time.deltaTime * is_boosting;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.position -= transform.up * movement_speed * Time.deltaTime * is_boosting;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                is_boosting = boost_value;
            }
            else
            {
                is_boosting = 1;
            }

            float mouseX = Input.GetAxis("Mouse X") * rotation_speed;
            float mouseY = -Input.GetAxis("Mouse Y") * rotation_speed;

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