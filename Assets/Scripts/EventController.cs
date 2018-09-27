using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour {

    #region PUBLIC Variables
    public GameObject contextMenuSystem;
    public GameObject parabolicPointer;
    [Tooltip("Teleport Vive script should be found on the Camera (eye) object.")]
    public TeleportVive teleportVive;

    [Header("Controllers")]
    public GameObject left;
    public GameObject right;

    public enum States { freeRoam, objectHighlighted, objectInHand, menuActive };
    public States myState;

    [HideInInspector]
    public List<GameObject> shelfProducts = new List<GameObject>();
    [HideInInspector]
    public ContextMenuSystem contextMenuScript;
    [HideInInspector]
    public GameObject[] objects;
    [HideInInspector]
    public GameObject collidingObject;
    [HideInInspector]
    public GameObject objectSelected;
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public Button buttonClicked;
    [HideInInspector]
    public bool menuOpen = false;    
    [HideInInspector]
    public bool touchpadPressed = false;
    [HideInInspector]
    public bool laserHolderOff = true;
    [HideInInspector]
    public bool objectInHand = false;
    [HideInInspector]
    public List<GameObject> materialMenus;   
    #endregion

    #region PRIVATE Variables    
    bool objectInEitherHand = false;
    private GameObject contextMenu;
    private bool parabolicPointerOn = true;
    private bool laserPointerOn = false;
    #endregion
    
    private void Update() {

        if (menuOpen) {
            myState = States.menuActive;         
        }

        if (!menuOpen) {
            //IF there is no colliding object in any instance of the ControllerGrabObject script (on either controller)
            //  THEN -> turn the freeRoam state back on.
            GameObject[] controllers = GameObject.FindGameObjectsWithTag("Controllers");
            objectInEitherHand = false;
                       
            foreach (GameObject obj in controllers) {
                // IF there is a colliding object
                if (collidingObject) {
                    objectInEitherHand = true;
                    //print(collidingObject);
                }
            }
            if (!objectInEitherHand) {
                myState = States.freeRoam;
            }
        }

        if (myState == States.freeRoam) {            
            if (!parabolicPointerOn) {
                if (!menuOpen) {
                    TurnOnParabolicPointer();
                }                
            }            

        } else if (myState == States.objectHighlighted) {
            if (parabolicPointerOn) {
                TurnOffParabolicPointer();
            }

            if (touchpadPressed) {
                touchpadPressed = false;
                if ((collidingObject.tag == "Untagged") || (collidingObject.tag == "Button") || (collidingObject.tag == "CleanButton")) {
                    return;
                }
                if (!menuOpen && !objectInHand) {
                    GameObject menuInScene = GameObject.FindGameObjectWithTag("ContextMenuSystem");
                    //If there is not menuInScene already, go ahead and create teh contextMenu gameobject.
                    if (menuInScene == null) {
                        contextMenu = Instantiate(contextMenuSystem) as GameObject;
                        Vector3 offsetPos = new Vector3(0, 0.25f, 0.25f);
                        Vector3 newPos = collidingObject.transform.position + offsetPos;
                        contextMenu.transform.position = newPos;

                        contextMenuScript = contextMenu.GetComponent<ContextMenuSystem>();
                        contextMenuScript.menuState = ContextMenuSystem.SelectionStates.level_1;
                        TurnOffControllerBalls();

                        ResetShelfProductsList();
                        shelfProducts.Add(collidingObject);

                        contextMenuScript.contextMenuObjHighlighted = collidingObject;
                        objectSelected = collidingObject;                        
                        collidingObject = null;
                        myState = States.menuActive;
                        menuOpen = true;
                    }
                }
                #region TURN ON MATERIAL PANELS
                if (!objectInHand) {
                    if (objectSelected.tag == "PasteBox_4.2oz_Horizontal") {
                        // NOTE:  When you search using TAG it will search all project folders (including prefabs!!!).
                        // This is why it could find it using the TAG and not the name.  I wasn't grabbing the instance!!!!
                        GameObject contextMenuTemp = GameObject.Find("UIContextMenuSystem(Clone)");
                        Transform[] transforms = contextMenuTemp.GetComponentsInChildren<Transform>();
                        for (int i = 0; i < transforms.Length; i++) {
                            if (transforms[i].gameObject.tag == "Menu_4.2oz_PasteBox_Horizontal") {
                                //print("I found: "+ transforms[i].gameObject.name);
                                transforms[i].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                                //print("The " + transforms[i].gameObject.name + " localScale is: " + transforms[i].gameObject.GetComponent<RectTransform>().localScale);
                            }
                        }

                    } else if (objectSelected.tag == "PasteBox_4.1oz_Vertical") {
                        GameObject contextMenuTemp = GameObject.Find("UIContextMenuSystem(Clone)");
                        Transform[] transforms = contextMenuTemp.GetComponentsInChildren<Transform>();
                        foreach (Transform trans in transforms) {
                            if (trans.gameObject.tag == "Menu_4.1oz_PasteBox_Vertical") {
                                trans.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                            }
                        }
                    } else if (objectSelected.tag == "Rinse_16oz") {
                        GameObject contextMenuTemp = GameObject.Find("UIContextMenuSystem(Clone)");
                        Transform[] transforms = contextMenuTemp.GetComponentsInChildren<Transform>();
                        foreach (Transform trans in transforms) {
                            if (trans.gameObject.tag == "Menu_16oz_Rinse") {
                                trans.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                            }
                        }
                    } else if (objectSelected.tag == "Fixodent_2.4oz") {
                        GameObject contextMenuTemp = GameObject.Find("UIContextMenuSystem(Clone)");
                        Transform[] transforms = contextMenuTemp.GetComponentsInChildren<Transform>();
                        foreach (Transform trans in transforms) {
                            if (trans.gameObject.tag == "Menu_Fixodent") {
                                trans.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                            }
                        }
                    } else if (objectSelected.tag == "Toms_4.7oz") {
                        GameObject contextMenuTemp = GameObject.Find("UIContextMenuSystem(Clone)");
                        Transform[] transforms = contextMenuTemp.GetComponentsInChildren<Transform>();
                        foreach (Transform trans in transforms) {
                            if (trans.gameObject.tag == "Menu_4.7oz_PasteBox") {
                                trans.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                            }
                        }
                    }
                }                
                #endregion
            }

        } else if (myState == States.objectInHand) {
            if (parabolicPointerOn) {
                TurnOffParabolicPointer();
            }


        } else if (myState == States.menuActive) {
            if (parabolicPointerOn) {
                TurnOffParabolicPointer();
            }            
            if (!laserPointerOn) {
                TurnOnLaserPointer();
            }           
            MenuExitingCheck();         
        }
    }       

    private void MenuExitingCheck() {
        //print("menuOpen = " + menuOpen);
        //print("collidingObject = " + collidingObject);
        //print("objectSelected = " + objectSelected);
        //print("button = " + button);
        //print("buttonClicked = " + buttonClicked);
        //print("touchPadPressed = " + touchpadPressed);

        if (!button && touchpadPressed) {
            DestroyContextMenu();
        }
    }

    public void DestroyContextMenu() {
        if (menuOpen) {
            contextMenuScript.TurnOffObjectHighlights();
            TurnOnControllerBalls();
            laserHolderOff = false;
            TurnOffLaserPointer();            
            menuOpen = false;
            objectSelected = null;
            objectInEitherHand = false;
            if (left) {
                left.GetComponent<ControllerGrabObject>().collidingObject = null;
            }
            if (right) {
                right.GetComponent<ControllerGrabObject>().collidingObject = null;
            }
            Destroy(contextMenu);
        }        
    }

    public void ResetShelfProductsList() {
        //foreach (var item in shelfProducts) {
        //    print("Shelf Product List item: " + item.name + ".");
        //}
        //print("=============================================");
        shelfProducts.Clear();
        shelfProducts = new List<GameObject>();
        //print("List is cleared.");
        //print("=============================================");
        //foreach (var item in shelfProducts) {
        //    print("Shelf Product List item: " + item.name + ".");
        //}
    }

    private void TurnOffParabolicPointer() {
        parabolicPointer.SetActive(false);
        parabolicPointer.GetComponent<ParabolicPointer>().enabled = false;
        parabolicPointerOn = false;
        teleportVive.CurrentTeleportState = TeleportState.Disabled;
        if (left) {
            Transform[] transforms = left.GetComponentsInChildren<Transform>();
            foreach (Transform trans in transforms) {
                if (trans.gameObject.name == "Pointer") {
                    trans.gameObject.GetComponent<ParabolicPointer>().enabled = false;
                }
            }
        }
        if (right) {
            Transform[] transforms = right.GetComponentsInChildren<Transform>();
            foreach (Transform trans in transforms) {
                if (trans.gameObject.name == "Pointer") {
                    trans.gameObject.GetComponent<ParabolicPointer>().enabled = false;
                }
            }
        }
    }

    private void TurnOnParabolicPointer() {
        if (left) {
            Transform[] transforms = left.GetComponentsInChildren<Transform>();
            foreach (Transform trans in transforms) {
                if (trans.gameObject.name == "Pointer") {
                    trans.gameObject.GetComponent<ParabolicPointer>().enabled = true;
                }
            }
        }
        if (right) {
            Transform[] transforms = right.GetComponentsInChildren<Transform>();
            foreach (Transform trans in transforms) {
                if (trans.gameObject.name == "Pointer") {
                    trans.gameObject.GetComponent<ParabolicPointer>().enabled = true;
                }
            }
        }
        parabolicPointer.SetActive(true);
        //parabolicPointer.GetComponent<ParabolicPointer>().enabled = true;
        parabolicPointerOn = true;
        teleportVive.CurrentTeleportState = TeleportState.None;       
    }

    private void TurnOnLaserPointer() {
        laserPointerOn = true;
        if (left != null && left.activeSelf) {
            left.GetComponent<SteamVR_LaserPointer>().enabled = true;
            if (left.GetComponent<SteamVR_LaserPointer>().holder) {
                GameObject holder = left.GetComponent<SteamVR_LaserPointer>().holder;
                holder.SetActive(true);
            }
        }
        if (right != null && right.activeSelf) {
            right.GetComponent<SteamVR_LaserPointer>().enabled = true;
            if (right.GetComponent<SteamVR_LaserPointer>().holder) {
                GameObject holder = right.GetComponent<SteamVR_LaserPointer>().holder;
                holder.SetActive(true);
            }
        }
        left.GetComponent<ControllerGrabObject>().enabled = false;
        right.GetComponent<ControllerGrabObject>().enabled = false;
    }

    public void TurnOffLaserPointer() {
        if (left != null && left.activeSelf) {
            left.GetComponent<SteamVR_LaserPointer>().enabled = false;
            if (!laserHolderOff) {                
                left.GetComponent<SteamVR_LaserPointer>().holder.SetActive(false);
            }
        }
        if (right != null && right.activeSelf) {
            right.GetComponent<SteamVR_LaserPointer>().enabled = false;
            if (!laserHolderOff) {
                right.GetComponent<SteamVR_LaserPointer>().holder.SetActive(false);
            }
        }           
        laserHolderOff = true;
        laserPointerOn = false;
        left.GetComponent<ControllerGrabObject>().enabled = true;
        right.GetComponent<ControllerGrabObject>().enabled = true;
    }

    private void TurnOffControllerBalls() {
        MeshRenderer leftBallMesh = left.transform.Find("Controller Ball Left").gameObject.GetComponent<MeshRenderer>();
        MeshRenderer rightBallMesh =  right.transform.Find("Controller Ball Right").gameObject.GetComponent<MeshRenderer>();
        leftBallMesh.enabled = false;
        rightBallMesh.enabled = false;
        left.GetComponent<BoxCollider>().enabled = false;
        right.GetComponent<BoxCollider>().enabled = false;
    }

    public void TurnOnControllerBalls() {
        MeshRenderer leftBallMesh = left.transform.Find("Controller Ball Left").gameObject.GetComponent<MeshRenderer>();
        MeshRenderer rightBallMesh = right.transform.Find("Controller Ball Right").gameObject.GetComponent<MeshRenderer>();
        leftBallMesh.enabled = true;
        rightBallMesh.enabled = true;
        left.GetComponent<BoxCollider>().enabled = true;
        right.GetComponent<BoxCollider>().enabled = true;
    }
}
