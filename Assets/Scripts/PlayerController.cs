using UnityEngine;
using System.Collections;

public enum EstadoJuego
{
	Iniciando,
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
	public bool areaChica;
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

		ficha = TipoFicha.Vacio;
        
		/// Calcular las propiedades del tablero
		// Definir el equipo
		if (x > (alto / 2))
		{
			equipo = Equipo.Blanco;
		}
		else if (x < (alto / 2))
		{
			equipo = Equipo.Rojo;
		}
		else
		{
			equipo = Equipo.Ninguno;
		}
        
		// Definir si es un corner
		corner = (x == 1 || x == (alto - 2)) && (y == 0 || y == (ancho - 1));
        
		// Definir si es parte del área
		area = ((x >= 1 && x <= 4) || (x >= (alto - 5) && x <= (alto - 2))) && 
			(y >= 1 && y <= (ancho - 2));
		areaChica = ((x >= 1 && x <= 2) || (x >= (alto - 3) && x <= (alto - 2))) && 
			(y >= 2 && y <= (ancho - 3));
        
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

	public void modificarInfluencia(TipoFicha ficha, bool negativo)
	{
		int Cantidad = 1 * (negativo ? -1 : 1);
        
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

	public bool esArquero(bool ChequearArea)
	{
		if (ficha != TipoFicha.BlancoArquero && ficha != TipoFicha.RojoArquero)
		{
			return false;
		}
        
		if (ChequearArea && !area)
		{
			return false;
		}
        
		return true;
	}

	public Equipo fichaEquipo()
	{
		if (ficha == TipoFicha.BlancoFicha || ficha == TipoFicha.BlancoArquero)
		{
			return Equipo.Blanco;
		}
		else if (ficha == TipoFicha.RojoFicha || ficha == TipoFicha.RojoArquero)
		{
			return Equipo.Rojo;
		}
		else
		{
			return Equipo.Ninguno;
		}
	}
}

public class PlayerController : MonoBehaviour
{
	// Constantes de identificacion
	public const string ID_Pelota = "BALL";
	public const string ID_Box = "Box";
	public const string ID_P1F1 = "P1F1";
	public const string ID_P1F2 = "P1F2";
	public const string ID_P1F3 = "P1F3";
	public const string ID_P1F4 = "P1F4";
	public const string ID_P1F5 = "P1F5";
	public const string ID_P2F1 = "P2F1";
	public const string ID_P2F2 = "P2F2";
	public const string ID_P2F3 = "P2F3";
	public const string ID_P2F4 = "P2F4";
	public const string ID_P2F5 = "P2F5";

	// Dimensiones del tablero
	static int ancho = 11;
	static int alto = 15;
	static int anchoArco = 5;

	// Matriz que representa el tablero de juego
	public static BoardCell[,] board = new BoardCell[alto, ancho];

	// Informacion sobre el turno
	public static Equipo turno = Equipo.Blanco;
	static int contadorTurnos = 0;
	static int pases = 0;
	static int pasesMaximos = 4;

	// Estado del juego
	//HACK public static EstadoJuego estado = EstadoJuego.Iniciando;
	public static EstadoJuego estado = EstadoJuego.Juego;

	// Tag de la ficha seleccionada
	private string selected = null;

	// Marcadores del juego
	public static int marcador1, marcador2;

	// Indica el fin del juego
	public static bool end = false;
    
	void Start()
	{
		marcador1 = 0;
		marcador2 = 0;
		turno = Equipo.Blanco;
		initializeMatrix();
	}

