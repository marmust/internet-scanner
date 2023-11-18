using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class NodePhysicsHandler : MonoBehaviour
{
    // i wont explain all this
    public NodeStructureHandler structure_handler;
    public VarHolder vars;
    public Rigidbody self_rb;
    public Vector3 init_cords;
    public bool in_camera_physics_range = false;

    public List<GameObject> connections;
    public float previous_update_time;

    // this function takes in distance, and calculates the force that should come out
    // so if the object to which the force should be applied is too far away, the force will be negative, if too close, then positive
    public float CalcConnectedNodesForce(float distance)
    {
        return (float)Math.Tanh(distance - vars.distance_sweet_spot);
    }

    // wake up settings when object is initialized
    private void Awake()
    {
        // mostly get components to be used later in the script
        structure_handler = gameObject.GetComponent<NodeStructureHandler>();
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
        self_rb = gameObject.GetComponent<Rigidbody>();
        init_cords = transform.position;
    }

    private void Update()
    {
        // make sure that the texture of the node is looking at the camera
        // that texture can be found as one of the children in the nodes (first one in the list)
        transform.GetChild(0).transform.LookAt(vars.transform);

        // mould is an onject used to clone the nodes
        // its found under the camera object
        // we dont want it to move around so dont apply physics to it
        if (gameObject.name != "mould")
        {
            // check if its the root node in a system
            // because  clones always have a parent
            if (transform.parent == null)
            {
                // if it is the root node we lock it in its initial position (check void awake()) so the graph wont fly away by accident
                transform.position = init_cords;
            }
            else
            {
                // if its not the root node then apply the normal physics loop
                // check if its time to update the physics (depends on the throttle)
                if (Time.time >= previous_update_time + vars.seconds_per_physics_update)
                {
                    previous_update_time = Time.time;
                    PhysicsLoop();
                }
            }
        }
    }

    private void PhysicsLoop()
    {
        if (in_camera_physics_range)
        {
            for (int x = 0; x < connections.Count; x++)
            {
                if (connections[x] != null)
                {
                    //// central gravity - done
                    //// attempt to stay at sweet spot distance (bias by num_of_connections)
                    //if (connections[x].name != "mould")
                    //{
                    //    Vector3 my_position = transform.position;
                    //    Vector3 other_position = connections[x].GetComponent<Transform>().position;
                    //
                    //    float num_of_connections_bias = connections[x].GetComponent<node_structure_handler>().connections.Count();
                    //    float distance = Vector3.Distance(my_position, other_position);
                    //    float biased_distance = connected_nodes_force(distance) * (num_of_connections_bias*num_of_connections_bias + 1);
                    //    Vector3 force_vector = (other_position - my_position) * biased_distance;
                    //    Vector3 normalized_force_vector = force_vector * Time.deltaTime * vars.physics_force_general_strength;
                    //
                    //    self_rb.AddForce(normalized_force_vector);
                    //    connections[x].GetComponent<Rigidbody>().AddForce(-normalized_force_vector);
                    //}
                }
            }

            if (gameObject.name != "mould" && transform.parent != null)
            {
                Vector3 force = transform.position - transform.parent.root.position;
                self_rb.AddForce(force * vars.central_gravity_strength);
            }

            Collider[] in_range_objects = Physics.OverlapSphere(transform.position, vars.foreighn_node_interaction_range);

            foreach (var current_foreighn_object in in_range_objects)
            {
                if (!current_foreighn_object.transform.IsChildOf(transform) &&
                    current_foreighn_object.transform.childCount > 0 &&
                    current_foreighn_object.gameObject != gameObject)
                {
                    self_rb.AddForce((transform.position - current_foreighn_object.transform.position));
                }
            }

            if (transform.parent != null)
            {
                if (transform.childCount == 1)
                {
                    self_rb.AddForce((transform.parent.transform.position - transform.position) * 10);
                }
            }
        }
    }
}
