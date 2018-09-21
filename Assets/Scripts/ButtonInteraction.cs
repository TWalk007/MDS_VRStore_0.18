using UnityEngine;


public class ButtonInteraction : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        print(other.transform.name + " has triggered this button.");
    }

}
