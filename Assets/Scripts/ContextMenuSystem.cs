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
    
    [HideInInspector]
    GameObject prefab;

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

        PopulateMaterialMenuArray();
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
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.1oz_GumDetoxify_OrigMint") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "GumDetoxify_GentleWht") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.1oz_GumDetoxify_GentleWht") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "GumDetoxify_ExtraFresh") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.1oz_GumDetoxify_ExtraFresh") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Toms_AntiPlqWht") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.7oz_Toms_AntiPlqWht") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Toms_Sensitive") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.7oz_Toms_Sensitive") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Toms_SimplyWht") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.7oz_Toms_SimplyWht") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Toms_WholeCare") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_4.7oz_Toms_WholeCare") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Rinse_GlamWht") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "RinseBottle_16oz_3DWhite_Burgundy") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);


        } else if (eventController.buttonClicked.name == "Rinse_GumCare_Blue") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "RinseBottle_16oz_GumCare_Blue") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Rinse_GumCare_Green") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "RinseBottle_16oz_GumCare_Green") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Fixodent_Original") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_2.4oz_Fixodent_Original") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

        } else if (eventController.buttonClicked.name == "Fixodent_Free") {
            for (int i = 0; i < this.prefabs.Length; i++) {
                if (this.prefabs[i].name == "PasteBox_2.4oz_Fixodent_Free") {
                    prefab = this.prefabs[i];
                }
            }
            UpdateMaterials(prefab);

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
     
    private void PopulateMaterialMenuArray() {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform trans in transforms) {
            if (trans.gameObject.tag == "MaterialMenu") {
                eventController.contextMenuSystem.GetComponent<ContextMenuSystem>().materialMenuList.Add(trans.gameObject);
            }
        }
    }
}
