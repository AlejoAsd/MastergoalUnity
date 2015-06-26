using UnityEngine;
using System.Collections;

/*
Se posee una matriz de 15x11, los valores de las celdas son:
0 : celda vacia 
1 : ficha de jugador 1 (servidor)
2 : ficha de jugador 2 (cliente)
3 : ficha arquero de jugador 1 (servidor)
4 : ficha arquero de jugador 2 (cliente)
5 : pelota
*/

public class PlayerController : MonoBehaviour {
	
 	

	// Matriz que representa el tablero de juego
	public static char[,] board = new char[15, 11]; 
	
	// Posicion de ficha y posicion destino
	int fichaX, fichaY, ballX, ballY, destinoX, destinoY;

	// Variable que almacena el tag de la ficha seleccionada
	string selected = null;
	public static bool ballSelected = false;

	public GUIStyle customStyle;

	// Para manejo de turno
	//public static int turn;
	//NetworkView networkView;
	
	void Start(){
		// Cargo la matriz con 0s (celdas vacias)
		for (int i = 0; i<=14; i++){
			for(int j = 0; j<=10; j++){
				board[i,j] = '0';
			}
		}

		// Cargo la matriz con los valores segun el nivel
		switch (MenuController.level) {
		case 1:
			board[10,5] = '1';
			board[4,5] = '2';
			break;
		case 2:
			board[10,5] = '1';
			board[12,5] = '1';
			board[4,5] = '2';
			board[2,5] = '2';
			break;
		case 3:
			board[8,2] = '1';
			board[8,8] = '1';
			board[10,3] = '1';
			board[10,7] = '1';
			board[12,5] = '3';

			board[6,2] = '2';
			board[6,8] = '2';
			board[4,3] = '2';
			board[4,7] = '2';
			board[2,5] = '4';
			break;
		}
		board[7,5] = '5';
		// Para manejo de turno
		//networkView = GetComponent<NetworkView> ();
		//turn = 1;
		//mensajeError("nde gay");
	}

	void Update(){	
		if (GetComponent<NetworkView>().isMine)
		{
			InputMovement();
		}

		if (Network.peerType == NetworkPeerType.Disconnected)
			Network.Destroy(GetComponent<NetworkView>().viewID);
	}
	
