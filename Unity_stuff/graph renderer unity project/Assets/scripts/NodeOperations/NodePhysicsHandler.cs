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
        for (int x = 0; x < connections.Count; x++)
        {
            if (connections[x] != null)
            {
                Vector3 positionA = transform.position;
                Vector3 positionB = connections[x].GetComponent<Transform>().position;

                float num_of_connections_bias = -connections[x].GetComponent<NodeStructureHandler>().connections.Count() * 1f;
                float biased_distance = Vector3.Distance(positionA, positionB) + num_of_connections_bias;
                float biased_force_vector = CalcConnectedNodesForce(biased_distance);

                self_rb.AddForce((positionB - positionA) * biased_force_vector * Time.deltaTime * 80);
                connections[x].GetComponent<Rigidbody>().AddForce((positionA - positionB) * biased_force_vector * Time.deltaTime * 150);
            }
        }

        if (gameObject.name != "mould" && transform.parent != null)
        {
            Vector3 force = transform.parent.root.position - transform.position;
            self_rb.AddForce(force * Time.deltaTime * vars.central_gravity_strength);
        }

        //if (transform.childCount > 0)
        //{
        Collider[] in_range_objects = Physics.OverlapSphere(transform.position, vars.foreighn_node_interaction_range);

        foreach (var current_foreighn_object in in_range_objects)
        {
            if (!current_foreighn_object.transform.IsChildOf(transform) &&
                current_foreighn_object.transform.childCount > 0 &&
                current_foreighn_object.gameObject != gameObject)
            {
                self_rb.AddForce((transform.position - current_foreighn_object.transform.position) * Time.deltaTime);
            }
        }
        //}

        if (gameObject.name != "mould" && transform.parent == null)
        {
            transform.position = init_cords;
        }
    }
}