	// Inicializa la matriz con los valores segun las posiciones de las fichas
	void initializeMatrix()
	{
		// Cargar la matriz con celdas vacias
		for (int i = 0; i < alto; i++)
		{
			for (int j = 0; j < ancho; j++)
			{
				board[i, j] = new BoardCell(alto, ancho, i, j);
			}
		}
		
		// Cargar la matriz con los valores segun el nivel
		switch (MenuController.level)
		{
			case 1:
				setFicha(10, 5, TipoFicha.BlancoFicha);
				setFicha(4, 5, TipoFicha.RojoFicha);
				break;
			case 2:
				setFicha(10, 5, TipoFicha.BlancoFicha);
				setFicha(12, 5, TipoFicha.BlancoFicha);
				setFicha(4, 5, TipoFicha.RojoFicha);
				setFicha(2, 5, TipoFicha.RojoFicha);
				break;
			case 3:
				setFicha(8, 2, TipoFicha.BlancoFicha);
				setFicha(8, 8, TipoFicha.BlancoFicha);
				setFicha(10, 3, TipoFicha.BlancoFicha);
				setFicha(10, 7, TipoFicha.BlancoFicha);
				setFicha(12, 5, TipoFicha.BlancoArquero);
			
				setFicha(6, 2, TipoFicha.RojoFicha);
				setFicha(6, 8, TipoFicha.RojoFicha);
				setFicha(4, 3, TipoFicha.RojoFicha);
				setFicha(4, 7, TipoFicha.RojoFicha);
				setFicha(2, 5, TipoFicha.RojoArquero);
				break;
		}
		setFicha(7, 5, TipoFicha.Pelota);
	}

	void Update()
	{   
		if (MenuController.screenValue == Constants.GAMESP && this.tag != ID_P1F1)
		{
			return;
		}

		if (MenuController.screenValue == Constants.GAMESP || (MenuController.screenValue == Constants.GAMEMP && GetComponent<NetworkView>().isMine))
		{
			InputMovement();
		}

		if (MenuController.screenValue == Constants.GAMEMP && Network.peerType == NetworkPeerType.Disconnected)
		{
			Network.Destroy(GetComponent<NetworkView>().viewID);
		}

		/* HACK // No permitir jugar hasta que hayan dos jugadores
		if (Network.isServer && Network.connections.Length == Network.maxConnections && estado == EstadoJuego.Iniciando)
		{
			setEstado(EstadoJuego.Juego);
		}*/
	}

	void OnGUI()
	{
		if (end)
		{
			// Boton Volver al Menu Principal
			if (GUI.Button(new Rect(Screen.width / 4, Screen.height / 3 + Screen.width / 6 + 60, Screen.width / 2, Screen.width / 6), "Volver al Menu Principal"))
			{
				// si es cliente solo cambiar screenValue
				// si es servidor, cambiar screenValue
				// luego de que el cliente haya cambiado el screenValue desconectar (atender que al desconectar tanto cliente como servidor van al main)
				
				// Mientras tanto si uno aprieta el boton los dos vuelven (cliente servidor)
				GetComponent<NetworkView>().RPC("setEnd", RPCMode.All, false);
				MenuController.screenValue = Constants.MAIN; 
				Network.Disconnect();
				MasterServer.UnregisterHost();
				
			}
		}
	}

