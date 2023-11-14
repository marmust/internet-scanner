using UnityEngine;
using TMPro;

public class object_lock_and_scan_system : MonoBehaviour
{
    public float clickCooldown = 1.0f;
    public float lastClickTime = 0.0f;

    public ui_system ui_system;

    private node_structure_handler current_node_handler;
    private GameObject targeted_object = null;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targeted_object = hit.collider.gameObject;
            node_structure_handler targeted_object_NH = targeted_object.GetComponent<node_structure_handler>();

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

            trigger_node(targeted_object);
        }
    }

    void change_text(TextMeshProUGUI text_component, string write, Color color)
    {
        text_component.text = write;
        text_component.color = color / 256;
    }

    private void trigger_node(GameObject node_GO)
    {
        current_node_handler = node_GO.GetComponent<node_structure_handler>();

        if (current_node_handler != null)
        {
            current_node_handler.expand_node();
        }
    }
}