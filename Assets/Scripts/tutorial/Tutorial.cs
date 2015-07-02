using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class Tutorial : MonoBehaviour {

	public int nroSlide;

	void Start () {
		if (button.textura == -1){GetComponent<RawImage> ().enabled = false;}
	}
	
	// Update is called once per frame
	void Update () {

		if (button.textura == nroSlide) {
			GetComponent<RawImage> ().enabled = true;
		}
		else{
			GetComponent<RawImage> ().enabled = false;
			button.bandera = true;
		}
	}
}
