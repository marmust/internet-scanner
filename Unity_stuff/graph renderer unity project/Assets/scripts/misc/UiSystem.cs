using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.scripts.misc;

public class UiSystem : MonoBehaviour
{
    public bool locked_on_node;

    public GameObject target_indicator;
    public GameObject target_node;
    public VarHolder vars;
    public GameObject URL_input;
    public TMP_InputField url_inputfield;

    public float physics_update_rate;

    public TextMeshProUGUI locked_node_url_text;
    public TextMeshProUGUI locked_node_cycle_text;
    public TextMeshProUGUI locked_node_scanned_text;
    public TextMeshProUGUI physics_update_rate_value_text;

    public TextMeshProUGUI max_physics_throttle_text;
    public TextMeshProUGUI min_physics_throttle_text;

    public TextMeshProUGUI ColorModeName;
    public TextMeshProUGUI ColorModePurpose;

    public RectTransform canvas_rctf;
    public RectTransform target_indicator_rctf;
    public RectTransform physics_update_rate_throttle;

    private Color throttle_red_shift;

    private void Awake()
    {
        // reset all ui to initial states
        ChangeText(locked_node_url_text, "NULL", new Color(69f, 69f, 69f)); // 256f isn't a valid value for alpha, let's clamp to 255 and use default
        ChangeText(locked_node_cycle_text, "NULL", new Color(69f, 69f, 69f));
        ChangeText(locked_node_scanned_text, "NULL", new Color(69f, 69f, 69f));
        ChangeText(physics_update_rate_value_text, "00", new Color(255f, 255f, 255f));
        max_physics_throttle_text.text = (vars.FastestPhysicsUpdates).ToString();
        min_physics_throttle_text.text = (vars.SlowestPhysicsUpdates).ToString();

        physics_update_rate_throttle.localPosition = new Vector3(927, -507, 0);
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
    }

    void ChangeText(TextMeshProUGUI text_component, string write, Color color)
    {
        text_component.text = write;
        if(color.a == 1) color.a *= 256; // alpha defaults to 1, so we need to multiply by 256 to get the correct value
        text_component.color = color / 256;
    }

    private void Update()
    { 
        throttle_red_shift = Color.Lerp(new Color(255f, 61f, 61f), new Color(255f, 255f, 255f), vars.SecondsPerPhysicsUpdate / vars.SlowestPhysicsUpdates);

        physics_update_rate_throttle.GetComponent<RawImage>().color = throttle_red_shift / 256;
        physics_update_rate_throttle.localPosition = Vector3.Lerp(new Vector3(927, -507, 0), new Vector3(927, -126, 0), 1 - vars.SecondsPerPhysicsUpdate / vars.SlowestPhysicsUpdates);
        ChangeText(physics_update_rate_value_text, ((int)vars.SecondsPerPhysicsUpdate).ToString().PadLeft(2, '0'), throttle_red_shift);

        URL_input.SetActive(vars.IsTypingUrl);

        if (locked_on_node)
        {
            var node = target_node.GetComponent<NodeStructureHandler>();

            target_indicator.SetActive(true);
            Vector2 screen_pos = RectTransformUtility.WorldToScreenPoint(Camera.main, target_node.transform.position);
            target_indicator_rctf.anchoredPosition = screen_pos - canvas_rctf.sizeDelta / 2f;

            ChangeText(locked_node_url_text, node.node_url, new Color(138f, 175f, 255f));

            if (node.expanded)
            {
                ChangeText(locked_node_scanned_text, "SCANNED", new Color(54f, 255f, 97f));

                if (node.is_cycle)
                {
                    ChangeText(locked_node_cycle_text, "LEADS TO ITSELF", new Color(54f, 255f, 97f));
                }
                else
                {
                    ChangeText(locked_node_cycle_text, "NON CYCLICAL", new Color(138f, 175f, 255f));
                }
            }
            else
            {
                string text;
                if (node.scanning)
                {
                    text = "SCANNING " + node.downloadProgress + "%";
                }
                else
                {
                    if (node.expanded)
                    {
                        text = "SCANNED";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(node.scanError))
                        {
                            text = "NOT SCANNED";
                        }
                        else
                        {
                            text = "ERROR - " + node.scanError;
                        }
                    }
                }
                ChangeText(locked_node_scanned_text, text, new Color(255f, 79f, 79f));
                ChangeText(locked_node_cycle_text, "UNKOWN - SCAN REQUIRED", new Color(255f, 79f, 79f));

            }
        }
        else
        {
            target_indicator.SetActive(false);
            ChangeText(locked_node_url_text, "NULL", new Color(69f, 69f, 69f));
            ChangeText(locked_node_cycle_text, "NULL", new Color(69f, 69f, 69f));
            ChangeText(locked_node_scanned_text, "NULL", new Color(69f, 69f, 69f));
        }
    }

    public void OnColorModeChange()
    {
        ChangeText(ColorModeName, vars.colorMode, vars.colorMode);
        ChangeText(ColorModePurpose, ColorModes.GetColorModePurpose(vars.colorMode), new Color(100f, 100f, 100f));
    }

    public void CollectUrlFromTextBox()
    {
        string inputted_url = url_inputfield.text;
        url_inputfield.text = "";
        vars.IsTypingUrl = false;
        Vector3 spawn_cords = vars.gameObject.transform.Find("initial_node_spawner").transform.position;

        if (!vars.AllNodeUrls.Contains(inputted_url))
        {
            GameObject current_node = Instantiate(GameObject.Find("mould"));
            current_node.GetComponent<NodePhysicsHandler>().init_cords = spawn_cords;
            current_node.transform.position = spawn_cords;
            current_node.GetComponent<NodeStructureHandler>().node_url = inputted_url;
            current_node.name = inputted_url;
        }
    }
}
