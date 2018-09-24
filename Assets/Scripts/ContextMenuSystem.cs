using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ContextMenuSystem : MonoBehaviour {

    #region PUBLIC Variables
    [Header("Buttons")]
    public Button backButton;
    public Button nextButton;
    public Button plusButton;
    public Button minusButton;

    public ColorBlock colorBlock;

    public enum MenuStates { level_1, level_2 };
    public MenuStates menuState;

    [HideInInspector]
    public GameObject objectInHand;
    #endregion

    #region PRIVATE Variables
    private Material[] objectMaterials;

    private ColorBlock startingColors;
    private GameObject canvasObj;
    private GameObject eventControllerGO;
    private EventController eventController;
    private GameObject parentObj;
    private Transform[] children;
    #endregion


    void Start() {
        canvasObj = transform.GetChild(0).gameObject;
        startingColors = backButton.colors;

        objectMaterials = objectInHand.GetComponent<HighlightController>().materials;
        objectInHand.GetComponentInChildren<Renderer>().material = objectMaterials[1];
        eventControllerGO = GameObject.FindGameObjectWithTag("Event Controller");
        eventController = eventControllerGO.GetComponent<EventController>();

        parentObj = objectInHand.transform.parent.gameObject;
    }

    private void Update() {
        if (menuState == MenuStates.level_1) {
            backButton.colors = colorBlock;
            minusButton.colors = colorBlock;
            plusButton.colors = startingColors;
        }

        if (menuState == MenuStates.level_2) {
            backButton.colors = startingColors;
            minusButton.colors = startingColors;
            plusButton.colors = colorBlock;
        }
    }

    public void ExitButton() {
        objectInHand.GetComponentInChildren<Renderer>().material = objectMaterials[0];        
        eventController.myState = EventController.States.freeRoam;
        eventController.menuOpen = false;
        children = null;
        objectInHand = null;
        Destroy(this.gameObject);
    }

    //When this button is selected, it will make the parent of the object in hand active.  This will show the group of objects switch to the highlighted material all at once.
    public void ExpandSelection() {                
        children = parentObj.GetComponentsInChildren<Transform>();
        foreach (Transform obj in children) {
            if (obj.gameObject.GetComponent<Renderer>()) {
                //print("The children transform is " + obj.name);
                Material mat = obj.parent.gameObject.GetComponent<HighlightController>().materials[1];
                obj.GetComponent<Renderer>().material = mat;
            }
        }
        menuState = MenuStates.level_2;
    }

    public void ShrinkSelection() {
        foreach (Transform obj in children) {
            if (obj.gameObject.GetComponent<Renderer>()) {                
                Material mat = obj.parent.gameObject.GetComponent<HighlightController>().materials[0];
                obj.GetComponent<Renderer>().material = mat;
            }
        }
        objectInHand.GetComponentInChildren<Renderer>().material = objectMaterials[1];
        menuState = MenuStates.level_1;
    }


}
