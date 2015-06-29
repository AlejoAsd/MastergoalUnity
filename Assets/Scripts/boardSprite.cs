using UnityEngine;
using System.Collections;

public class boardSprite : MonoBehaviour {


	public Sprite lineas_sprite; //lineas de la cancha

	// Use this for initialization
	void Start () {
		// Insertar el sprite de las lineas de la cancha
		// crea el objeto
		GameObject lineasSprite = new GameObject("Lineas");
		lineas_sprite.name = "LineasSprite";
		// Agrega el componente "SpriteRenderer" al gameobject
		lineasSprite.AddComponent<SpriteRenderer>();
		// Asigna el sprite
		lineasSprite.GetComponent<SpriteRenderer>().sprite = lineas_sprite;
		// Modifica la rotacion, la posicion y la escala
		lineasSprite.transform.Rotate(90, 90, 0);
		lineasSprite.transform.position = new Vector3(0.0f, 0.05f, 0.0f);
		lineasSprite.transform.localScale = new Vector3(3.0f, 3.0f, 0.0f);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
