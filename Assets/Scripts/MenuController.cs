using UnityEngine;
using System.Collections;

static class Constants
{
	public const int MAIN = 0;
	public const int SERVERLIST = 1;
	public const int LEVELSELECTIONSP = 2;
	public const int LEVELSELECTIONMP = 3;
	public const int GAMESP = 4;
	public const int GAMEMP = 5;
	public const int TUTORIAL = 9;
	
}

public class marcadores
{
	
	public static string errorText = "Error";// solo reemplazar esto para tener los mensajes
	public static string turnoText = "Blanca";
	public static int puntajeRojo = 0;
	public static int puntajeBlanco = 0;
	public static bool ShowLabel = false;
	public static int contador = 45;
	public static float contFloat = 45;
	public static float contadorErrorFloat = 3;
	public static int contadorError = 4;
	
}

public class MenuController : MonoBehaviour
{

	/**************************** Variables ****************************/

	// Variables globales
	public static int level;
	public static int screenValue = Constants.MAIN;

	// Variables para manejo de la red
	private const string typeName = "guidonuGame";
	private string gameName;
	private HostData[] hostList;
	public GameObject[] playerPrefab1 = new GameObject[5];
	public GameObject[] playerPrefab2 = new GameObject[5];
	public GameObject ballPrefab;
	private MatrixAttributes matrixAttributes;
	Vector3 spawningPosition;

	// Variable para ScrollView
	public Vector2 scrollPosition = Vector2.zero;
	private string textoScrollView = "No hay partidas disponibles";

	// Variables auxiliares
	private int X_INICIAL = (int)(Screen.width / 6);
	private int Y_INICIAL = Screen.height / 3;
	private int BTN_WIDTH = (int)(Screen.width / 1.5);
	private int BTN_HEIGHT = Screen.width / 6;

	// Estilos
	public GUIStyle customStyle;
	public GUIStyle customButton;
	public GUIStyle customBox;
	public GUIStyle marcadorStyle;

	/**************************** Funciones de red ****************************/ 
	//Inicializar servidor
	private void StartServer()
	{
		Network.InitializeServer(1, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	//Ver lista de servidores
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList();
		}
	}
	
