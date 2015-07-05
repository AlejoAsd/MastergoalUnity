using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class button2 : MonoBehaviour {
	//public static int textura = -1;
	//public static bool bandera = true;
	// Use this for initialization
	void Start()
	{
	}
	
	void Update () {
		//if (Input.GetMouseButtonUp(0))
		if (MenuController.screenValue == 9)
		{
			GetComponent<Button>().onClick.AddListener(delegate(){ contador();});
		}
	}
	void contador ()
	{
		if (button.bandera && button.textura >-1) 
		{
			button.bandera = false;
			button.textura = button.textura - 1;	
			Debug.Log (button.textura);
			//Debug.Log (textura);
			//GetComponent<Canvas>().enabled = false;
		}

		
	}
}
