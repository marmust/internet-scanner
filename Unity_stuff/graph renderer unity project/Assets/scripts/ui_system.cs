using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ui_system : MonoBehaviour
{
    public bool locked_on_node;

    public string locked_node_url;
    public bool locked_node_cycle;
    public bool locked_node_scanned;

    public GameObject target_indicator;
    public GameObject target_node;
    public var_holder vars;
    public GameObject URL_input;
    public TMP_InputField url_inputfield;

    public float physics_update_rate;

    public TextMeshProUGUI locked_node_url_text;
    public TextMeshProUGUI locked_node_cycle_text;
    public TextMeshProUGUI locked_node_scanned_text;
    public TextMeshProUGUI physics_update_rate_value_text;

    public TextMeshProUGUI max_physics_throttle_text;
    public TextMeshProUGUI min_physics_throttle_text;

    public RectTransform canvas_rctf;
    public RectTransform target_indicator_rctf;
    public RectTransform physics_update_rate_throttle;

    private Color throttle_red_shift;

    private void Awake()
    {
        // reset all ui to initial states
        change_text(locked_node_url_text, "NULL", new Color(69f, 69f, 69f, 256f));
        change_text(locked_node_cycle_text, "NULL", new Color(69f, 69f, 69f, 256f));
        change_text(locked_node_scanned_text, "NULL", new Color(69f, 69f, 69f, 256f));
        change_text(physics_update_rate_value_text, "00", new Color(256f, 256f, 256f, 256f));
        max_physics_throttle_text.text = (vars.fastest_physics_updates).ToString();
        min_physics_throttle_text.text = (vars.slowest_physics_updates).ToString();

        physics_update_rate_throttle.localPosition = new Vector3(927, -507, 0);
        vars = GameObject.Find("Main Camera").GetComponent<var_holder>();
    }

    void change_text(TextMeshProUGUI text_component, string write, Color color)
    {
        text_component.text = write;
        text_component.color = color / 256;
    }

    private void Update()
    {
        throttle_red_shift = Color.Lerp(new Color(256f, 61f, 61f, 256f), new Color(256f, 256f, 256f, 256f), vars.seconds_per_physics_update / vars.slowest_physics_updates);

        physics_update_rate_throttle.GetComponent<RawImage>().color = throttle_red_shift / 256;
        physics_update_rate_throttle.localPosition = Vector3.Lerp(new Vector3(927, -507, 0), new Vector3(927, -126, 0), 1 - vars.seconds_per_physics_update / vars.slowest_physics_updates);
        change_text(physics_update_rate_value_text, ((int)vars.seconds_per_physics_update).ToString().PadLeft(2, '0'), throttle_red_shift);

        if (vars.typing_url)
        {
            URL_input.SetActive(true);
        }
        else
        {
            URL_input.SetActive(false);
        }

        if (locked_on_node)
        {
            target_indicator.SetActive(true);
            Vector2 screen_pos = RectTransformUtility.WorldToScreenPoint(Camera.main, target_node.transform.position);
            target_indicator_rctf.anchoredPosition = screen_pos - canvas_rctf.sizeDelta / 2f;

            change_text(locked_node_url_text, locked_node_url, new Color(138f, 175f, 256f, 256f));

            if (locked_node_scanned)
            {
                change_text(locked_node_scanned_text, "SCANNED", new Color(54f, 256f, 97f, 256f));

                if (locked_node_cycle)
                {
                    change_text(locked_node_cycle_text, "LEADS TO ITSELF", new Color(54f, 256f, 97f, 256f));
                }
                else
                {
                    change_text(locked_node_cycle_text, "NON CYCLICAL", new Color(138f, 175f, 256f, 256f));
                }
            }
            else
            {
                change_text(locked_node_scanned_text, "NOT SCANNED", new Color(256f, 79f, 79f, 256f));
                change_text(locked_node_cycle_text, "UNKOWN - SCAN REQUIRED", new Color(256f, 79f, 79f, 256f));
            }
        }
        else
        {
            target_indicator.SetActive(false);
            change_text(locked_node_url_text, "NULL", new Color(69f, 69f, 69f, 256f));
            change_text(locked_node_cycle_text, "NULL", new Color(69f, 69f, 69f, 256f));
            change_text(locked_node_scanned_text, "NULL", new Color(69f, 69f, 69f, 256f));
        }
    }

    public void collect_url_from_textbox()
    {
        string inputted_url = url_inputfield.text;
        url_inputfield.text = "";
        vars.typing_url = false;
        Vector3 spawn_cords = vars.gameObject.transform.FindChild("initial_node_spawner").transform.position;

        if (!vars.all_node_urls.Contains(inputted_url))
        {
            GameObject current_node = Instantiate(GameObject.Find("mould"));
            current_node.GetComponent<node_physics_handler>().init_cords = spawn_cords;
            current_node.transform.position = spawn_cords;
            current_node.GetComponent<node_structure_handler>().node_url = inputted_url;
            current_node.name = inputted_url;
            vars.all_node_urls += inputted_url + " \n ";
        }
    }
}