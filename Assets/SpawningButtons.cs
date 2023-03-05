using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningButtons : MonoBehaviour
{

    [SerializeField] private UbiqController controller; // The Ubiq hand controller that will be used to interact with the menu
    [SerializeField] private GameObject objectToSpawn; // The GameObject that will be spawned when a button is pressed

    private Button currentButton; // The currently selected button

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the UbiqController component attached to the same GameObject as this script
        if (controller == null)
        {
            controller = GetComponent<UbiqController>();
        }

        // Set the default highlight state for all buttons in the menu
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.SetHighlighted(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Use the Ubiq SDK to detect when the hand controller is hovering over a button
        RaycastHit hit;
        if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, Mathf.Infinity))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null)
            {
                // If the hand controller is hovering over a different button than the current one, highlight the new button
                if (button != currentButton)
                {
                    if (currentButton != null)
                    {
                        currentButton.SetHighlighted(false);
                    }
                    button.SetHighlighted(true);
                    currentButton = button;
                }
            }
            else
            {
                // If the hand controller is not hovering over a button, remove the highlight from the current button (if there is one)
                if (currentButton != null)
                {
                    currentButton.SetHighlighted(false);
                    currentButton = null;
                }
            }
        }
        else
        {
            // If the hand controller is not pointing at anything, remove the highlight from the current button (if there is one)
            if (currentButton != null)
            {
                currentButton.SetHighlighted(false);
                currentButton = null;
            }
        }

        // Use the Ubiq SDK to detect when the hand controller clicks
        if (controller.GetButtonDown(UbiqButton.Trigger))
        {
            // If the hand controller clicks on a button, spawn a new instance of the objectToSpawn GameObject at the current position of the hand controller
            if (currentButton != null)
            {
                Instantiate(objectToSpawn, controller.transform.position, Quaternion.identity);
            }
        }
    }
}
    }
}
