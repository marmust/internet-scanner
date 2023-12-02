using UnityEngine;
using TMPro;

public class ObjectLockScanSystem : MonoBehaviour
{
    public float clickCooldown = 1.0f;
    public float lastClickTime = 0.0f;

    public UiSystem ui_system;
    public VarHolder vars;

    private NodeStructureHandler current_node_handler;
    private GameObject targeted_object = null;

    void Update()
    {
        if (!vars.IsPaused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("node"))
                {
                    targeted_object = hit.collider.gameObject;
                    ui_system.locked_on_node = true;
                    ui_system.target_node = targeted_object;
                }
            }
            else
            {
                ui_system.locked_on_node = false;
                targeted_object = null;
            }

            if (Input.GetMouseButtonDown(0) && Time.time > lastClickTime + clickCooldown && targeted_object != null)
            {
                lastClickTime = Time.time;

                TriggerNode(targeted_object);
            }
        }
    }

    void ChangeText(TextMeshProUGUI text_component, string write, Color color)
    {
        text_component.text = write;
        text_component.color = color / 256;
    }

    private void TriggerNode(GameObject node_GO)
    {
        current_node_handler = node_GO.GetComponent<NodeStructureHandler>();

        if (current_node_handler != null)
        {
            current_node_handler.ExpandNode();
        }
    }
}
