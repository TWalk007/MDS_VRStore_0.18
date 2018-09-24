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
    public GameObject[] objects;
    [HideInInspector]
    public GameObject collidingObject;
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public bool menuOpen = false;
    [HideInInspector]
    public bool touchpadPressed = false;
    [HideInInspector]
    public bool laserHolderOff = true;
    #endregion

    #region PRIVATE Variables
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
            bool objectInEitherHand = false;

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
                if (!menuOpen) {

                    GameObject menuInScene = GameObject.FindGameObjectWithTag("ContextMenuSystem");

                    //If there is not menuInScene already, go ahead and create teh contextMenu gameobject.
                    if (menuInScene == null) {
                        GameObject contextMenu = Instantiate(contextMenuSystem) as GameObject;
                        Vector3 offsetPos = new Vector3(0, 0.25f, 0.25f);

                        // the "Controller.transform.pos" was changed from "collidingObject.transform.position".
                        // This could screw up the placement of the "UI Context Menu Sysem".
                        Vector3 newPos = collidingObject.transform.position + offsetPos;
                        contextMenu.transform.position = newPos;
                        ContextMenuSystem contextMenuScript = contextMenu.GetComponent<ContextMenuSystem>();
                        contextMenuScript.menuState = ContextMenuSystem.MenuStates.level_1;

                        TurnOffControllerBalls();

                        contextMenuScript.objectInHand = collidingObject;

                        shelfProducts.Add(collidingObject);
                        collidingObject = null;

                        myState = States.menuActive;
                        menuOpen = true;                       
                    }
                }
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
        }
    }

    public void TurnOffObjectHighlights() {
        // Create a list of all objects that get highlighted.
        // When finished working in the menu iterate through the list and unhighlight the items.
        // Clear the List.
        //print("Beginning to clear object highlights...");
        foreach (var item in shelfProducts) {
            Material unHighlightedMaterial = item.gameObject.GetComponent<HighlightController>().materials[0];
            item.GetComponentInChildren<Renderer>().material = unHighlightedMaterial;
            //print("Object " + item.name + " is no longer highlighted.");
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
    }

    private void TurnOnParabolicPointer() {
        parabolicPointer.SetActive(true);
        parabolicPointer.GetComponent<ParabolicPointer>().enabled = true;
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
