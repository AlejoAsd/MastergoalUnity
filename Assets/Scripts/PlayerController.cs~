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

public enum EstadoJuego
{
	Juego,
	Pase,
	Movimiento,
	Reiniciando,
	Fin
}

public enum Equipo
{
	Blanco = -1,
	Ninguno,
	Rojo,
	Ambos
}

public enum TipoFicha
{
	Vacio,
	Pelota,
	BlancoFicha,
	BlancoArquero,
	RojoFicha,
	RojoArquero
}

public class BoardCell
{
	public int x;
	public int y;
	
	public TipoFicha ficha;
	public Equipo equipo;
	public int influenciaBlanco;
	public int influenciaRojo;
	
	public bool area;
	public bool corner;
	public bool arco;
	public bool especial;
	public bool arquero;
	
	public BoardCell(int alto, int ancho, int x, int y)
	{
		this.x = x;
		this.y = y;
		
		influenciaBlanco = 0;
		influenciaRojo = 0;
		
		arquero = false;
		
		// Calcular las propiedades del tablero
		// Definir el equipo
		if (x < (alto / 2))
			equipo = Equipo.Blanco;
		else if (x > (alto / 2))
			equipo = Equipo.Rojo;
		else
			equipo = Equipo.Ninguno;
		
		// Definir si es un corner
		corner = (x == 1 || x == (alto - 2)) && (y == 0 || y == (ancho - 1));
		
		// Definir si es parte del área
		area = ((x >= 1 && x <= 4) || (x >= (alto - 5) && x <= (alto - 2))) && 
			(y >= 1 && y <= (ancho - 2));
		
		// Definir si es un arco
		arco = x == 0 || x == (alto - 1);
		
		// Definir si es una casilla especial
		especial = (x == 1 || x == (alto - 2)) && 
			(y == 0 || y == (ancho - 1) || (y >= 3 && y <= 7));
	}
	
	/*
	 * Indica si la influencia de los equipos se neutraliza en la casilla.
	 * Una casilla sin influencia tambien es considerada neutral.
	 * En caso de querer estar seguro que no hay ningún tipo de influencia en la casilla usar InfluenciaCero().
	 */
	public bool influenciaNeutra()
	{
		return (influenciaRojo - influenciaBlanco) == 0;
	}
	
	/*
	 * Indica si la casilla no esta bajo influencia alguna
	 */
	public bool influenciaCero()
	{
		return influenciaRojo == 0 && influenciaBlanco == 0;
	}
	
	/*
	 * Indica si un equipo tiene posesion de esta casilla.
	 * Parámetros:
	 * Equipo - Equipo sobre el que se valora la influencia
	 * Estricto - Define el nivel de influencia que se debe tener para tener posesion.
	 * Retorna: 
	 * true si hay un empate o mayoria de influencia del equipo en el caso no estricto.
	 * true si hay mayoría de influencia del equipo en el caso estricto.
	 */
	public bool tieneInfluencia(Equipo equipo, bool estricto)
	{
		if (estricto)
		{
			return ((influenciaRojo - influenciaBlanco) * (int)equipo) > 0;
		}
		else
		{
			return ((influenciaRojo - influenciaBlanco) * (int)equipo) >= 0;
		}
	}

	public void modificarInfluencia(TipoFicha ficha, bool inverso)
	{
		int Cantidad = 1 * (inverso ? -1 : 1);
		
		if (ficha == TipoFicha.BlancoArquero || ficha == TipoFicha.RojoArquero)
		{
			Cantidad *= 6;
		}

		if (ficha == TipoFicha.BlancoFicha || ficha == TipoFicha.BlancoArquero)
		{
			influenciaBlanco += Cantidad;
		}
		else if (ficha == TipoFicha.RojoFicha || ficha == TipoFicha.RojoArquero)
		{
			influenciaRojo += Cantidad;
		}
	}
}

public class PlayerController : MonoBehaviour {

	// Dimensiones del tablero
	static int ancho = 11, alto = 15;

	// Matriz que representa el tablero de juego
	public static BoardCell[,] board = new BoardCell[alto, ancho]; 
	
	// Posicion de ficha y posicion destino
	int fichaX, fichaY, ballX, ballY, destinoX, destinoY;

	// Variable que almacena el tag de la ficha seleccionada
	string selected = null;
	public static bool ballSelected = false;

	// Para manejo de turno
	//public static int turn;
	//NetworkView networkView;
	
