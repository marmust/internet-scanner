using UnityEngine;

public class VarHolder : MonoBehaviour
{
    // for graph structure handling
    public string AllNodeUrls;
    public float InitGenRange;

    // for physics settings
    public float PhysicsForceGeneralStrength;
    public float ForeighnNodeInteractionRange;
    public float MinimalChildDistance;
    public float ChildDistanceConnectionsEffect;
    public float ParentWeight;
    public float TanhSoften;

    // for player to controll
    public string ColorMode = "in_range";
    public float SecondsPerPhysicsUpdate;
    // maybe dont let the player controll that
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