	void InputMovement(){
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

				if (Network.peerType == NetworkPeerType.Server /*&& turn == 1*/){

					if (ballSelected == false){
						// Selecciono o deselecciono la ficha presionada
						if (hit.collider.tag == "P1F1" ) {
							selectDeselectPiece("P1F1");
						}

						if (hit.collider.tag == "P1F2" ) {
							selectDeselectPiece("P1F2");
						}

						if (hit.collider.tag == "P1F3" ) {
							selectDeselectPiece("P1F3");
						}

						if (hit.collider.tag == "P1F4" ) {
							selectDeselectPiece("P1F4");
						}

						if (hit.collider.tag == "P1F5" ) {
							selectDeselectPiece("P1F5");
						}

						// Selecciona una casilla a donde se movera la ficha seleccionada
						if (hit.collider.tag == "Box" && selected != null) {
							movePiece(hit);
						}
					} 
					else {
						if (hit.collider.tag == "Box") {
							moveBall(hit);
						}
					}
				}

				if (Network.peerType == NetworkPeerType.Client /*&& turn == 2*/){
					if (ballSelected == false){
							// Selecciono o deselecciono la ficha presionada
							if (hit.collider.tag == "P2F1" ) {
								selectDeselectPiece("P2F1");
							}
							
							if (hit.collider.tag == "P2F2" ) {
								selectDeselectPiece("P2F2");
							}

							if (hit.collider.tag == "P2F3" ) {
								selectDeselectPiece("P2F3");
							}

							if (hit.collider.tag == "P2F4" ) {
								selectDeselectPiece("P2F4");
							}

							if (hit.collider.tag == "P2F5" ) {
								selectDeselectPiece("P2F5");
							}

							// Selecciona una casilla a donde se movera la ficha seleccionada
							if (hit.collider.tag == "Box" && selected != null) {
								movePiece(hit);
							}
					}
					else {
						if (hit.collider.tag == "Box") {
							moveBall(hit);
						}
					}
				}
			}
		}
	}

	// Selecciona o deselecciona una ficha
	void selectDeselectPiece(string tag){
		if (selected == null){
			GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.blue;
			selected = tag;
		} 
		else {	
			if (selected == tag){
				if (Network.peerType == NetworkPeerType.Server)
					GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.white;
				else
					GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.red;
				selected = null;
			}
		}
	}

	// Selecciona la casilla a donde mover la ficha, verifica si es un movimiento valido, y mueve la ficha
	void movePiece(RaycastHit hit){
		// Obtengo la posicion de la ficha
		fichaX = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes> ().x;
		fichaY = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes> ().y;
		
		// Obtengo la posicion destino
		destinoX = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().x;
		destinoY = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().y;
		
		// Verifico si es un movimiento valido
		if (isValid(fichaX, fichaY, destinoX, destinoY)){
			// Cargo nuevos valores en la matriz
			GetComponent<NetworkView>().RPC ("setMatrix", RPCMode.All, fichaX, fichaY, destinoX, destinoY);
			// Muevo la ficha
			GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().x = destinoX; 
			GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().y = destinoY;
			GameObject.FindWithTag(selected).transform.position = hit.collider.transform.position;
			if (Network.peerType == NetworkPeerType.Server)
				GameObject.FindWithTag(selected).GetComponent<Renderer>().material.color = Color.white;
			else
				GameObject.FindWithTag(selected).GetComponent<Renderer>().material.color = Color.red;
			selected = null;
			if (getBall(destinoX, destinoY)){ 
				ballSelected = true;
				//selected = "BALL";
				GameObject.FindWithTag("BALL").GetComponent<Renderer>().material.color = Color.blue;
				marcadores.ShowLabel = false;
			}
			//networkView.RPC ("setTurn", RPCMode.All);
		}
	}

	void moveBall(RaycastHit hit){
		// Obtengo la posicion de la ficha
		ballX = GameObject.FindWithTag("BALL").GetComponent<MatrixAttributes> ().x;
		ballY = GameObject.FindWithTag("BALL").GetComponent<MatrixAttributes> ().y;
		
		// Obtengo la posicion destino
		destinoX = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().x;
		destinoY = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().y;
		
		// Verifico si es un movimiento valido
		if (isValid(ballX, ballY, destinoX, destinoY)){
			// Muevo la pelota, pinto y cargo los valores en la matriz tanto en el servidor como en el cliente
			GetComponent<NetworkView>().RPC ("moveBallOnServerAndClient", RPCMode.All, destinoX, destinoY, hit.collider.transform.position, ballX, ballY);		
			selected = null;
			ballSelected = false;
			// if (ficha alrededor de la pelota){
			//ballSelected = true;	
			//GameObject.FindWithTag("BALL").GetComponent<Renderer>().material.color = Color.blue;
			//}
			//networkView.RPC ("setTurn", RPCMode.All);
		}
	}

	// Verifica si el movimiento a realizar es valido
	bool isValid(int fichaX, int fichaY, int destinoX, int destinoY){
		if ( board[destinoX, destinoY] == '0' ){
			mensajeError("nde gay");
			return true;
		}
		return false;
	}

	// Verifica si la pelota se encuentra alrededor de la ficha
	bool getBall(int x, int y){
		// En caso de que la ficha se encuentre en un limite de la matriz
		if (x == 0) {
			if (board [x, y - 1] == '5' || board [x, y + 1] == '5' || board [x + 1, y + 1] == '5' ||
			    board [x + 1, y] == '5' || board [x + 1, y - 1] == '5' 
			    )
				return true;
		}
		if (x == 14) {
			if (board [x, y - 1] == '5' || board [x - 1, y - 1] == '5' || board [x - 1, y] == '5' || 
			    board [x - 1, y + 1] == '5' || board [x, y + 1] == '5' 
			    )
				return true;		
		}
		if (y == 0) {
			if (board [x - 1, y] == '5' || board [x - 1, y + 1] == '5' || board [x, y + 1] == '5' || 
			    board [x + 1, y + 1] == '5' || board [x + 1, y] == '5' 
			    )
				return true;
		}
		if (y == 10) {
			if (board [x, y - 1] == '5' || board [x - 1, y - 1] == '5' || board [x - 1, y] == '5' || 
			    board [x + 1, y] == '5' || board [x + 1, y - 1] == '5' 
			    )
				return true;
		}
		// En caso de que la ficha no se encuentre en un limite de la matriz
		if (x != 0 && x != 14 && y != 0 && y != 10) {
			if (board [x, y - 1] == '5' || board [x - 1, y - 1] == '5' || board [x - 1, y] == '5' || 
			    board [x - 1, y + 1] == '5' || board [x, y + 1] == '5' || board [x + 1, y + 1] == '5' ||
			    board [x + 1, y] == '5' || board [x + 1, y - 1] == '5' 
			    )
				return true;
		}
		return false; 
	}

	// Carga el nuevo movimiento en la matriz
	[RPC]
	void setMatrix(int fichaX, int fichaY, int destinoX, int destinoY){
		board [destinoX, destinoY] = board [fichaX, fichaY];
		board[fichaX, fichaY] = '0';	
	}

	// Mueve la pelota en el servidor y en el cliente
	/* Esto se hace porque el servidor es el que instancio la pelota y por ende cuando el cliente mueve la pelota
	los cambios no se ven en el servidor*/
	[RPC]
	void moveBallOnServerAndClient(int destX, int destY, Vector3 pos, int origX, int origY){
		GameObject.FindWithTag("BALL").GetComponent<MatrixAttributes>().x = destX; 
		GameObject.FindWithTag("BALL").GetComponent<MatrixAttributes>().y = destY;
		// Muevo la pelota
		GameObject.FindWithTag("BALL").transform.position = pos;
		GameObject.FindWithTag("BALL").GetComponent<Renderer>().material.color = Color.yellow;
		// Actualizo la matriz (hardcoded porque no andaba llamando a setMatrix)
		board [destX, destY] = '5';
		board[origX, origY] = '0';	
	}

	void mensajeError(string mensaje)
	{
		int i=0;
		marcadores.errorText = mensaje;
		marcadores.ShowLabel = true;

			//for ( i= 0; i < 1000000000; i++) {
				
		//}
		StartCoroutine("wait3");
		//if (i == 10000) {
		//marcadores.ShowLabel = false;
		//}

		//marcadores.ShowLabel = false;


			//YieldInstruction StartCoroutine(WaitAndPrint(2.0));
			//YieldInstruction WaitForSeconds(3);
		//	MenuCont.ShowLabel = false;

	}


	/*
	 *   var speedsmooth:float = 0.5f; // your speed smooth, but not time
  private var myAlpha:float = 1.0f;
 
  function Start() {
   myAlpha = 1.0f; // maybe you need other value
  }
 
  function Update() {
   myAlpha = myAlpha - speedsmooth *Time.deltaTime;
   if(myAlpha > 0) {
    this.transform.guitext.color = new Color(1.0f,1.0f,1.0f,myAlpha);
   } else {
    Destroy(this.gameObject);
   }
  }
	 * */

	IEnumerator wait3(){
		yield return new WaitForSeconds (10);
	}



	/*
	// Para manejo de turnos
	[RPC]
	void setTurn ()
	{
		if (turn == 1) {
			turn = 2;
		} else {
			turn = 1;
		}
	}
	*/

}
