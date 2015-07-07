using UnityEngine;
using System.Collections;

static class Constants
{
	public const int MAIN = 0;
	public const int SERVERLIST = 1;
	public const int LEVELSELECTIONSP = 2;
	public const int LEVELSELECTIONMP = 3;
	public const int LEVELSELECTIONMPOFFLINE = 4;
	public const int GAMESP = 5;
	public const int GAMEMP = 6;
	public const int GAMEMPOFFLINE = 7;
	public const int MAINMP = 8;
	public const int TUTORIAL = 9;
}

public class marcadores
{
	
	public static string errorText = "Error";// solo reemplazar esto para tener los mensajes
	public static string turnoText = "Blanco";
	public static int puntajeRojo = 0;
	public static int puntajeBlanco = 0;
	public static bool ShowLabel = false;
	public static int contador = 45;
	public static float contFloat = 45;
	public static float contadorErrorFloat = 5;
	public static int contadorError = 4;
	public static int ajusteResolucion = 0;
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
	public GUIStyle customText;
	public GUIStyle marcadorStyle ;
	public GUIStyle menuButton;

	bool isPaused= false;

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
	}

	public void OnDisconnectedFromServer()
	{
		PlayerController.end = false;
		destruirFichas();
		screenValue = Constants.MAIN;
	}

	void OnPlayerDisconnected()
	{
		Network.Disconnect();
		MasterServer.UnregisterHost();
		PlayerController.end = false;
		destruirFichas();
		screenValue = Constants.MAIN;
	}

	//Generar nombre de juego
	string generateRoomName(int level)
	{
		int number = Random.Range(0, 100);
		return "Juego" + number + "-Nivel" + level;
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

			// Boton Crear Partida Multijugador Offline
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + BTN_HEIGHT + 30, BTN_WIDTH, BTN_HEIGHT), "Multijugador Offline", customButton))
			{
				screenValue = Constants.LEVELSELECTIONMPOFFLINE;
			}

			// Boton Buscar Partida
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + 2 * BTN_HEIGHT + 60, BTN_WIDTH, BTN_HEIGHT), "Multijugador Online", customButton))
			{
				screenValue = Constants.MAINMP;
			}

			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + 3 * BTN_HEIGHT + 90, BTN_WIDTH, BTN_HEIGHT), "Tutorial", customButton))
			{
				screenValue = Constants.TUTORIAL;
			}


		} 

		/**************************** Pantalla de Seleccion de nivel ****************************/
		if (screenValue == Constants.LEVELSELECTIONSP || screenValue == Constants.LEVELSELECTIONMP || screenValue == Constants.LEVELSELECTIONMPOFFLINE)
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
				else if (screenValue == Constants.LEVELSELECTIONMPOFFLINE)
				{
					InitializeSP();
                    screenValue = Constants.GAMEMPOFFLINE;
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
				else if (screenValue == Constants.LEVELSELECTIONMPOFFLINE)
				{
					InitializeSP();
                    screenValue = Constants.GAMEMPOFFLINE;
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
				else if (screenValue == Constants.LEVELSELECTIONMPOFFLINE)
				{
					InitializeSP();	
                    screenValue = Constants.GAMEMPOFFLINE;
				}
			}

		}

		/**************************** Pantalla de Multijugador Online ****************************/

		if (screenValue == Constants.MAINMP) {
			generateTitle();

			// Boton Crear Partida Multijugador Online
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL, BTN_WIDTH, BTN_HEIGHT), "Crear Partida", customButton))
			{
				screenValue = Constants.LEVELSELECTIONMP;
			}
			
			// Boton Buscar Partida Multijugador Online
			if (GUI.Button(new Rect(X_INICIAL, Y_INICIAL + BTN_HEIGHT + 30, BTN_WIDTH, BTN_HEIGHT), "Buscar Partida", customButton))
			{
				screenValue = Constants.SERVERLIST;
				RefreshHostList();
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

					if (GUI.Button(new Rect(0, (110 * i), Screen.width * 3 / 4 - 20, 80), hostList[i].gameName, customButton))
					{						
						// Obtengo el nivel
						level = (int)char.GetNumericValue((hostList[i].gameName)[(hostList[i].gameName).Length - 1]);
						JoinServer(hostList[i]);
						gameName = hostList[i].gameName;
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

		}

		/**************************** Pantalla de Juego ****************************/
		if (screenValue == Constants.GAMESP || screenValue == Constants.GAMEMP || screenValue == Constants.GAMEMPOFFLINE)
		{

			if (!PlayerController.end){
				if (screenValue == Constants.GAMEMP){
					GUI.Label(new Rect(Screen.width/4, 10, 200, 40), gameName, marcadorStyle);
				}

				// Muestro el turno
				if (marcadores.turnoText == "Blanco")
					GUI.Label(new Rect(Screen.width - 350, 110, 200, 40), ("Turno : " + marcadores.turnoText), marcadorStyle);
				else
					GUI.Label(new Rect(Screen.width - 295, 110, 200, 40), ("Turno : " + marcadores.turnoText), marcadorStyle);

				// Muestro los puntajes
				GUI.Label(new Rect(10, 110, 200, 20), ("Blanco : " + marcadores.puntajeBlanco), marcadorStyle);
				GUI.Label(new Rect(10, 160, 200, 20), ("Rojo : " + marcadores.puntajeRojo), marcadorStyle);

				//Para pausar
				if (!isPaused)
				{
					if (GUI.Button(new Rect(Screen.width-120,Screen.height-120,120,120), "Menu", customButton))
					{
						isPaused = true;
					}
				}

				// Agrego un menu de juego para juegos SP y MPOFFLINE
				if (screenValue == Constants.GAMESP  || screenValue == Constants.GAMEMPOFFLINE)
				{	
					if (isPaused)
					{
						GUI.BeginGroup(new Rect(Screen.width/2 - 150, Screen.height/2 - 150,600, 400));
						
						GUI.Box(new Rect(5, 15, 330, 360), "");
						
						// CONTINUA EL JUEGO
						if(GUI.Button(new Rect(10, 20, 320, 80), "Continuar" , customButton)){
							print("you clicked Continuar");
							isPaused = false;
						}
						
						// REINICIA EL JUEGO
						if(GUI.Button(new Rect(10, 110, 320, 80), "Reiniciar", customButton)){
							PlayerController.end = false;
							destruirFichas();
							isPaused = false;
							InitializeSP();
						}
						
						///LLEVA AL MENU PRINCIPAL
						if(GUI.Button(new Rect(10, 200, 320, 80), "Menu Principal", customButton)){
							screenValue = Constants.MAIN;
							PlayerController.end = false;
							destruirFichas();
							isPaused = false;
						}
						
						// SALE DEL JUEGO
						if(GUI.Button(new Rect(10, 290, 320, 80), "Salir", customButton)){
							Application.Quit();
						}
						
						GUI.EndGroup();
					}
				}

				// Agrego un menu de juego para el juego multijugador online
				if (screenValue == Constants.GAMEMP)
				{	
					if (isPaused)
					{
						GUI.BeginGroup(new Rect(Screen.width/2 - 150, Screen.height/2 - 150,600, 400));
						
						GUI.Box(new Rect(5, 15, 330, 270), "");
						
						// CONTINUA EL JUEGO
						if(GUI.Button(new Rect(10, 20, 320, 80), "Continuar" , customButton)){
							print("you clicked Continuar");
							isPaused = false;
						}
						
						///LLEVA AL MENU PRINCIPAL
						if(GUI.Button(new Rect(10, 110, 320, 80), "Menu Principal", customButton)){
							Network.Disconnect();
							MasterServer.UnregisterHost();
							screenValue = Constants.MAIN;
							PlayerController.end = false;
							destruirFichas();
							isPaused = false;
						}
						
						// SALE DEL JUEGO
						if(GUI.Button(new Rect(10, 200, 320, 80), "Salir", customButton)){
							Application.Quit();
						}
						
						GUI.EndGroup();
					}
				}

			} else if (PlayerController.end){
				if (screenValue == Constants.GAMESP || screenValue == Constants.GAMEMPOFFLINE){
					// Muestro el cuadro de resultados luego de que desaparezca el mensaje de gol
					if (Time.time > (PlayerController.endTime + 0.75f)){
						// Cuadro de resultados
						GUI.Box(new Rect(50,Screen.height*1/4,Screen.width - 100,Screen.height/2 + Screen.height/12),"");
						
						// Textos del cuadro
						GUI.Label(new Rect(100, Screen.height*1/4 + 50, Screen.width - 200, 40), "Resultado Final", customText);
						GUI.Label(new Rect(100, Screen.height*1/4 + (Screen.height/2 + Screen.height/12)*1/4 + 1/8, Screen.width - 200, 40), "Blanco : " + marcadores.puntajeBlanco,customText);
						GUI.Label(new Rect(100, Screen.height*1/4 + (Screen.height/2 + Screen.height/12)*1/4 + 1/8 + 110, Screen.width - 200, 40), "Rojo : " + marcadores.puntajeRojo,customText);
						
						// Boton Reiniciar
						if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 3/4 + Screen.height/12 - 50 - Screen.width / 6 - Screen.width / 6 - 20, Screen.width / 2, Screen.width / 6), "Reiniciar", customButton))
						{
							PlayerController.end = false;
							destruirFichas();
							isPaused = false;
							InitializeSP();	
						}
						
						// Boton Volver al Menu Principal
						if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 3/4 + Screen.height/12 - 50 - Screen.width / 6, Screen.width / 2, Screen.width / 6), "Menu Principal", customButton))
						{
							PlayerController.end = false;
							destruirFichas();
							screenValue = Constants.MAIN; 
							isPaused = false;
						}
						
					}
				}
			}


		}

	}

	void Update(){
		// Al presionar el boton atras de android
		if (Input.GetKeyUp (KeyCode.Escape)) {
			switch (screenValue){
				case Constants.MAIN:
					Application.Quit();
					break;
				case Constants.LEVELSELECTIONMP:
					screenValue = Constants.MAINMP;
					break;
				case Constants.SERVERLIST:
					screenValue = Constants.MAINMP;
					break;
				case Constants.GAMESP:
					screenValue = Constants.MAIN;
					PlayerController.end = false;
					destruirFichas();
					break;
				case Constants.GAMEMPOFFLINE:
					screenValue = Constants.MAIN;
					PlayerController.end = false;
					destruirFichas();
					break;
				case Constants.GAMEMP:
					Network.Disconnect();
					MasterServer.UnregisterHost();
					screenValue = Constants.MAIN;
					PlayerController.end = false;
					destruirFichas();
					break;
				default:
					screenValue = Constants.MAIN;
					break;
			}
			isPaused = false;
		}
	}

	// Destruye las fichas al salir del juego
	public static void destruirFichas()
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