	void Start(){
		// Cargo la matriz con 0s (celdas vacias)
		for (int i = 0; i < alto; i++)
		{
			for(int j = 0; j < ancho; j++)
			{
				board[i,j] = new BoardCell(alto, ancho, i, j);
				board[i,j].ficha = TipoFicha.Vacio;
			}
		}

		// Cargo la matriz con los valores segun el nivel
		switch (MenuController.level) {
		case 1:
			board[10,5].ficha = TipoFicha.BlancoFicha;
			board[4,5].ficha = TipoFicha.RojoFicha;
			break;
		case 2:
			board[10,5].ficha = TipoFicha.BlancoFicha;
			board[12,5].ficha = TipoFicha.BlancoFicha;
			board[4,5].ficha = TipoFicha.RojoFicha;
			board[2,5].ficha = TipoFicha.RojoFicha;
			break;
		case 3:
			board[8,2].ficha = TipoFicha.BlancoFicha;
			board[8,8].ficha = TipoFicha.BlancoFicha;
			board[10,3].ficha = TipoFicha.BlancoFicha;
			board[10,7].ficha = TipoFicha.BlancoFicha;
			board[12,5].ficha = TipoFicha.BlancoArquero;

			board[6,2].ficha = TipoFicha.RojoFicha;
			board[6,8].ficha = TipoFicha.RojoFicha;
			board[4,3].ficha = TipoFicha.RojoFicha;
			board[4,7].ficha = TipoFicha.RojoFicha;
			board[2,5].ficha = TipoFicha.RojoArquero;
			break;
		}
		board[7,5].ficha = TipoFicha.Pelota;

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
		fichaX = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().x;
		fichaY = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().y;
		
		// Obtengo la posicion destino
		destinoX = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().x;
		destinoY = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().y;
		
		// Verifico si es un movimiento valido
		if (validarMovimiento(fichaX, fichaY, destinoX, destinoY)){
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

			// En caso que se encuentre en una posicion adyacente a la pelota, pasar a modo pase
			if (getBall(destinoX, destinoY)){ 
				ballSelected = true;
				//selected = "BALL";
				GameObject.FindWithTag("BALL").GetComponent<Renderer>().material.color = Color.blue;
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
		if (validarMovimiento(ballX, ballY, destinoX, destinoY)){
			// Muevo la pelota, pinto y cargo los valores en la matriz tanto en el servidor como en el cliente
			GetComponent<NetworkView>().RPC ("moveBallOnServerAndClient", RPCMode.All, destinoX, destinoY, hit.collider.transform.position, ballX, ballY);		
			/*selected = null;
			ballSelected = false;*/
			// if (ficha alrededor de la pelota){
			//ballSelected = true;	
			//GameObject.FindWithTag("BALL").GetComponent<Renderer>().material.color = Color.blue;
			//}
			//networkView.RPC ("setTurn", RPCMode.All);
		}
	}

	// Verifica si el movimiento a realizar es valido
	bool validarMovimiento(int fichaX, int fichaY, int destinoX, int destinoY)
	{
		if ( board[destinoX, destinoY].ficha == TipoFicha.Vacio )
		{
			return true;
		}
		return false;
	}

	// Modifica la influencia de las casillas adyacentes a una posicion
	void modificarInfluencia(int x, int y, bool inverso)
	{
		for (int i = (x - 1); i <= (x + 1); i++)
		{
			for (int j = (y - 1); j <= (y + 1); j++)
			{
				if ((i != x || j != y) &&
				    i > 0 && i < (alto - 2) &&
				    j >= 0 && j < ancho)
				{
					board[i,j].modificarInfluencia(board[x,y].ficha, inverso);
				}
			}
		}
	}

	// Verifica si la pelota se encuentra alrededor de la ficha
	bool getBall(int x, int y){
		// En caso de que la ficha se encuentre en un limite de la matriz
		if (x == 0) {
			if (board [x, y - 1].ficha == TipoFicha.Pelota || board [x, y + 1].ficha == TipoFicha.Pelota || board [x + 1, y + 1].ficha == TipoFicha.Pelota ||
			    board [x + 1, y].ficha == TipoFicha.Pelota || board [x + 1, y - 1].ficha == TipoFicha.Pelota 
			    )
				return true;
		}
		if (x == 14) {
			if (board [x, y - 1].ficha == TipoFicha.Pelota || board [x - 1, y - 1].ficha == TipoFicha.Pelota || board [x - 1, y].ficha == TipoFicha.Pelota || 
			    board [x - 1, y + 1].ficha == TipoFicha.Pelota || board [x, y + 1].ficha == TipoFicha.Pelota 
			    )
				return true;		
		}
		if (y == 0) {
			if (board [x - 1, y].ficha == TipoFicha.Pelota || board [x - 1, y + 1].ficha == TipoFicha.Pelota || board [x, y + 1].ficha == TipoFicha.Pelota || 
			    board [x + 1, y + 1].ficha == TipoFicha.Pelota || board [x + 1, y].ficha == TipoFicha.Pelota 
			    )
				return true;
		}
		if (y == 10) {
			if (board [x, y - 1].ficha == TipoFicha.Pelota || board [x - 1, y - 1].ficha == TipoFicha.Pelota || board [x - 1, y].ficha == TipoFicha.Pelota || 
			    board [x + 1, y].ficha == TipoFicha.Pelota || board [x + 1, y - 1].ficha == TipoFicha.Pelota 
			    )
				return true;
		}
		// En caso de que la ficha no se encuentre en un limite de la matriz
		if (x != 0 && x != 14 && y != 0 && y != 10) {
			if (board [x, y - 1].ficha == TipoFicha.Pelota || board [x - 1, y - 1].ficha == TipoFicha.Pelota || board [x - 1, y].ficha == TipoFicha.Pelota || 
			    board [x - 1, y + 1].ficha == TipoFicha.Pelota || board [x, y + 1].ficha == TipoFicha.Pelota || board [x + 1, y + 1].ficha == TipoFicha.Pelota ||
			    board [x + 1, y].ficha == TipoFicha.Pelota || board [x + 1, y - 1].ficha == TipoFicha.Pelota 
			    )
				return true;
		}
		return false; 
	}

	// Carga el nuevo movimiento en la matriz
	[RPC]
	void setMatrix(int fichaX, int fichaY, int destinoX, int destinoY){
		board[destinoX, destinoY].ficha = board[fichaX, fichaY].ficha;
		board[fichaX, fichaY].ficha = TipoFicha.Vacio;	
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
		// No anda llamando a setMatrix porque esta funcion ya se ejecuta en el servidor
		selected = null;
		ballSelected = false;
		board[destX, destY].ficha = TipoFicha.Pelota;
		board[origX, origY].ficha = TipoFicha.Vacio;	
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
