using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class button : MonoBehaviour {
	public static int textura = -1;
	public static bool bandera = true;
	// Use this for initialization
	void Start()
	{
	}
	
	void Update () {
		//if (Input.GetMouseButtonUp(0))
		if (MenuController.screenValue == 9)
		{
			if (textura == -1) {
				textura = 0;
				bandera = true;
			}
			GetComponent<Button>().onClick.AddListener(delegate(){ contador();});
		}
	}
	void contador ()
	{
		if (bandera && textura <19) 
		{
			bandera = false;
			textura = textura + 1;	
			Debug.Log (textura);
		}
		//if (textura == 12) {
		//	textura = -1;
		//}


	}
}
