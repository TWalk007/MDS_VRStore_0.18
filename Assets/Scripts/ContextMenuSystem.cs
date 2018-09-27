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

    //[Header("Context Sensitive Menus")]
    //public GameObject pasteBox4OzHoriz;
    //public GameObject pasteBox4OzVert;
    //public GameObject pasteBox47OzToms;
    //public GameObject pasteBoxFixodent;
    //public GameObject Rinse16Oz;


    [Header("Prefabs for Material Swapping")]
    public GameObject[] prefabs;

    [Header("ColorBlock for Unselectable Button")]
    public ColorBlock colorBlock;

    [HideInInspector]
    public bool IsButtonClicked = false;

    public enum SelectionStates { level_1, level_2 };
    public SelectionStates menuState;
    
    [HideInInspector]
    public GameObject parentObj;

    //[Header("Material Menu Panels")]
    //public GameObject[] materialMenus;

    [HideInInspector]
    public List<GameObject> materialMenuList;

    [HideInInspector]
    public GameObject contextMenuObjHighlighted;
    #endregion

    #region PRIVATE Variables
    private Material[] objectMaterials;
    private ColorBlock startingColors;
    private GameObject canvasObj;
    private GameObject eventControllerGO;
    private EventController eventController;    
    private Transform[] children;
    #endregion

    void Start() {
        canvasObj = transform.GetChild(0).gameObject;
        startingColors = backButton.colors;
        objectMaterials = contextMenuObjHighlighted.GetComponent<HighlightController>().materials;
        contextMenuObjHighlighted.GetComponentInChildren<Renderer>().material = objectMaterials[1];
        eventControllerGO = GameObject.FindGameObjectWithTag("Event Controller");
        eventController = eventControllerGO.GetComponent<EventController>();
        parentObj = contextMenuObjHighlighted.transform.parent.gameObject;
        
        //This didn't work because it was happening AFTER it already adjusted the Material Menu panels scale.
        //Essentially it was putting the scale back at ZERO after I already set it to 1,1,1.  So it's turned off.
        //PopulateMaterialMenuArray();
    }

    private void Update() {
        if (menuState == SelectionStates.level_1) {
            backButton.colors = colorBlock;
            minusButton.colors = colorBlock;
            plusButton.colors = startingColors;
        }

        if (menuState == SelectionStates.level_2) {
            backButton.colors = startingColors;
            minusButton.colors = startingColors;
            plusButton.colors = colorBlock;
        }

        if (IsButtonClicked) {
            IsButtonClicked = false;
            IdentifyProductType();
        }
    }

    private void IdentifyProductType() {
        //print(eventController.buttonClicked.name);
        if (eventController.buttonClicked.name == "NEXT") {
            if (menuState == ContextMenuSystem.SelectionStates.level_1) {
                menuState = ContextMenuSystem.SelectionStates.level_2;
            }
        } else if (eventController.buttonClicked.name == "BACK") {
            if (menuState == ContextMenuSystem.SelectionStates.level_2) {
                menuState = ContextMenuSystem.SelectionStates.level_1;
            }        
        } else if (eventController.buttonClicked.name == "PLUS") {
            ExpandSelection();
        } else if (eventController.buttonClicked.name == "MINUS") {
            ShrinkSelection();

            //TODO// Nothing happens here as there is only a single piece of art.
        } else if (eventController.buttonClicked.name == "ProHealth_4.2oz_HealthyFresh") {
            
        //TODO//  Still need to add this artwork.
        } else if (eventController.buttonClicked.name == "ProHealth_4.2oz_Clinical") {

        } else if (eventController.buttonClicked.name == "GumDetoxify_OrigMint") {
            // When updating object's material it will need to:
            //  1) Find the ProHealth 4.2oz_HealthyFresh materials and put them to a new Materials[] variable.
            //  2) Update the object's parent's HighlightController script to the correct materials.
            //  3) Change the material to the highlighted material from it's HighlightController.



        } else if (eventController.buttonClicked.name == "GumDetoxify_GentleWht") {

        } else if (eventController.buttonClicked.name == "GumDetoxify_ExtraFresh") {

        } else if (eventController.buttonClicked.name == "Toms_AntiPlqWht") {

        } else if (eventController.buttonClicked.name == "Toms_Sensitive") {

        } else if (eventController.buttonClicked.name == "Toms_SimplyWht") {

        } else if (eventController.buttonClicked.name == "Toms_WholeCare") {

        } else if (eventController.buttonClicked.name == "Rinse_GlamWht") {

        } else if (eventController.buttonClicked.name == "Rinse_GumCare") {

        } else if (eventController.buttonClicked.name == "Rinse_GumCare2") {

        } else if (eventController.buttonClicked.name == "Fixodent_Original") {

        } else if (eventController.buttonClicked.name == "Fixodent_Free") {

        } else {
            print("IdentifyProductType(): Product Type unknown!");
        }
    }

    //This function takes in a prefab and changes the materials on the highlighted object to this prefab.
    private void UpdateMaterials (GameObject prefabObj) {
        Material[] prefabMats = prefabObj.GetComponent<HighlightController>().materials;
        if (menuState == SelectionStates.level_1) {
            contextMenuObjHighlighted.GetComponent<HighlightController>().materials = prefabMats;
            Material mat = contextMenuObjHighlighted.gameObject.GetComponent<HighlightController>().materials[1];
            contextMenuObjHighlighted.transform.GetChild(0).GetComponent<Renderer>().material = mat;
        }
        if (menuState == SelectionStates.level_2) {
            foreach (Transform obj in children) {
                if (obj.gameObject.GetComponent<Renderer>()) {
                    obj.parent.gameObject.GetComponent<HighlightController>().materials = prefabMats;
                    Material mat = obj.parent.gameObject.GetComponent<HighlightController>().materials[1];
                    obj.GetComponent<Renderer>().material = mat;
                }
            }
        }
    }

    public void TurnOffObjectHighlights() {
        Transform[] children = parentObj.GetComponentsInChildren<Transform>();
        foreach (Transform obj in children) {
            if (obj.gameObject.GetComponent<Renderer>()) {
                Material mat = obj.transform.parent.gameObject.GetComponent<HighlightController>().materials[0];
                obj.GetComponent<Renderer>().material = mat;
            }
        }
    }

    //When this button is selected, it will make the parent of the object in hand active.
    //This will show the group of objects switch to the highlighted material all at once.
    public void ExpandSelection() {                
        children = parentObj.GetComponentsInChildren<Transform>();
        foreach (Transform obj in children) {
            if (obj.gameObject.GetComponent<Renderer>()) {
                //print("The children transform is " + obj.name);
                Material mat = obj.parent.gameObject.GetComponent<HighlightController>().materials[1];
                obj.GetComponent<Renderer>().material = mat;
            }
        }
        menuState = SelectionStates.level_2;
    }

    public void ShrinkSelection() {
        if (menuState == SelectionStates.level_2) {
            foreach (Transform obj in children) {
                if (obj.gameObject.GetComponent<Renderer>()) {
                    Material mat = obj.parent.gameObject.GetComponent<HighlightController>().materials[0];
                    obj.GetComponent<Renderer>().material = mat;
                }
            }
            contextMenuObjHighlighted.GetComponentInChildren<Renderer>().material = objectMaterials[1];
            menuState = SelectionStates.level_1;
        }        
    }


    //This didn't work because it was happening AFTER it already adjusted the Material Menu panels scale.
    //Essentially it was putting the scale back at ZERO after I already set it to 1,1,1.  So it's turned off.
    //private void PopulateMaterialMenuArray() {
    //    Transform[] transforms = GetComponentsInChildren<Transform>();
    //    foreach (Transform trans in transforms) {
    //        if (trans.gameObject.tag == "MaterialMenu") {
    //            eventController.contextMenuSystem.GetComponent<ContextMenuSystem>().materialMenuList.Add(trans.gameObject);
    //            //print(trans.gameObject.name);
    //            trans.GetChild(0).gameObject.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 0f);
    //        }
    //    }
    //}
}
