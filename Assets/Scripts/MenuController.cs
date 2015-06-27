﻿using UnityEngine;
using System.Collections;

static class Constants
{
	public const int MAIN = 0;
	public const int SERVERLIST = 1;
	public const int LEVELSELECTION = 2;
	public const int GAME = 3;
	
}

public class MenuController : MonoBehaviour {

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
	private int X_INICIAL = Screen.width/4;
	private int Y_INICIAL = Screen.height/3;
	private int BTN_WIDTH = Screen.width/2;
	private int BTN_HEIGHT = Screen.width/6;

	// Estilos
	public GUIStyle customStyle;
	public GUIStyle customButton;
	public GUIStyle customBox;

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
			hostList = MasterServer.PollHostList();
	}
	
	//Unirse a un servidor
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		SpawnPlayer1 ();
		spawnBall ();
	}

	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		SpawnPlayer2 ();
	}


	void OnDisconnectedFromServer(){
		screenValue = Constants.MAIN;
	}

	void OnPlayerDisconnected(){
		Network.Disconnect ();
		MasterServer.UnregisterHost ();
		screenValue = Constants.MAIN;
	}

	//Generar nombre de juego
	string generateRoomName(int level){
		int number = Random.Range (0, 100);
		return "Juegol" + number + "-Nivel" + level;
	}

	// Genera las fichas del jugador 1 (server)
	private void SpawnPlayer1()
	{		
		// Genero las fichas segun el nivel seleccionado
		switch (level) {
			case 1:
				spawnPlayer1Piece (10, 5, "105", 0);
				break;
			case 2:
				spawnPlayer1Piece (10, 5, "105", 0);
				spawnPlayer1Piece (12, 5, "125", 1);
				break;
			case 3:
				spawnPlayer1Piece (8, 2, "82", 0);
				spawnPlayer1Piece (8, 8, "88", 1);
				spawnPlayer1Piece (10, 3, "103", 2);
				spawnPlayer1Piece (10, 7, "107", 3);
				spawnPlayer1Piece (12, 5, "125", 4);
				break;
		}
	}
	
	// Genera las fichas del jugador 2 (cliente)
	private void SpawnPlayer2()
	{
		// Genero las fichas segun el nivel seleccionado
		switch (level) {
		case 1:
			spawnPlayer2Piece (4, 5, "45", 0);
			break;
		case 2:
			spawnPlayer2Piece (4, 5, "45", 0);
			spawnPlayer2Piece (2, 5, "25", 1);
			break;
		case 3:
			spawnPlayer2Piece (6, 2, "62", 0);
			spawnPlayer2Piece (6, 8, "68", 1);
			spawnPlayer2Piece (4, 3, "43", 2);
			spawnPlayer2Piece (4, 7, "47", 3);
			spawnPlayer2Piece (2, 5, "25", 4);
			break;
		}
	}

	// Genera la pelota
	private void spawnBall(){
		matrixAttributes = ballPrefab.GetComponent<MatrixAttributes> ();
		matrixAttributes.x = 7;
		matrixAttributes.y = 5;
		spawningPosition = GameObject.Find ("75").transform.position;
		Network.Instantiate (ballPrefab, spawningPosition, ballPrefab.transform.rotation, 0);
	}

	// Genera una ficha del jugador 1
	void spawnPlayer1Piece(int x, int y, string pieceName, int pieceNumber){
		matrixAttributes = playerPrefab1[pieceNumber].GetComponent<MatrixAttributes> ();
		matrixAttributes.x = x;
		matrixAttributes.y = y;
		spawningPosition = GameObject.Find (pieceName).transform.position;
		Network.Instantiate (playerPrefab1[pieceNumber], spawningPosition, playerPrefab1[pieceNumber].transform.rotation, 0);
	}
	
	// Genera una ficha del jugador 2
	void spawnPlayer2Piece(int x, int y, string pieceName, int pieceNumber){
		matrixAttributes = playerPrefab2[pieceNumber].GetComponent<MatrixAttributes> ();
		matrixAttributes.x = x;
		matrixAttributes.y = y;
		spawningPosition = GameObject.Find (pieceName).transform.position;
		Network.Instantiate (playerPrefab2[pieceNumber], spawningPosition, playerPrefab2[pieceNumber].transform.rotation, 0);
	}

	/**************************** Interfaz de usuario ****************************/
	// Labels para el titulo
	void generateTitle(){
		GUI.Label (new Rect (0,10,Screen.width,100), "MASTERGOAL", customStyle);
		GUI.Label (new Rect (0,110,Screen.width,100), "MULTIJUGADOR", customStyle);
	}

	void OnGUI () {

		/**************************** Pantalla de Menu Principal ****************************/
		if (screenValue == Constants.MAIN) {

			generateTitle();

			// Boton Crear Partida
			if (GUI.Button (new Rect (X_INICIAL, Y_INICIAL, BTN_WIDTH, BTN_HEIGHT), "Crear Partida", customButton)) {
				screenValue = Constants.LEVELSELECTION;
			}
			
			// Boton Buscar Partida
			if (GUI.Button (new Rect (X_INICIAL, Y_INICIAL + BTN_HEIGHT + 60, BTN_WIDTH, BTN_HEIGHT), "Buscar Partida", customButton)) {
				screenValue = Constants.SERVERLIST;
				RefreshHostList();
			}
		
			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyDown(KeyCode.Escape)) { 
				Application.Quit(); 
			}

		} 

		/**************************** Pantalla de Seleccion de nivel ****************************/
		if (screenValue == Constants.LEVELSELECTION) {

			generateTitle();

			// Boton Nivel 1
			if (GUI.Button (new Rect (X_INICIAL, Y_INICIAL, BTN_WIDTH, BTN_HEIGHT), "Nivel 1", customButton)) {
				level = 1;
				gameName = generateRoomName(level);
				StartServer();
				screenValue = Constants.GAME;
			}
			
			// Boton Nivel 2
			if (GUI.Button (new Rect (X_INICIAL, Y_INICIAL + BTN_HEIGHT + 30, BTN_WIDTH, BTN_HEIGHT), "Nivel 2", customButton)) {
				level = 2;
				gameName = generateRoomName(level);
				StartServer();
				screenValue = Constants.GAME;
			}

			// Boton Nivel 3
			if (GUI.Button (new Rect (X_INICIAL, Y_INICIAL + (BTN_HEIGHT + 30) * 2, BTN_WIDTH, BTN_HEIGHT), "Nivel 3", customButton)) {
				level = 3;
				gameName = generateRoomName(level);
				StartServer();
				screenValue = Constants.GAME;
			}

			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyUp(KeyCode.Escape)) { 
				screenValue = Constants.MAIN; 
			}

		}

		/**************************** Pantalla de Lista de Servidores ****************************/
		if (screenValue == Constants.SERVERLIST){

			generateTitle();

			// ScrollView con lista de servidores
			scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/8, Screen.height/4, Screen.width*3/4, Screen.height/2), scrollPosition, new Rect(0, 0, Screen.width*3/4-20, Screen.height*3/4));
				GUI.Box(new Rect(0,0,Screen.width*3/4,Screen.height*3/4),textoScrollView, customBox);
				if (hostList != null)
				{
					if (hostList.Length > 0) textoScrollView = "";
					else textoScrollView = "No hay partidas disponibles";
					for (int i = 0; i < hostList.Length; i++)
					{	

						if (GUI.Button(new Rect(0,(110 * i), Screen.width*3/4-20, 80), hostList[i].gameName)){						
							// Obtengo el nivel
							level = (int)char.GetNumericValue((hostList[i].gameName)[(hostList[i].gameName).Length - 1]);
							JoinServer(hostList[i]);
							screenValue = Constants.GAME;
						}
					}
				} 
			GUI.EndScrollView();

			// Boton Refrescar
			if (GUI.Button (new Rect (X_INICIAL, Screen.height*3/4 + 50, BTN_WIDTH, BTN_HEIGHT), "Refrescar", customButton)) {
				RefreshHostList();
			}

			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyUp(KeyCode.Escape)) { 
				screenValue = Constants.MAIN; 
			}

		}

		/**************************** Pantalla de Juego ****************************/
		if (screenValue == Constants.GAME) {

			// Evento a llamar al apretar el boton de atras de android
			if (Input.GetKeyUp(KeyCode.Escape)) { 
				Network.Disconnect();
				MasterServer.UnregisterHost();
				screenValue = Constants.MAIN; 
			}	
		}

	}
}
