using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeColorHandler : MonoBehaviour
{
    public SpriteRenderer sprite;
    public LineRenderer linerenderer;
    public VarHolder vars;
    public NodePhysicsHandler PhysicsHandler;
    public NodeStructureHandler StructureHandler;

    private float ColorRefreshCooldown = 0.1f;
    //private float lastUpdateTime = Time.time;

    private void Awake()
    {
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        linerenderer = gameObject.GetComponent<LineRenderer>();

        PhysicsHandler = gameObject.GetComponent<NodePhysicsHandler>();
        StructureHandler = gameObject.GetComponent<NodeStructureHandler>();
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
        //if (Time.time - lastUpdateTime >= ColorRefreshCooldown)
        //{
            UpdateColors();
        //    lastUpdateTime = Time.time;
        //}
    }

    // 0 - none (all white)
    // 1 - in_range (red if in physics range)
    // 2 - is_scanned (blue if has been scanned)
    // 3 - url_length (the shorter the URL, the greener)

    public void UpdateColors()
      {
          if (vars.ColorModeIDX == 1)
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

          if (vars.ColorModeIDX == 2)
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

          if (vars.ColorModeIDX == 3)
          {
              if (gameObject.name != "mould")
              {
                  SetColor(new Color(1.0f, 1.0f, 1.0f, 0.2f));

                  // length - hue function: \frac{1}{0.2\max\left(x,\ 20\right)-f}   <<< copy paste to desmos (f=3)
                  int length = StructureHandler.node_url.Length;
                  float hue = 1 / (0.2f * Math.Max(length, 30) - 5);
                
                  SetColor(new Color(0.3f, 1.0f, 0.3f, hue), false);
              }
          }

          if (vars.ColorModeIDX == 0)
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
