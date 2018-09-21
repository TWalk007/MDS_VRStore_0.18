//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public struct PointerEventArgs {
    public uint controllerIndex;
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void PointerEventHandler(object sender, PointerEventArgs e);


public class SteamVR_LaserPointer : MonoBehaviour {
    #region PUBLIC Variables  /////////////////////////////////////////////////////
    public EventController eventController;
    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool addRigidBody = false;
    public Transform reference;
    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;
    #endregion

    #region PRIVATE Variables ////////////////////////////////////////////////////
    private Transform previousContact = null;
    private RaycastHit hit;
    private SteamVR_TrackedController controller;
    #endregion

    private void OnEnable() {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.PadUnclicked += PadUnclicked;
    }

    private void OnDisable() {
        controller.PadUnclicked -= PadUnclicked;
    }

    

    void Start() {
        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;
        holder.tag = "Laser";

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody) {
            if (collider) {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        } else {
            if (collider) {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
    }

    public virtual void OnPointerIn(PointerEventArgs e) {
        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(PointerEventArgs e) {
        if (PointerOut != null)
            PointerOut(this, e);
    }


    // Update is called once per frame
    void Update() {
        if (!isActive) {
            isActive = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

        float dist = 100f;


        Ray raycast = new Ray(transform.position, transform.forward);
        bool bHit = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact != hit.transform) {
            PointerEventArgs args = new PointerEventArgs();
            if (controller != null) {
                args.controllerIndex = controller.controllerIndex;
            }
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform) {
            PointerEventArgs argsIn = new PointerEventArgs();
            if (controller != null) {
                argsIn.controllerIndex = controller.controllerIndex;
            }
            argsIn.distance = hit.distance;
            argsIn.flags = 0;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }
        if (!bHit) {
            previousContact = null;
        }
        if (bHit && hit.distance < 100f) {
            dist = hit.distance;
        }

        if (bHit) {

            // Places the button variable into the event controller as a placeholder.  This allows
            // both controllers to access the same variable for reference so they aren't duplicating events.
            if (hit.transform.tag == "Button") {
                eventController.button = hit.transform.gameObject.GetComponent<Button>();                
                eventController.button.OnSelect(new BaseEventData(EventSystem.current));                
            }
            if (hit.transform.tag != "Button") {
                if (eventController.button != null) {
                    eventController.button.GetComponent<Button>().OnDeselect(new BaseEventData(EventSystem.current));
                }                
            }
                
            if (controller != null && controller.padPressed) {
                pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);

            } else {
                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            }
            pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
        }
    }

    // This is a controller event subscription.  It executes when the event takes place.
    private void PadUnclicked(object sender, ClickedEventArgs e) {
        Button menuButton = eventController.button;
        if (menuButton) {
            if (menuButton.name == "EXIT") {
                Destroy(GameObject.FindGameObjectWithTag("ContextMenuSystem"));
                eventController.menuOpen = false;
                eventController.myState = EventController.States.freeRoam;
                eventController.laserHolderOff = false;
                eventController.TurnOffLaserPointer();
            } else if (menuButton.name == "BACK") {
                eventController.contextMenuSystem.GetComponent<ContextMenuSystem>().menuState = ContextMenuSystem.MenuStates.level_1;
            } else if (menuButton.name == "NEXT") {
                if (eventController.contextMenuSystem.GetComponent<ContextMenuSystem>().menuState == ContextMenuSystem.MenuStates.level_1) {
                    eventController.contextMenuSystem.GetComponent<ContextMenuSystem>().menuState = ContextMenuSystem.MenuStates.level_2;
                }
            } else if (menuButton.name == "PLUS") {
                // Grow selection
            } else if (menuButton.name == "MINUS") {
                // Shrink selection.
            }
        }
    }
}
