using UnityEngine;

public class NodeColorHandler : MonoBehaviour
{
    public SpriteRenderer sprite;
    public LineRenderer linerenderer;
    public VarHolder vars;
    public NodePhysicsHandler physics_handler;

    private void Awake()
    {
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        linerenderer = gameObject.GetComponent<LineRenderer>();
        physics_handler = gameObject.GetComponent<NodePhysicsHandler>();
    }

    public void set_color(Color color)
    {
        sprite.color = color;
        linerenderer.startColor = color;
        linerenderer.endColor = color;
    }

    private void Update()
    {
        if (vars.ColorMode == "in_range")
        {
            if (physics_handler.in_camera_physics_range)
            {
                set_color(new Color(1.0f, 0.27f, 0.27f, 1.0f));
            }
            else
            {
                set_color(new Color(1.0f, 1.0f, 1.0f, 0.5f));
            }
        }

        //if (vars.color_mode == "by_branch")
        //{
        //    if (transform.parent != null)
        //    {
        //        set_color(transform.parent.getcomponent)
        //    }
        //}
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
    //    //set_color(mutated_color);
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
