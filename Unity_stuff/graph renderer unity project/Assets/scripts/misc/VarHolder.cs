using Assets.scripts.misc;
using UnityEngine;

public class VarHolder : MonoBehaviour
{
    // for graph structure handling
    [InspectorName("Graph Structure")]
    public string AllNodeUrls;
    public float InitGenRange;

    // for physics settings
    [InspectorName("Physics")]
    public float PhysicsForceGeneralStrength;
    public float ForeignNodeInteractionRange;
    public float MinimalChildDistance;
    public float ChildDistanceConnectionsEffect;
    public float ParentWeight;
    public float TanhSoften;

    // for player to control
    public ColorMode ColorMode = ColorMode.in_range;
    [HideInInspector]
    public float SecondsPerPhysicsUpdate;

    public float FastestPhysicsUpdates = 0.1f;
    public float SlowestPhysicsUpdates = 10f;


    // for camera movement settings
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 3.0f;
    public float BoostValue = 3.0f;

    // to check if the player is inputting a url currently
    public bool IsTypingUrl;

    // for later use: public float by_branch_mutaion_rate = 0.1f;
}
