using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;



/*
READ PLEASE:
if you are here to fix the physics tanh situation then let me give you some context:
the idea here is to have 2 forces:
1: if nodes in "range" add repulsion.    <<< already working so everything good here.
2: keep connected nodes at "distance" using the TANH function.   <<< not working

how (2) supposted to work?
check desmos so understand tanh(x) shape better     >>>     \tanh\left(x-c\right)    ---+
basically x is the distance between nodes, y is the force that should be applied        |
their vector 3 difference * tanh(distance)                                              |
                                                                                        V
C - is the optimal distance, where there are no force applied bcs vector3 is multiplied by 0.

problem with (2):
idk why but nodes go into the -x -y -z quadrant of the world.
this is also happening when using linear (no tanh)
i can reverse it and make the nodes go to +x +y +z by setting ideal distance to negative

CTRL F --()-- to find all problematic places in the code
*/




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
    public Vector3 CalcTanhOfVector3(Vector3 forceVec3, float RequiredDistance)
    {
        //if (!useTanh) return forceVec3;
        //if (forceVec3.magnitude * vars.TanhSoften >= 0.005f && forceVec3.magnitude * vars.TanhSoften <= 45f) { // slightly wider range to capture two or three components with a magnitude of <22 but big enough to escape otherwise
        //    float tanhX = FastTanh(forceVec3.x * vars.TanhSoften - RequiredDistance);       // we only use magnitude here to save on performance, since this is called a lot and using magnitude is good enough, also saves checking sign values
        //    float tanhY = FastTanh(forceVec3.y * vars.TanhSoften - RequiredDistance);
        //    float tanhZ = FastTanh(forceVec3.z * vars.TanhSoften - RequiredDistance);
        //    return new Vector3(tanhX, tanhY, tanhZ);
        //}
        //else
        //{
            float tanhX = (float)Math.Tanh(forceVec3.x - RequiredDistance);
            float tanhY = (float)Math.Tanh(forceVec3.y - RequiredDistance);
            float tanhZ = (float)Math.Tanh(forceVec3.z - RequiredDistance);
            return new Vector3(tanhX, tanhY, tanhZ);
        //}
    }

    private float FastTanh(float x) // much faster approximation for values between ~0.05 and 22, since our calculations would be expected to be in that range, this is a good optimisation
    {
        // i changed the previous optimization bcs it approached 0
        // this was bad because the force should stay constant no matter how far away the node is
        // copy to desmos to see graph: f(x)=\frac{3^{xh}-1}{3^{xh}+1}
        return (Mathf.Pow(3, x) - 1) / (Mathf.Pow(3, x) + 1);
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
            foreach (GameObject connection in connections)
            {
                // --()--
                // the distance that we want the current node to be held at
                float IdealDistance = vars.MinimalChildDistance +
                                      connection.GetComponent<NodeStructureHandler>().connections.Count() *
                                      vars.ChildDistanceConnectionsEffect;

                // this vector is pointing towards THIS node, from the CONNECTION node
                // it is used to pull the CONNECTION node to the correct holding position
                // will be inverted if the node needs to be pushed
                Vector3 PullVector = transform.position - connection.transform.position;
                Vector3 ForceVector = CalcTanhOfVector3(PullVector, IdealDistance) * vars.PhysicsForceGeneralStrength;

                connection.GetComponent<Rigidbody>().AddForce(ForceVector);
            }
        }
    }

    // Change-notes: we don't need to calculate the other force vector because it will be applied when the loop goes over it, this will just make the sim unstable
    //private void repulsion(GameObject other)
    //{
    //    Vector3 MyForceVector = CalcTanhOfVector3(transform.position - other.transform.position) * vars.PhysicsForceGeneralStrength;
    //    SelfRB.AddForce(MyForceVector * vars.ParentWeight);
    //}
    //
    //private void attraction(GameObject other)
    //{
    //    Vector3 MyForceVector = CalcTanhOfVector3(other.transform.position - transform.position) * vars.PhysicsForceGeneralStrength;
    //    SelfRB.AddForce(MyForceVector * vars.ParentWeight);
    //}
}
