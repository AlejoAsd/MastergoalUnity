using UnityEngine;
using System.Collections;

public class ShowBackground : MonoBehaviour {

	void Update(){
		// Si estoy en el juego desabilito el background
		if (MenuController.screenValue == 3) {
			GetComponent<Renderer>().enabled = false;
		} 

		// Si estoy en el menu principal habilito el background
		if (MenuController.screenValue == 0) {
			GetComponent<Renderer>().enabled = true;
		} 

	}
}
