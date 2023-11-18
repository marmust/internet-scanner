using UnityEngine;
using TMPro;

public class ObjectLockScanSystem : MonoBehaviour
{
    public float clickCooldown = 1.0f;
    public float lastClickTime = 0.0f;

    public UiSystem ui_system;

    private NodeStructureHandler current_node_handler;
    private GameObject targeted_object = null;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targeted_object = hit.collider.gameObject;
            NodeStructureHandler targeted_object_NH = targeted_object.GetComponent<NodeStructureHandler>();

            ui_system.locked_on_node = true;
            ui_system.locked_node_url = targeted_object_NH.node_url;
            ui_system.locked_node_cycle = targeted_object_NH.is_cycle;
            ui_system.locked_node_scanned = targeted_object_NH.expanded;
            ui_system.target_node = targeted_object;
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