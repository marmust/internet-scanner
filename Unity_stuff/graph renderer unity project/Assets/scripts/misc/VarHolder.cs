using Assets.scripts.misc;
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
    public int MinimalChildDistance;
    public float ChildDistanceConnectionsEffect;
    public float RepulsionRadius;
    public float TanhSoften;

  // for player to control
  [Tooltip("The coloring mode for the nodes:" +
      "\nin_range - Nodes in physics range are red." +
      "\nby_branch - Nodes inherit the color of their parent." +
      "\nby_relation - Nodes' color will mutate over time")]

    // 0 - none (all white)
    // 1 - in_range (red if in physics range)
    // 2 - is_scanned (blue if has been scanned)
    // 3 - url_length (the shorter the URL, the greener)
    // 4 - by_branch (mutate the color every connection, each branch has a different color)
    // 5 - by_site (mutate the color by the hash of the domain)
    [HideInInspector]
    public ColorModes colorMode = ColorMode.NONE;
    // only for by_branch colormode
    public float ColorMutationRate = 0.42f;
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

    // to check if the player is currently paused
    [HideInInspector]
    public bool IsPaused;

    // to check if camera mode is on
    [HideInInspector]
    public bool IsCameraOn;
}
