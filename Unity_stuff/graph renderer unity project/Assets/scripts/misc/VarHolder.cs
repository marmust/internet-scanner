using Assets.scripts.misc;
using System.ComponentModel;
using UnityEngine;

public class VarHolder : MonoBehaviour
{
    // for graph structure handling
    [Header("Graph Structure")]
    public string AllNodeUrls;
    public float InitGenRange;

    // for physics settings
    [Header("Physics")]
    [Tooltip("How strongly a node is attracted and repelled.")]
    public float PhysicsForceGeneralStrength;
    public float ForeignNodeInteractionRange;
    public float MinimalChildDistance;
    public float ChildDistanceConnectionsEffect;
    public float TanhSoften;

    // for player to control
    [Tooltip("The coloring mode for the nodes:" +
        "\nin_range - Far away nodes appear faded." +
        "\nby_branch - Nodes inherit the color of their parent." +
        "\nby_relation - Nodes' color will mutate over time")]
    public ColorMode ColorMode = ColorMode.in_range;
    [HideInInspector]
    public float SecondsPerPhysicsUpdate;
    [Range(0.01f,1f)]
    public float FastestPhysicsUpdates = 0.1f;
    [Range(1f, 100f)]
    public float SlowestPhysicsUpdates = 10f;
    [Tooltip("Much more stable, but much slower.")]
    public bool UseTanh = true;


    // for camera movement settings
    [Header("Camera")]
    [Range(1f,20f)]
    public float MovementSpeed = 5.0f;
    [Range(1f,10f)]
    public float RotationSpeed = 3.0f;
    [Tooltip("How fast the camera moves when holding LeftShift")]
    public float BoostValue = 3.0f;

    // to check if the player is inputting a url currently
    [HideInInspector]
    public bool IsTypingUrl;

    // for later use: public float by_branch_mutaion_rate = 0.1f;
}
