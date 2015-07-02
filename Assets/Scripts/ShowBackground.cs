using UnityEngine;
using System.Collections;

public class ShowBackground : MonoBehaviour
{
	void Update()
	{
		// Si estoy en el juego desabilito el background
		if (MenuController.screenValue == Constants.GAMESP || MenuController.screenValue == Constants.GAMEMP || MenuController.screenValue == Constants.GAMEMPOFFLINE)
		{
			GetComponent<Renderer>().enabled = false;
		} 

		// Si estoy en el menu principal habilito el background
		if (MenuController.screenValue == Constants.MAIN)
		{
			GetComponent<Renderer>().enabled = true;
		} 
	}
}
