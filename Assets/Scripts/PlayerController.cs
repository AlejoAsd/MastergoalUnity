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
	int fichaX, fichaY, destinoX, destinoY;
	
	// Variable que almacena el tag de la ficha seleccionada
	string selected = null;
	
	// Para manejo de turno
	//public static int turn;
	//NetworkView networkView;
	
	int BanMovJugador= 0;
	
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
		
		// Para manejo de turno
		//networkView = GetComponent<NetworkView> ();
		//turn = 1;
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
						selectPieceAndMove(hit);
					}
				}
				
				if (Network.peerType == NetworkPeerType.Client /*&& turn == 2*/){
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
						selectPieceAndMove(hit);
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
	void selectPieceAndMove(RaycastHit hit){
		// Obtengo la posicion de la ficha
		fichaX = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes> ().x;
		fichaY = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes> ().y;
		
		// Obtengo la posicion destino
		destinoX = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().x;
		destinoY = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().y;
		///////////////////////aca toque hay que diferenciar entre jugador y pelota/////////////////////
		// Verifico si es un movimiento valido
		if (isValid(fichaX, fichaY, destinoX, destinoY, "jugador")){
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
			//networkView.RPC ("setTurn", RPCMode.All);
		}
	}
	
	
	
	
	// Verifica si el movimiento a realizar es valido
	bool isValid(int fichaX, int fichaY, int destinoX, int destinoY, string tipoFicha){
		
		bool legalMove = false;
		
		// delta de moviemientos para x e y
		int _deltaX = (int)(destinoX - fichaX);
		int _deltaY = (int)(destinoY - fichaY);
		
		// Use the name of the _SelectedPiece Game7Object to find the piece used
		
		switch (tipoFicha) {
		case "jugador": // puede realizar hasta 2 movimientos a la redonda
			if ( board[destinoX, destinoY] != '0' ){
				legalMove= false;
			}else{
				if (Mathf.Abs (_deltaX) < 3) {
					if (Mathf.Abs (_deltaY) < 3) {
						if(illegalMovesPlayer(fichaX, fichaY,  destinoX,  destinoY)){
						legalMove= true;
						
						}
					}
				}
			}
			
			break;
			
		case "pelota":// puede realizar hasta 4 movimientos a la redonda
			if (Mathf.Abs (_deltaX) < 5) {
				if (Mathf.Abs (_deltaY) < 5) {
					if ( board[destinoX, destinoY] == '0' )
						legalMove= true;
				}
			}
			
			
			break;
			
		default:
			legalMove= false;
			break;
		}
		return legalMove;
		//return true;
		/// retorna si es valido o no el movimiento 
		//	return movimientoLegal;
	}
	

	bool illegalMovesPlayer(int fichaX, int fichaY, int destinoX, int destinoY){
		
		
		switch (board [fichaX, fichaY]) {
			
		case '1':// caso de las fichas blancas
			
			// no puede entrar a su arco	
			for (int j=3;j<8;j++){
				int i = 14;
				if (i == destinoX )
					if(j == destinoY)
						return false;
			}
			
			// no puede entrar a su corner
			if ( destinoX== 13 )
				if(destinoY==0 || destinoY==10 )
					return false;
			
			
			break;
			
		case '2':// caso de las fichas rojas
			
			// no puede entrar a su arco	
			for (int j=3;j<8;j++){
				int i = 0;
				if (i == destinoX )
					if(j == destinoY)
						return false;
			}
			
			// no puede entrar a su corner
			if ( destinoX== 1 )
				if(destinoY==0 || destinoY==10 )
					return false;
			
			
			break;
		default:
			return true;
			break;
			
		}
		
		
		return true;
		
	}
	
	bool illegalmovesBall(int fichaX, int fichaY, int destinoX, int destinoY){
		
		// aca hay que tener un identificador para ver de quien es el turno entonces se hacen las reglas de la pelota
		// voy a hacer para cuando tenga el caso de las blancas nomas mientras
		switch (board [fichaX, fichaY]) {
			
		case '1':// caso de las fichas blancas puedan chutar
			
			// no puede chutar a su area a no ser que sea un pase
			// si nro de pases es ==0 entonces

			//if (pases==0){
			 
		//}
			for (int i=10;i<13;i++){
				for (int j=1;j<9;j++){
					if (i == destinoX )
						if(j == destinoY)
							return false;
				}
			}
			
			// no puede chutar a su corner
			if ( destinoX== 13 )
				if(destinoY==0 || destinoY==10 )
					return false;
			

		
			// si estoy en el area grande no puedo pasar encima del arquero
			//si estoy en el area chica no puedo pasar la pelota encima de jugadores y arquero

			if (pelotaChoca( fichaX,  fichaY,  destinoX,  destinoY)){// si la pelota choco contra el jugador o arquero?
				return false;
			}else{// si no choca
				return true;
			}

		case '2':
			break;
		
		default:
			return true;
			break;
			
		}

		return true;
		
	}


	// esta funcion confirma si la pelota en su trayectoria del area chica rebota por un jugador en el area chica o el arquero en el area grande
	bool pelotaChoca(int fichaX, int fichaY, int destinoX, int destinoY){


		int _deltaX = (int)(destinoX - fichaX);
		int _deltaY = (int)(destinoY - fichaY);
		int incX = 0; // recorre en X
		int incY = 0; // recorre en Y
		int i;
		int j;


		// Cacula el incremento no divide entre 0
		if(_deltaX != 0){
			incX = (_deltaX/Mathf.Abs(_deltaX));
		}
		if(_deltaY != 0){
			incY = (_deltaY/Mathf.Abs(_deltaY));
		}

		
		i = fichaX + incX;
		j = fichaY + incY;

		while (i !=destinoX && j !=destinoY) {



		}


		return false;
	//for (int i=12;i<13;i++){
	//	for (int j=2;j<8;j++){// estoy en el area chica
			
			
	//		if (i == destinoX )
	//			if(j == destinoY)
	//				return false;
		//}
	//}
}




	// Carga el nuevo movimiento en la matriz
	[RPC]
	void setMatrix(int fichaX, int fichaY, int destinoX, int destinoY){
		board[fichaX, fichaY] = '0';
		if (Network.peerType == NetworkPeerType.Server) {
			board[destinoX, destinoY] = '1';
		}
		
		if (Network.peerType == NetworkPeerType.Client) {
			board[destinoX, destinoY] = '2';
		}
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

/* para que no pueda entrar en el area en la primera jugada
	bool firstMovPlayer( int destinoX, int destinoY){
		//bool movimientoLegal = false;

		if (destinoX == 6 && (destinoX >= 4 && destinoY <= 6)) 
			{
				return true;
			}
			else 
			{
				return false;
			}
		
	}


	
	//if (BanMovJugador==0){// primera jugada
	//	el jugador no puede entrar dentro de su area en la primera jugada
	
	//	if (firstMovPlayer(destinoX,destinoY)){
	//		BanMovJugador++;		
	//		return true;
	//	}else{// si entra al area es movimiento invalido
	//	return false;
	//	}
	//	}else{// no es la primera juagda

*/