	void InputMovement()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				// Verificar si se esta intentando seleccionar una ficha
				if (estado == EstadoJuego.Juego)
				{
					for (int i = 1; i <= 5; i++)
					{
						string id;
						if ((MenuController.screenValue == Constants.GAMESP || 
						    (MenuController.screenValue == Constants.GAMEMP && Network.isServer)) && 
						    hit.collider.tag == (id = "P1F" + i))
						{
							selectDeselectPiece(id);
							return;
						}
						else if ((MenuController.screenValue == Constants.GAMESP || 
						         (MenuController.screenValue == Constants.GAMEMP && Network.isClient)) && 
						         hit.collider.tag == (id = "P2F" + i))
						{
							selectDeselectPiece(id);
							return;
						}
					}
				}
				// Verificar si se esta intentando mover la ficha seleccionada
				if (hit.collider.tag == ID_Box && 
				    selected != null &&
				    (estado == EstadoJuego.Juego ||
				 	 estado == EstadoJuego.Pase))
				{
					// Evaluar que haya terminado el juego si la pelota es una ficha
					bool evaluar = (selected == ID_Pelota);

					moverFicha(hit);
					
					if (evaluar)
					{
						evaluarFin();
                    }
                }
			}	
		}	
	}

	// Selecciona o deselecciona una ficha
	void selectDeselectPiece(string tag)
	{
		if (selected == null)
		{
			GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.blue;
			selected = tag;
		}
		else if (selected == tag)
		{
			if (tag == ID_Pelota)
			{
				GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.yellow;
			}
			else if ((MenuController.screenValue == Constants.GAMEMP && Network.isServer) ||
			         (MenuController.screenValue == Constants.GAMESP && turno == Equipo.Blanco))
			{
				GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.white;
			}
			else if ((MenuController.screenValue == Constants.GAMEMP && Network.isClient) ||
			         (MenuController.screenValue == Constants.GAMESP && turno == Equipo.Rojo))
			{
				GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.red;
			}
			selected = null;
		}
	}

	// Selecciona la casilla a donde mover la ficha, verifica si es un movimiento valido, y mueve la ficha
	void moverFicha(RaycastHit hit)
	{
		// Obtengo la posicion de la ficha
		int fichaX = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().x;
		int fichaY = GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().y;
        
		// Obtengo la posicion destino
		int destinoX = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().x;
		int destinoY = GameObject.Find(hit.collider.name).GetComponent<MatrixAttributes>().y;
        
		// Verifico si es un movimiento valido
		if (validarMovimiento(fichaX, fichaY, destinoX, destinoY))
		{
			// Actualizar los valores de la matriz
			if (MenuController.screenValue == Constants.GAMEMP)
			{
				GetComponent<NetworkView>().RPC("setMatrix", RPCMode.All, fichaX, fichaY, destinoX, destinoY);
			}
			else if (MenuController.screenValue == Constants.GAMESP)
			{
				setMatrix(fichaX, fichaY, destinoX, destinoY);
			}

			if (selected == ID_Pelota)
			{
				if (MenuController.screenValue == Constants.GAMEMP)
				{
					GetComponent<NetworkView>().RPC("moverPelotaEnServidorYCliente", RPCMode.All, destinoX, destinoY, hit.collider.transform.position);
				}
				else if (MenuController.screenValue == Constants.GAMESP)
				{
					moverPelotaEnServidorYCliente(destinoX, destinoY, hit.collider.transform.position);
				}
			}
			else
			{
				// Mover la ficha
				GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().x = destinoX; 
				GameObject.FindWithTag(selected).GetComponent<MatrixAttributes>().y = destinoY;
				GameObject.FindWithTag(selected).transform.position = hit.collider.transform.position;
			}

			// Deseleccionar la ficha
			selectDeselectPiece(selected);

			// Actualizar la posicion de la pelota si se esta moviendo la pelota
			BoardCell pelota = obtenerPelotaAdyacente(board[destinoX, destinoY]);
			if (pelota != null && pelota.tieneInfluencia(turno, true))
			{
				estado = EstadoJuego.Pase;
				selectDeselectPiece(ID_Pelota);
                Debug.Log("Pase");
			}
			else
			{
				estado = EstadoJuego.Juego;

                if (selected == ID_Pelota)
                {
                    GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.yellow;
                }
                /*else if (turno == Equipo.Blanco)
                {
                    GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.white;
                }
                else if (turno == Equipo.Rojo)
                {
                    GameObject.FindWithTag(tag).GetComponent<Renderer>().material.color = Color.red;
                }*/
                selected = null;

				if (MenuController.screenValue == Constants.GAMEMP)
				{
					GetComponent<NetworkView>().RPC("cambiarTurno", RPCMode.All);
				}
				else if (MenuController.screenValue == Constants.GAMESP)
				{
					cambiarTurno();
				}
			}
		}
		Debug.Log("Tablero");
		imprimirTablero();
		Debug.Log("Influencia");
		imprimirInfluencia();
	}

	[RPC]
	void moverPelotaEnServidorYCliente(int destX, int destY, Vector3 pos){
		GameObject.FindWithTag(ID_Pelota).GetComponent<MatrixAttributes>().x = destX; 
		GameObject.FindWithTag(ID_Pelota).GetComponent<MatrixAttributes>().y = destY;
		GameObject.FindWithTag(ID_Pelota).transform.position = pos;
	}

	// Verifica si el movimiento a realizar es valido
	bool validarMovimiento(int fichaX, int fichaY, int destinoX, int destinoY)
	{
		int arcoOffset = (ancho - anchoArco) / 2;
		string mensaje = string.Empty;
		BoardCell ficha = board[fichaX, fichaY];
		BoardCell destino = board[destinoX, destinoY];
        
		/// Validar destino
		// Asegurar que esté dentro del tablero. Los tableros cuentan como fuera del arco excepto en el caso de la pelota.
		if (destinoX < 0 || destinoX >= alto ||
			destinoY < 0 || destinoY >= ancho)
		{
			mensaje = "Casilla invalida";
			mensajeError(mensaje);
			return false;
		}
        
		// Asegurar que exista una casilla para la posición indicada
		if (ficha.ficha != TipoFicha.Pelota && 
			(destinoX == 0 || destinoX == (alto - 1)) &&
			(destinoY >= arcoOffset || destinoY <= arcoOffset + anchoArco))
		{
			mensaje = "Los jugadores \n no pueden entrar al arco";
			mensajeError(mensaje);
			return false;
		}
        
		// Asegurar que no sea un corner del equipo
		if ((ficha.ficha == TipoFicha.Pelota || ficha.fichaEquipo() == turno) && turno == destino.equipo &&
			destino.corner)
		{
			mensaje = "No se puede mover a un corner propio";
			mensajeError(mensaje);
			return false;
		}
        
		// Asegurar que al mover la pelota la casilla pertenezca al equipo de turno
		if (ficha.ficha == TipoFicha.Pelota && !destino.tieneInfluencia(turno, false))
		{
			mensaje = "El balon no puede terminar \nen posesion del oponente";
			mensajeError(mensaje);
			return false;
		}
        
		// Asegurar que la pelota termine en una casilla neutra y fuera del area del jugador de turno
		if (ficha.ficha == TipoFicha.Pelota &&
			pases == pasesMaximos)
		{
			// Casilla neutra
			if (!destino.influenciaNeutra())
			{
				mensaje = "Solo queda un pase disponible. La pelota \ndebe quedar en una casilla neutra";
				mensajeError(mensaje);
				return false;
			}
		}
        
		// Asegurar que la pelota no termine del lado del jugador que saca en el primer turno
		if (contadorTurnos == 1 &&
			pases == pasesMaximos &&
			turno == destino.equipo)
		{
			mensaje = "La pelota no puede terminar\n del lado del equipo que empieza";
			mensajeError(mensaje);
			return false;
		}
        
		/// Validar movimiento
		int deltaDestinoX = System.Math.Abs(destinoX - ficha.x);
		int deltaDestinoY = System.Math.Abs(destinoY - ficha.y);
		int maximoMovimientos = ficha.ficha == TipoFicha.Pelota ? 4 : 2;
        
		// El movimiento es en línea recta
		if ((deltaDestinoX == 0 && deltaDestinoY == 0) ||
			(deltaDestinoX != 0 && deltaDestinoY != 0 &&
			deltaDestinoX != deltaDestinoY))
		{
			mensaje = "El movimiento debe ser recto";
			mensajeError(mensaje);
			return false;
		}
        
		// El movimiento está dentro del rango de la ficha
		if (deltaDestinoX > maximoMovimientos ||
			deltaDestinoY > maximoMovimientos)
		{
			mensaje = "Mover hasta dos casillas";
			mensajeError(mensaje);
			return false;
		}
        
		// La casilla objetivo está ocupada
		if (destino.ficha != TipoFicha.Vacio)
		{
			mensaje = "Se debe mover a una casilla libre";
			mensajeError(mensaje);
			return false;
		}
		// La casilla objetivo (y las adyacentes en caso de haber un arquero) está libre
		if (ficha.ficha != TipoFicha.Pelota)
		{
			for (int i = -1; i <= 1; i+=2)
			{
				if (destinoY + i >= 0 && destinoY + i < ancho &&
					board[destinoX, destinoY + i] != ficha &&
					board[destinoX, destinoY + i].ficha != TipoFicha.Vacio &&
					board[destinoX, destinoY + i].ficha != TipoFicha.Pelota)
				{
					if (ficha.esArquero(false))
					{
						mensaje = "Una casilla adyacente no se encuentra libre";
					}
					else if (board[destinoX, destinoY + i].esArquero(false))
					{
						mensaje = "No se puede terminar en una casilla\n adyacente al arquero";
					}
					if (mensaje != string.Empty)
					{
						mensajeError(mensaje);
						return false;
					}
				}
			}
		}
        
		// Asegurar que no sea un autopase
		if (ficha.ficha == TipoFicha.Pelota)
		{
			BoardCell fichaPase = obtenerFichaPase(fichaX, fichaY);
			BoardCell fichaReceptora = obtenerFichaPase(destinoX, destinoY);
            
			if (fichaPase != null && fichaPase == fichaReceptora)
			{
				mensaje = "No se puede hacer un autopase";
				mensajeError(mensaje);
				return false;
			}
		}
        
		// Asegurar que el movimiento del jugador no rompa el balance de influencia
		if (ficha.ficha != TipoFicha.Pelota)
		{
			BoardCell pelota = obtenerPelotaAdyacente(ficha);
			if (pelota != null)
			{
				int deltaX = System.Math.Abs(pelota.x - ficha.x);
				int deltaY = System.Math.Abs(pelota.y - ficha.y);
                
				if (deltaX > 1 || deltaY > 1)
				{
					mensaje = "No se puede perder la neutralidad \nde la pelota";
					mensajeError(mensaje);
					return false;
				}
			}
		}

		// Asegurar que la pelota no termine en el area del jugador de turno
		if (ficha.ficha == TipoFicha.Pelota &&
			destino.area &&
			destino.equipo == turno &&
			!destino.tieneInfluencia(turno, true))
		{
			mensaje = "La pelota debe quedar fuera del area";
			mensajeError(mensaje);
			return false;
		}
        
		/// Determinar dirección
		int direccionDestinoX = 0;
		int direccionDestinoY = 0;
        
		// destinoX
		if (ficha.x < destinoX)
		{
			direccionDestinoX = 1;
		}
		else if (ficha.x > destinoX)
		{
			direccionDestinoX = -1;
		}
        
		// destinoY
		if (ficha.y < destinoY)
		{
			direccionDestinoY = 1;
		}
		else if (ficha.y > destinoY)
		{
			direccionDestinoY = -1;
		}
        
		destinoX = ficha.x;
		destinoY = ficha.y;
        
		int cantidadMovimientos = deltaDestinoX > 0 ? deltaDestinoX : deltaDestinoY;
		for (int i = 1; i < cantidadMovimientos; i++)
		{
			destinoX += direccionDestinoX;
			destinoY += direccionDestinoY;

			// En caso de ser la pelota debe saltar fichas excepto si es un arquero en el area
			if (ficha.ficha == TipoFicha.Pelota &&
				((destinoY - 1 >= 0 &&
				board[destinoX, destinoY - 1].fichaEquipo() != turno && 
				board[destinoX, destinoY - 1].esArquero(true) &&
				board[destinoX, destinoY - 1].area) 
                ||
				(board[destinoX, destinoY].fichaEquipo() != turno && 
				(board[destinoX, destinoY].esArquero(true) || 
				(board[destinoX, destinoY].areaChica &&
				board[destinoX, destinoY].ficha != TipoFicha.Vacio)))
                ||
				(destinoY + 1 < ancho &&
				board[destinoX, destinoY + 1].fichaEquipo() != turno && 
				board[destinoX, destinoY + 1].esArquero(true) &&
				board[destinoX, destinoY + 1].area)))
			{
				mensaje = "La pelota no puede pasar jugadores en el area";
				mensajeError(mensaje);
				return false;
			}
			else if (board[destinoX, destinoY].ficha != TipoFicha.Vacio &&
				ficha.ficha != TipoFicha.Pelota && 
				ficha.ficha != TipoFicha.Vacio && 
				board[destinoX, destinoY].ficha != TipoFicha.Vacio)
			{
				mensaje = "No se puede atravesar a otros jugadores";
				mensajeError(mensaje);
				return false;
			}
		}

		return true;
	}


	void imprimirInfluencia()
	{
		string influencia = string.Empty;
		for (int i = 1; i < alto - 1; i++)
		{
			for (int j = 0; j < ancho; j++)
			{
				influencia += System.Math.Abs(board[i, j].influenciaRojo - board[i, j].influenciaBlanco).ToString();
			}
			influencia += "\n";
		}
		Debug.Log(influencia);
	}

	void imprimirTablero()
	{
		string influencia = string.Empty;
		for (int i = 1; i < alto - 1; i++)
		{
			for (int j = 0; j < ancho; j++)
			{
				influencia += (int)board[i, j].ficha;
			}
			influencia += "\n";
		}
		Debug.Log(influencia);
	}

	// Encuentra la ficha adyacente a la pelota. En caso de haber mas de un jugador retorna null.
	BoardCell obtenerFichaPase(int x, int y)
	{
		BoardCell ficha = null;
        
		for (int i = (x - 1); i <= (x + 1); i++)
		{
			for (int j = (y - 1); j <= (y + 1); j++)
			{
				if ((i != x || j != y) &&
					i > 0 && i < (alto - 1) &&
					j >= 0 && j < ancho &&
					board[i, j].fichaEquipo() == turno)
				{
					if (ficha == null)
					{
						ficha = board[i, j];
					}
					else
					{
						ficha = null;
						return ficha;
					}
				}
			}
		}
        
		return ficha;
	}

	BoardCell obtenerPelotaAdyacente(BoardCell ficha)
	{
		for (int i = (ficha.x - 1); i <= (ficha.x + 1); i++)
		{
			for (int j = (ficha.y - 1); j <= (ficha.y + 1); j++)
			{
				if (i > 0 && i < (alto - 1) &&
					j >= 0 && j < ancho &&
					board[i, j].ficha == TipoFicha.Pelota)
				{
					return board[i, j];
				}
			}
		}
		return null;
	}

	// Modifica la influencia de las casillas adyacentes a una posicion
	void modificarInfluencia(int x, int y, TipoFicha ficha, bool negativo)
	{
		for (int i = (x - 1); i <= (x + 1); i++)
		{
			for (int j = (y - 1); j <= (y + 1); j++)
			{
				if (i > 0 && i < (alto - 1) &&
					j >= 0 && j < ancho)
				{
					board[i, j].modificarInfluencia(ficha, negativo);
				}
			}
		}
	}

	void setFicha(int x, int y, TipoFicha ficha)
	{
		TipoFicha fichaPrevia = board[x, y].ficha;
		board[x, y].ficha = ficha;
		if (ficha == TipoFicha.Vacio)
		{
			modificarInfluencia(x, y, fichaPrevia, true);
		}
		else
		{
			modificarInfluencia(x, y, ficha, false);
		}
	}

	// Carga el nuevo movimiento en la matriz
	[RPC]
	void setMatrix(int fichaX, int fichaY, int destinoX, int destinoY)
	{
		setFicha(destinoX, destinoY, board[fichaX, fichaY].ficha);
		setFicha(fichaX, fichaY, TipoFicha.Vacio);
	}

	// Actualiza el marcador en el cliente y servidor
	[RPC]
	void refreshScore(int player)
	{
		if (player == 1)
		{
			marcador1 = marcador1 + 1;
			marcadores.puntajeBlanco=marcador1;
		}
		else
		{
			marcador2 = marcador2 + 1;
			marcadores.puntajeRojo=marcador2;
		}
	}

	void mensajeError(string mensaje)
	{
		Debug.Log(mensaje);
		int i=0;
		marcadores.errorText = mensaje;
		marcadores.ShowLabel = true;
		marcadores.contadorErrorFloat = 4;
	}


	// Verifica si se metio un gol y aumenta el marcador
	bool isGoal()
	{
		for (int j=3; j<=7; j++)
		{
			if (board[0, j].ficha == TipoFicha.Pelota)
			{
				// Aumento el marcador del jugador 1
				if (MenuController.screenValue == Constants.GAMEMP)
				{
					GetComponent<NetworkView>().RPC("refreshScore", RPCMode.All, 1);
				}
				else if (MenuController.screenValue == Constants.GAMESP)
				{
					refreshScore(1);
				}
				return true;
			}
			if (board[14, j].ficha == TipoFicha.Pelota)
			{
				// Aumento el marcador del jugador 2
				if (MenuController.screenValue == Constants.GAMEMP)
				{
					GetComponent<NetworkView>().RPC("refreshScore", RPCMode.All, 2);
				}
				else if (MenuController.screenValue == Constants.GAMESP)
				{
					refreshScore(2);
				}
				return true;
			}
		}
		return false;
	}
	
	// Coloca las fichas a sus posiciones iniciales
	[RPC]
	void restartPieces()
	{
		// Inicializo la matriz
		initializeMatrix();
		
		// Muevo las fichas
		restartPiece(ID_Pelota, 7, 5, "75");
		switch (MenuController.level)
		{
			case 1:
				restartPiece("P1F1", 10, 5, "105");	
				
				restartPiece("P2F1", 4, 5, "45");
				break;
			case 2:
				restartPiece("P1F1", 10, 5, "105");
				restartPiece("P1F2", 12, 5, "125");
				
				restartPiece("P2F1", 4, 5, "45");
				restartPiece("P2F2", 2, 5, "25");
				break;
			case 3:
				restartPiece("P1F1", 8, 2, "82");
				restartPiece("P1F2", 8, 8, "88");
				restartPiece("P1F3", 10, 3, "103");
				restartPiece("P1F4", 10, 7, "107");
				restartPiece("P1F5", 12, 5, "125");
				
				restartPiece("P2F1", 6, 2, "62");
				restartPiece("P2F2", 6, 8, "68");
				restartPiece("P2F3", 4, 3, "43");
				restartPiece("P2F4", 4, 7, "47");
				restartPiece("P2F5", 2, 5, "25");
				break;
		}
	}
	
	// Mueve una pieza a su posicion inicial
	void restartPiece(string tag, int x, int y, string destiny)
	{
		GameObject.FindWithTag(tag).GetComponent<MatrixAttributes>().x = x; 
		GameObject.FindWithTag(tag).GetComponent<MatrixAttributes>().y = y;
		GameObject.FindWithTag(tag).transform.position = GameObject.Find(destiny).transform.position;
	}
	
	// Termina el juego en caso de que hayan entrado dos goles, reinicia las fichas en caso de un gol
	void evaluarFin()
	{
		if (isGoal())
		{
			if (marcador1 + marcador2 == 2)
			{
				if (MenuController.screenValue == Constants.GAMEMP)
				{
					GetComponent<NetworkView>().RPC("setEnd", RPCMode.All, true);
				}
				else if (MenuController.screenValue == Constants.GAMESP)
				{
					setEnd(true);
				}
			}
			else
			{
				if (MenuController.screenValue == Constants.GAMEMP)
				{
					GetComponent<NetworkView>().RPC("restartPieces", RPCMode.All);
				}
				else if (MenuController.screenValue == Constants.GAMESP)
				{
					restartPieces();
				}
			}
			
		}
	}

	[RPC]
	void setEstado(EstadoJuego nuevoEstado)
	{
		estado = nuevoEstado;
	}

	[RPC]
	void setEnd(bool value)
	{
		end = value;
	}

	// Para manejo de turnos
	[RPC]
	void cambiarTurno()
	{
		if (turno == Equipo.Blanco)
		{
			turno = Equipo.Rojo;
			marcadores.turnoText= "Roja";
			marcadores.contador = 45;
		}
		else if (turno == Equipo.Rojo)
		{
			turno = Equipo.Blanco;
			marcadores.turnoText= "Blanca";
			marcadores.contador = 45;
		}
	}
}
