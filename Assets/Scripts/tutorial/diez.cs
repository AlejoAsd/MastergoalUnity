﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
public class diez : MonoBehaviour {

	
	void Start () {
		if (button.textura == -1){GetComponent<RawImage> ().enabled = false;}
	}
	
	// Update is called once per frame
	void Update () {
		if (button.textura == 9)
		{
			GetComponent<RawImage> ().enabled = true;
		}
		else{
			GetComponent<RawImage> ().enabled = false;
			button.bandera = true;
		}
	}
}