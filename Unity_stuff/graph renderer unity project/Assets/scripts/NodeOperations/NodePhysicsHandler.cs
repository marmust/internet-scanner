using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class NodePhysicsHandler : MonoBehaviour
{
    // i wont explain all this
    public NodeStructureHandler structure_handler;
    public VarHolder vars;
    public Rigidbody SelfRB;
    public Vector3 init_cords;
    public bool in_camera_physics_range = false;

    public List<GameObject> connections;
    public float previous_update_time;

    private bool useTanh;

    private LayerMask NodeLayer;

    // this function takes in distance, and calculates the force that should come out
    // so if the object to which the force should be applied is too far away, the force will be negative, if too close, then positive
    public Vector3 CalcTanhOfVector3(Vector3 forceVec3)
    {
        if (!useTanh) return forceVec3;
        if (forceVec3.magnitude * vars.TanhSoften >= 0.005f && forceVec3.magnitude * vars.TanhSoften <= 45f) { // slightly wider range to capture two or three components with a magnitude of <22 but big enough to escape otherwise
            float tanhX = FastTanh(forceVec3.x * vars.TanhSoften);       // we only use magnitude here to save on performance, since this is called a lot and using magnitude is good enough, also saves checking sign values
            float tanhY = FastTanh(forceVec3.y * vars.TanhSoften);
            float tanhZ = FastTanh(forceVec3.z * vars.TanhSoften);
            return new Vector3(tanhX, tanhY, tanhZ);
        }
        else
        {
            float tanhX = (float)Math.Tanh(forceVec3.x * vars.TanhSoften);
            float tanhY = (float)Math.Tanh(forceVec3.y * vars.TanhSoften);
            float tanhZ = (float)Math.Tanh(forceVec3.z * vars.TanhSoften);
            return new Vector3(tanhX, tanhY, tanhZ);
        }
    }

    private float FastTanh(float x) // much faster approximation for values between ~0.05 and 22, since our calculations would be expected to be in that range, this is a good optimisation
    {
        return x / (1f + x * x);
    }

    // wake up settings when object is initialized
    private void Awake()
    {
        // mostly get components to be used later in the script
        structure_handler = gameObject.GetComponent<NodeStructureHandler>();
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
        SelfRB = gameObject.GetComponent<Rigidbody>();
        init_cords = transform.position;
        if(gameObject.name == "mould") previous_update_time = Time.time; // hacky fix for an obscure bug
        useTanh = vars.UseTanh;
        NodeLayer = LayerMask.GetMask("NodeLayer");
    }

    private void Update()
    {
        useTanh = vars.UseTanh;
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

            // if its not the root node then apply the normal physics loop
            // check if its time to update the physics (depends on the throttle)
            if (Time.time >= previous_update_time + vars.SecondsPerPhysicsUpdate)
            {
                previous_update_time = Time.time;
                PhysicsLoop();
            }
        }
    }

    private void PhysicsLoop()
    {
        if (in_camera_physics_range)
        {
            // attract all connected nodes to yourself
            foreach (GameObject connection in connections)
            {
                if (connection.Equals(this)) continue; // don't iterate on self
                float IdealDistance = vars.MinimalChildDistance +
                                      connection.GetComponent<NodeStructureHandler>().connections.Count *
                                      vars.ChildDistanceConnectionsEffect;


                //attraction(connection);
                if (Vector3.Distance(transform.position, connection.transform.position) < IdealDistance)
                {
                    repulsion(connection);
                }
                else
                {
                    attraction(connection);
                }
            }

            Collider[] InRangeObjects = Physics.OverlapSphere(transform.position, vars.ForeignNodeInteractionRange, NodeLayer); //use layer masking instead of tags, which is expensive, especialy with a direct == check

            // repel all unconnected nodes in range
            InRangeObjects.Where(CurrentForeignObject => CurrentForeignObject.name != "mould")
                .ToList()
                .ForEach(CurrentFilteredObject => repulsion(CurrentFilteredObject.gameObject)); //use Linq query
        }
    }

    // Change-notes: we don't need to calculate the other force vector because it will be applied when the loop goes over it, this will just make the sim unstable
    private void repulsion(GameObject other)
    {
        Vector3 MyForceVector = CalcTanhOfVector3(transform.position - other.transform.position) * vars.PhysicsForceGeneralStrength;
        SelfRB.AddForce(MyForceVector * vars.ParentWeight);
    }

    private void attraction(GameObject other)
    {
        Vector3 MyForceVector = CalcTanhOfVector3(other.transform.position - transform.position) * vars.PhysicsForceGeneralStrength;
        SelfRB.AddForce(MyForceVector * vars.ParentWeight);
    }
}
