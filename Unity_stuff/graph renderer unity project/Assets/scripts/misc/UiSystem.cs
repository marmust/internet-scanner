using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    // 0 - none (all white)
    // 1 - in_range (red if in physics range)
    // 2 - is_scanned (blue if has been scanned)
    // 3 - url_length (the shorter the URL, the greener)
    // 4 - by_branch (mutate the color every connection, each branch has a different color)
    private List<string> ColorModeNames = new List<string> {"NONE", "IN_RANGE", "IS_SCANNED", "URL_LENGTH", "BY_BRANCH"};
    private List<string> ColorModePurposes = new List<string> {"all white",
                                                               "nodes is 'physics range' are red (helps with optimization)",
                                                               "nodes that have been scanned are blue (helps with seeing 'explored regions')",
                                                               "the shorter the URL, the greener (helps detect 'main' sites)",
                                                               "colors mutate over connections, (helps detect webpages that are closely connected)"};
    private List<Color> ColorModeAsociatedColors = new List<Color> {new Color(255f, 255f, 255f, 255f),
                                                                    new Color(255f, 37f, 37f, 255f),
                                                                    new Color(138f, 175f, 255f, 255f),
                                                                    new Color(56f, 255f, 79f, 255f),
                                                                    new Color(134f, 41f, 255f, 255f)};

    private void Awake()
    {
        // reset all ui to initial states
        ChangeText(locked_node_url_text, "NULL", new Color(69f, 69f, 69f, 256f));
        ChangeText(locked_node_cycle_text, "NULL", new Color(69f, 69f, 69f, 256f));
        ChangeText(locked_node_scanned_text, "NULL", new Color(69f, 69f, 69f, 256f));
        ChangeText(physics_update_rate_value_text, "00", new Color(256f, 256f, 256f, 256f));
        max_physics_throttle_text.text = (vars.FastestPhysicsUpdates).ToString();
        min_physics_throttle_text.text = (vars.SlowestPhysicsUpdates).ToString();

        physics_update_rate_throttle.localPosition = new Vector3(927, -507, 0);
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
    }

    void ChangeText(TextMeshProUGUI text_component, string write, Color color)
    {
        text_component.text = write;
        text_component.color = color / 256;
    }

    private void Update()
    {
        throttle_red_shift = Color.Lerp(new Color(256f, 61f, 61f, 256f), new Color(256f, 256f, 256f, 256f), vars.SecondsPerPhysicsUpdate / vars.SlowestPhysicsUpdates);

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

            ChangeText(locked_node_url_text, node.node_url, new Color(138f, 175f, 256f, 256f));

            if (node.expanded) {
                ChangeText(locked_node_scanned_text, "SCANNED", new Color(54f, 256f, 97f, 256f));

                if (node.is_cycle)
                {
                    ChangeText(locked_node_cycle_text, "LEADS TO ITSELF", new Color(54f, 256f, 97f, 256f));
                }
                else
                {
                    ChangeText(locked_node_cycle_text, "NON CYCLICAL", new Color(138f, 175f, 256f, 256f));
                }
            }
            else {
                string text;
                if (node.scanning) {
                    text = "SCANNING "+node.downloadProgress+"%";
                } else {
                    if (node.expanded) {
                        text = "SCANNED";
                    } else {
                        if (string.IsNullOrEmpty(node.scanError)) {
                            text = "NOT SCANNED";
                        } else {
                            text = "ERROR - "+node.scanError;
                        }
                    }
                }
                ChangeText(locked_node_scanned_text, text, new Color(256f, 79f, 79f, 256f));
                ChangeText(locked_node_cycle_text, "UNKOWN - SCAN REQUIRED", new Color(256f, 79f, 79f, 256f));
                
            }
        }
        else
        {
            target_indicator.SetActive(false);
            ChangeText(locked_node_url_text, "NULL", new Color(69f, 69f, 69f, 256f));
            ChangeText(locked_node_cycle_text, "NULL", new Color(69f, 69f, 69f, 256f));
            ChangeText(locked_node_scanned_text, "NULL", new Color(69f, 69f, 69f, 256f));
        }
    }

    public void OnColorModeChange()
    {
        ChangeText(ColorModeName, ColorModeNames[vars.ColorModeIDX], ColorModeAsociatedColors[vars.ColorModeIDX]);
        ChangeText(ColorModePurpose, ColorModePurposes[vars.ColorModeIDX], new Color(100f, 100f, 100f, 256f));
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
