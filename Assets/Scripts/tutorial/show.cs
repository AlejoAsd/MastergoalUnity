using UnityEngine;
using System.Collections;

public class show : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (MenuController.screenValue == 0) {
			GetComponent<Canvas> ().enabled = false;
		}
		if (MenuController.screenValue == 9) {
			GetComponent<Canvas> ().enabled = true;
		}
	}
}
