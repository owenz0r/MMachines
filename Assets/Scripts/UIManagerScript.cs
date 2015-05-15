using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

	public Animator p1Text;
	public Animator p2Text;
	public Animator p3Text;
	public Animator p4Text;

	private int nextPlayer = 1;
	private bool[] inputJoined = { false, false, false, false, false }; // 5 so we don't have to +1 all the time
	//private string[] inputMap = {"","","","",""}; // 5 so we don't have to +1 all the time

	void Start()
	{
		PlayerPrefs.DeleteAll();
	}

	void Update()
	{
		if( Input.GetButtonDown( "Start_1" ) ){
			if( !inputJoined[1] )
			{
				addPlayer( "1" );
			} else if( nextPlayer > 2 ) {
				StartGame();
			}
		} else if ( Input.GetButtonDown( "Start_2" ) ) {
			if( !inputJoined[2] )
			{
				addPlayer( "2" );
			} else if( nextPlayer > 2 ) {
				StartGame();
			}
		} else if ( Input.GetButtonDown( "Start_3" ) ) {
			if( !inputJoined[3] )
			{
				addPlayer( "3" );
			} else if( nextPlayer > 2 ) {
				StartGame();
			}
		} else if ( Input.GetButtonDown( "Start_4" ) ) {
			if( !inputJoined[4] )
			{
				addPlayer( "4" );
			} else if( nextPlayer > 2 ) {
				StartGame();
			}
		}
	}

	void addPlayer( string input_number )
	{
		print( "Adding " + input_number + " as " + nextPlayer );
		switch( nextPlayer )
		{
			case 1: 
				p1Text.enabled = true;
				//inputMap[1] = input_number;
				PlayerPrefs.SetString( "Player1", input_number );
				break;
			case 2: 
				p2Text.enabled = true;
				//inputMap[2] = input_number;
				PlayerPrefs.SetString( "Player2", input_number );
				break;
			case 3: 
				p3Text.enabled = true;
				//inputMap[3] = input_number;
				PlayerPrefs.SetString( "Player3", input_number );
				break;
			case 4: 
				p4Text.enabled = true;
				//inputMap[4] = input_number;
				PlayerPrefs.SetString( "Player4", input_number );
				break;
			default:
				print( "Error on player number - " + nextPlayer );
				break;
		}
		inputJoined[ int.Parse( input_number ) ] = true;
		nextPlayer++;
	}

	public void StartGame()
	{
		PlayerPrefs.SetInt( "NumPlayers", nextPlayer - 1 );
		Application.LoadLevel("main");
	}
}