	//Unirse a un servidor
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		ServerSpawnPlayer1();
		ServerSpawnBall();
	}

	void OnConnectedToServer()
	{
		Debug.Log("Joined Server");
		ServerSpawnPlayer2();
		GameObject.FindWithTag("P2F1").AddComponent<PlayerController>();
	}

	void OnDisconnectedFromServer()
	{
		destruirFichas();
		screenValue = Constants.MAIN;
	}

	void OnPlayerDisconnected()
	{
		Network.Disconnect();
		MasterServer.UnregisterHost();
		destruirFichas();
		screenValue = Constants.MAIN;
	}

	//Generar nombre de juego
	string generateRoomName(int level)
	{
		int number = Random.Range(0, 100);
		return "Juegol" + number + "-Nivel" + level;
	}

	public void update()
	{
		marcadores.contFloat -= Time.deltaTime;
		marcadores.contador = (int)marcadores.contFloat;
		
		// esto es para que desaparezca el mensaje de error en cierto tiempo
		marcadores.contadorErrorFloat -= Time.deltaTime;
		marcadores.contadorError = (int)marcadores.contadorErrorFloat;
		
		if (marcadores.contadorError <= 0)// o fin de turno
		{
			marcadores.ShowLabel = false;
		}
		
		if (marcadores.contador == 0)// o fin de turno
		{
			marcadores.contador = 45;
		}
	}

	void InitializeSP()
	{
		SpawnPlayer1();
		SpawnPlayer2();
		SpawnBall();
	}

	#region Spawns Singleplayer
	// Genera las fichas del jugador 1 (server)
	private void SpawnPlayer1()
	{		
		// Genero las fichas segun el nivel seleccionado
		switch (level)
		{
			case 1:
				SpawnPlayer1Piece(10, 5, "105", 0);
				break;
			case 2:
				SpawnPlayer1Piece(10, 5, "105", 0);
				SpawnPlayer1Piece(12, 5, "125", 1);
				break;
			case 3:
				SpawnPlayer1Piece(8, 2, "82", 0);
				SpawnPlayer1Piece(8, 8, "88", 1);
				SpawnPlayer1Piece(10, 3, "103", 2);
				SpawnPlayer1Piece(10, 7, "107", 3);
				SpawnPlayer1Piece(12, 5, "125", 4);
				break;
		}
	}
	
	// Genera las fichas del jugador 2 (cliente)
	private void SpawnPlayer2()
	{
		// Genero las fichas segun el nivel seleccionado
		switch (level)
		{
			case 1:
				SpawnPlayer2Piece(4, 5, "45", 0);
				break;
			case 2:
				SpawnPlayer2Piece(4, 5, "45", 0);
				SpawnPlayer2Piece(2, 5, "25", 1);
				break;
			case 3:
				SpawnPlayer2Piece(6, 2, "62", 0);
				SpawnPlayer2Piece(6, 8, "68", 1);
				SpawnPlayer2Piece(4, 3, "43", 2);
				SpawnPlayer2Piece(4, 7, "47", 3);
				SpawnPlayer2Piece(2, 5, "25", 4);
				break;
		}
	}
	
	// Genera la pelota
	private void SpawnBall()
	{
		matrixAttributes = ballPrefab.GetComponent<MatrixAttributes>();
		matrixAttributes.x = 7;
		matrixAttributes.y = 5;
		spawningPosition = GameObject.Find("75").transform.position;
		Instantiate(ballPrefab, spawningPosition, ballPrefab.transform.rotation);
	}
	
	// Genera una ficha del jugador 1
	void SpawnPlayer1Piece(int x, int y, string pieceName, int pieceNumber)
	{
		matrixAttributes = playerPrefab1[pieceNumber].GetComponent<MatrixAttributes>();
		matrixAttributes.x = x;
		matrixAttributes.y = y;
		spawningPosition = GameObject.Find(pieceName).transform.position;
		Instantiate(playerPrefab1[pieceNumber], spawningPosition, playerPrefab1[pieceNumber].transform.rotation);
	}
	
	// Genera una ficha del jugador 2
	void SpawnPlayer2Piece(int x, int y, string pieceName, int pieceNumber)
	{
		matrixAttributes = playerPrefab2[pieceNumber].GetComponent<MatrixAttributes>();
		matrixAttributes.x = x;
		matrixAttributes.y = y;
		spawningPosition = GameObject.Find(pieceName).transform.position;
		Instantiate(playerPrefab2[pieceNumber], spawningPosition, playerPrefab2[pieceNumber].transform.rotation);
	}
	#endregion Spawn Singleplayer

	#region Spawns Multiplayer
	// Genera las fichas del jugador 1 (server)
	private void ServerSpawnPlayer1()
	{		
		// Genero las fichas segun el nivel seleccionado
		switch (level)
		{
			case 1:
				ServerSpawnPlayer1Piece(10, 5, "105", 0);
				break;
			case 2:
				ServerSpawnPlayer1Piece(10, 5, "105", 0);
				ServerSpawnPlayer1Piece(12, 5, "125", 1);
				break;
			case 3:
				ServerSpawnPlayer1Piece(8, 2, "82", 0);
				ServerSpawnPlayer1Piece(8, 8, "88", 1);
				ServerSpawnPlayer1Piece(10, 3, "103", 2);
				ServerSpawnPlayer1Piece(10, 7, "107", 3);
				ServerSpawnPlayer1Piece(12, 5, "125", 4);
				break;
		}
	}
	
	// Genera las fichas del jugador 2 (cliente)
	private void ServerSpawnPlayer2()
	{
		// Genero las fichas segun el nivel seleccionado
		switch (level)
		{
			case 1:
				ServerSpawnPlayer2Piece(4, 5, "45", 0);
				break;
			case 2:
				ServerSpawnPlayer2Piece(4, 5, "45", 0);
				ServerSpawnPlayer2Piece(2, 5, "25", 1);
				break;
			case 3:
				ServerSpawnPlayer2Piece(6, 2, "62", 0);
				ServerSpawnPlayer2Piece(6, 8, "68", 1);
				ServerSpawnPlayer2Piece(4, 3, "43", 2);
				ServerSpawnPlayer2Piece(4, 7, "47", 3);
				ServerSpawnPlayer2Piece(2, 5, "25", 4);
				break;
		}
	}

	// Genera la pelota
	private void ServerSpawnBall()
	{
		matrixAttributes = ballPrefab.GetComponent<MatrixAttributes>();
		matrixAttributes.x = 7;
		matrixAttributes.y = 5;
		spawningPosition = GameObject.Find("75").transform.position;
		Network.Instantiate(ballPrefab, spawningPosition, ballPrefab.transform.rotation, 0);
	}

	// Genera una ficha del jugador 1
	void ServerSpawnPlayer1Piece(int x, int y, string pieceName, int pieceNumber)
	{
		matrixAttributes = playerPrefab1[pieceNumber].GetComponent<MatrixAttributes>();
		matrixAttributes.x = x;
		matrixAttributes.y = y;
		spawningPosition = GameObject.Find(pieceName).transform.position;
		Network.Instantiate(playerPrefab1[pieceNumber], spawningPosition, playerPrefab1[pieceNumber].transform.rotation, 0);
	}
	
	// Genera una ficha del jugador 2
	void ServerSpawnPlayer2Piece(int x, int y, string pieceName, int pieceNumber)
	{
		matrixAttributes = playerPrefab2[pieceNumber].GetComponent<MatrixAttributes>();
		matrixAttributes.x = x;
		matrixAttributes.y = y;
		spawningPosition = GameObject.Find(pieceName).transform.position;
		Network.Instantiate(playerPrefab2[pieceNumber], spawningPosition, playerPrefab2[pieceNumber].transform.rotation, 0);
	}
	#endregion Spawns Multiplayer

	/**************************** Interfaz de usuario ****************************/
	// Labels para el titulo
	void generateTitle()
	{
		GUI.Label(new Rect(0, 10, Screen.width, 100), "MASTERGOAL", customStyle);
	}

	void OnGUI()
	{

		/**************************** Pantalla de Menu Principal ****************************/
		if (screenValue == Constants.MAIN)
		{

			generateTitle();

			// Boton Crear Partida Individual
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL, BTN_WIDTH, BTN_HEIGHT), "Stand Alone", customButton))
			{
				screenValue = Constants.LEVELSELECTIONSP;
			}

			// Boton Crear Partida Multijugador
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + BTN_HEIGHT + 30, BTN_WIDTH, BTN_HEIGHT), "Crear Partida", customButton))
			{
				screenValue = Constants.LEVELSELECTIONMP;
			}

			// Boton Buscar Partida
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + 2 * BTN_HEIGHT + 60, BTN_WIDTH, BTN_HEIGHT), "Buscar Partida", customButton))
			{
				screenValue = Constants.SERVERLIST;
				RefreshHostList();
			}

			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + BTN_HEIGHT + 300, BTN_WIDTH, BTN_HEIGHT), "Tutorial", customButton))
			{
				screenValue = Constants.TUTORIAL;
				RefreshHostList();
			}
		
			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyDown(KeyCode.Escape))
			{ 
				Application.Quit(); 
			}

		} 

		/**************************** Pantalla de Seleccion de nivel ****************************/
		if (screenValue == Constants.LEVELSELECTIONSP || screenValue == Constants.LEVELSELECTIONMP)
		{

			generateTitle();

			// Boton Nivel 1
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL, BTN_WIDTH, BTN_HEIGHT), "Nivel 1", customButton))
			{
				level = 1;
				if (screenValue == Constants.LEVELSELECTIONSP)
				{
					InitializeSP();
					screenValue = Constants.GAMESP;
				}
				else if (screenValue == Constants.LEVELSELECTIONMP)
				{
					gameName = generateRoomName(level);
					StartServer();
					screenValue = Constants.GAMEMP;
				}
			}
			
			// Boton Nivel 2
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + BTN_HEIGHT + 30, BTN_WIDTH, BTN_HEIGHT), "Nivel 2", customButton))
			{
				level = 2;
				if (screenValue == Constants.LEVELSELECTIONSP)
				{
					InitializeSP();
					screenValue = Constants.GAMESP;
				}
				else if (screenValue == Constants.LEVELSELECTIONMP)
				{
					gameName = generateRoomName(level);
					StartServer();
					screenValue = Constants.GAMEMP;
				}
			}

			// Boton Nivel 3
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + (BTN_HEIGHT + 30) * 2, BTN_WIDTH, BTN_HEIGHT), "Nivel 3", customButton))
			{
				level = 3;
				if (screenValue == Constants.LEVELSELECTIONSP)
				{
					InitializeSP();
					screenValue = Constants.GAMESP;
				}
				else if (screenValue == Constants.LEVELSELECTIONMP)
				{
					gameName = generateRoomName(level);
					StartServer();
					screenValue = Constants.GAMEMP;
				}
			}

			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyUp(KeyCode.Escape))
			{ 
				screenValue = Constants.MAIN; 
			}

		}

		/**************************** Pantalla de Lista de Servidores ****************************/
		if (screenValue == Constants.SERVERLIST)
		{

			generateTitle();

			// ScrollView con lista de servidores
			scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 8, Screen.height / 4, Screen.width * 3 / 4, Screen.height / 2), scrollPosition, new Rect(0, 0, Screen.width * 3 / 4 - 20, Screen.height * 3 / 4));
			GUI.Box(new Rect(0, 0, Screen.width * 3 / 4, Screen.height * 3 / 4), textoScrollView, customBox);
			if (hostList != null)
			{
				if (hostList.Length > 0)
				{
					textoScrollView = "";
				}
				else
				{
					textoScrollView = "No hay partidas disponibles";
				}
				for (int i = 0; i < hostList.Length; i++)
				{	

					if (GUI.Button(new Rect(0, (110 * i), Screen.width * 3 / 4 - 20, 80), hostList[i].gameName))
					{						
						// Obtengo el nivel
						level = (int)char.GetNumericValue((hostList[i].gameName)[(hostList[i].gameName).Length - 1]);
						JoinServer(hostList[i]);
						screenValue = Constants.GAMEMP;
					}
				}
			} 
			GUI.EndScrollView();

			// Boton Refrescar
			if (GUI.Button(new Rect(X_INICIAL, Screen.height * 3 / 4 + 50, BTN_WIDTH, BTN_HEIGHT), "Refrescar", customButton))
			{
				RefreshHostList();
			}

			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyUp(KeyCode.Escape))
			{ 
				screenValue = Constants.MAIN; 
			}

		}

		/**************************** Pantalla de Juego ****************************/
		if (screenValue == Constants.GAMESP || screenValue == Constants.GAMEMP)
		{

			update();// actualiza el valor del cronometro
			
			if (marcadores.ShowLabel)
			{// esto es para los mensajes de error
				GUI.Label(new Rect(100, 10, 200, 40), marcadores.errorText, marcadorStyle);
			}
			
			// muestra el turno actual
			GUI.Label(new Rect(400, 200, 200, 40), ("Turno = " + marcadores.turnoText), marcadorStyle);
			
			// para que desaparezca el  mensaje error
			//GUI.Label (new Rect (80,10,200,20), ("" + marcadores.contadorError),marcadorStyle);
			
			// muestra el contador
			//GUI.Label (new Rect (80,10,200,20), ("" + marcadores.contador),marcadorStyle);
			
			// esto es para los puntajes
			GUI.Label(new Rect(100, 400, 200, 20), ("Blanco : " + marcadores.puntajeBlanco), marcadorStyle);
			GUI.Label(new Rect(100, 450, 200, 20), ("Rojo : " + marcadores.puntajeRojo), marcadorStyle);


			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyUp(KeyCode.Escape) && screenValue == Constants.GAMEMP)
			{ 
				Network.Disconnect();
				MasterServer.UnregisterHost();
				screenValue = Constants.MAIN;
				destruirFichas();
			}
			else if (Input.GetKeyUp(KeyCode.Escape) && screenValue == Constants.GAMEMP)
			{
				screenValue = Constants.MAIN;
				destruirFichas();
			}
		}

	}

	// Destruye las fichas al salir del juego
	void destruirFichas()
	{
		switch (level)
		{
			case 1:
				Destroy(GameObject.FindWithTag("P1F1"));
				Destroy(GameObject.FindWithTag("P2F1"));
				break;
			case 2:
				Destroy(GameObject.FindWithTag("P1F1"));
				Destroy(GameObject.FindWithTag("P1F2"));
				
				Destroy(GameObject.FindWithTag("P2F1"));
				Destroy(GameObject.FindWithTag("P2F2"));
				break;
			case 3:
				Destroy(GameObject.FindWithTag("P1F1"));
				Destroy(GameObject.FindWithTag("P1F2"));
				Destroy(GameObject.FindWithTag("P1F3"));
				Destroy(GameObject.FindWithTag("P1F4"));
				Destroy(GameObject.FindWithTag("P1F5"));
				
				Destroy(GameObject.FindWithTag("P2F1"));
				Destroy(GameObject.FindWithTag("P2F2"));
				Destroy(GameObject.FindWithTag("P2F3"));
				Destroy(GameObject.FindWithTag("P2F4"));
				Destroy(GameObject.FindWithTag("P2F5"));
				break;
		}
		Destroy(GameObject.FindWithTag("BALL"));
	}
}
