using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class NodeColorHandler : MonoBehaviour
{
    public SpriteRenderer sprite;
    public LineRenderer linerenderer;
    public VarHolder vars;
    public NodePhysicsHandler PhysicsHandler;
    public NodeStructureHandler StructureHandler;

    private int ColorModeRefreshFlag = -1;

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
        if (ColorModeRefreshFlag != vars.ColorModeIDX)
        {
            UpdateColors();
            ColorModeRefreshFlag = vars.ColorModeIDX;
        }
    }

    // 0 - none (all white)
    // 1 - in_range (red if in physics range)
    // 2 - is_scanned (blue if has been scanned)
    // 3 - url_length (the shorter the URL, the greener)
    // 4 - by_branch (mutate the color every connection, each branch has a different color)

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

          if (vars.ColorModeIDX == 4)
          {
              if (transform.parent == null)
              {
                  Color RandomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                  Color NormalizedColor = RandomColor / RandomColor.maxColorComponent;

                  PassDownMutatedColor(NormalizedColor);
              }
          }

          if (vars.ColorModeIDX == 0)
          {
              SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
          }

    }

    // recieve and pass down the mutated color if colormode is by_branch
    public void PassDownMutatedColor(Color PassDownColor)
    {
        // mutate the color
        Color MutatedColor = PassDownColor + new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * vars.ColorMutationRate;
        // normalize the color so it goes between 0 and 1
        Color NormalizedColor = MutatedColor / MutatedColor.maxColorComponent;
        // set our own color to that
        SetColor(NormalizedColor);

        // pass the mutated color down to the children
        var ChildList = gameObject.transform.Cast<Transform>().Select(t => t.gameObject).ToList();

        foreach (Transform childTransform in gameObject.transform)
        {
            // Check that we didn't hit the rotating texture
            if (childTransform.CompareTag("node"))
            {
                childTransform.GetComponent<NodeColorHandler>().PassDownMutatedColor(NormalizedColor);
            }
        }
    }
}
