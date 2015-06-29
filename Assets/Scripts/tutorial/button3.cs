using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class button3 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (MenuController.screenValue == 9)
		{
			GetComponent<Button>().onClick.AddListener(delegate(){ menu();});
		}
	}
	void menu(){
		button.bandera = true;
		button.textura = -1;
		MenuController.screenValue = 0;
	}
}
