using Assets.scripts.misc;
using UnityEngine;

public class NodeColorHandler : MonoBehaviour
{
    public SpriteRenderer sprite;
    public LineRenderer linerenderer;
    public VarHolder vars;
    public NodePhysicsHandler PhysicsHandler;
    public NodeStructureHandler StructureHandler;

    public ColorMode ColorModeSwitchFlag;

    private void Awake()
    {
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        linerenderer = gameObject.GetComponent<LineRenderer>();

        PhysicsHandler = gameObject.GetComponent<NodePhysicsHandler>();
        StructureHandler = gameObject.GetComponent<NodeStructureHandler>();

        ColorModeSwitchFlag = vars.ColorMode;

        // take care of newborn nodes (so when they spawn they immediatly get correct color)
        // (without waiting for colormode switch)
        UpdateColors();
  }

    public void SetColor(Color color, bool ChangeLinerenderer = true)
    {
        sprite.color = color;

        if (ChangeLinerenderer)
        {
            linerenderer.startColor = color;
            linerenderer.endColor = color;
        }
    }

    private void Update()
    {
        // detect colormode switch
        if (ColorModeSwitchFlag != vars.ColorMode)
        {
            ColorModeSwitchFlag = vars.ColorMode;
            UpdateColors();
        }
    }

    public void UpdateColors()
    {
        if (vars.ColorMode == ColorMode.in_range)
        {
            if (PhysicsHandler.in_camera_physics_range)
            {
                SetColor(new Color(1.0f, 0.27f, 0.27f, 1.0f));
            }
            else
            {
                SetColor(new Color(1.0f, 1.0f, 1.0f, 0.2f));
            }
        }

        if (vars.ColorMode == ColorMode.is_scanned)
        {
            SetColor(new Color(1.0f, 1.0f, 1.0f, 0.2f));

            if (StructureHandler.expanded)
            {
                SetColor(new Color(0.54f, 0.68f, 1.0f, 1.0f), false);
            }
            else
            {
                SetColor(new Color(1.0f, 1.0f, 1.0f, 0.2f));
            }
        }

        if (vars.ColorMode == ColorMode.none)
        {
            SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }

    }

    // for later use:
    //public void propogate_by_branch_mode(Color parent_color)
    //{
    //    //Color mutated_color = new Color(
    //    //    Mathf.Clamp01(parent_color.r + Random.Range(-vars.by_branch_mutaion_rate, vars.by_branch_mutaion_rate)),
    //    //    Mathf.Clamp01(parent_color.g + Random.Range(-vars.by_branch_mutaion_rate, vars.by_branch_mutaion_rate)),
    //    //    Mathf.Clamp01(parent_color.b + Random.Range(-vars.by_branch_mutaion_rate, vars.by_branch_mutaion_rate))
    //    //);
    //
    //    //SetColor(mutated_color);
    //
    //    foreach (Transform child in transform)
    //    {
    //        NodeColorHandler child_color_handler = child.GetComponent<CodeColorHandler>();
    //
    //        if (child_color_handler != null)
    //        {
    //            child_color_handler.propogate_by_branch_mode(mutated_color);
    //        }
    //    }
    //}
}
