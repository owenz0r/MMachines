using UnityEngine;
using System.Collections;

public class CarManager : MonoBehaviour {

	public Transform[] carArray;

	public void Awake()
	{
		/*
		int numPlayers = PlayerPrefs.GetInt( "NumPlayers" );
		for( int i=0; i < numPlayers; i++ )
		{
			carArray[i].GetComponent<CarController>().inputNumber = PlayerPrefs.GetString( "Player" + (i+1) );
		}
		*/
		for( int i=0; i < 4; i++ )
		{
			if( PlayerPrefs.HasKey( "Player" + (i+1) ) )
			{
				carArray[i].GetComponent<CarController>().inputNumber = PlayerPrefs.GetString( "Player" + (i+1) );
			} else {
				carArray[i].gameObject.SetActive( false );
			}
		}
	}

	public void resetAll()
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().Reset();
	}

	public void freezeAll()
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().freeze();
	}

	public void matchAllCheckpoints( CarController controller )
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().matchCheckpoint( controller );
	}

	public void updateAllCheckpoints( Transform checkpoint )
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().setCheckpoint( checkpoint );
	}

	public int numAlive()
	{
		int count = 0;
		foreach( Transform car in carArray )
		{
			if( !car.GetComponent<CarController>().isDead )
				count++;
		}
		return count;
	}

	public Transform getWinner()
	{
		foreach( Transform car in carArray )
		{
			if( !car.GetComponent<CarController>().isDead )
				return car;
		}
		return null;
	}

	public Transform[] getAlive()
	{
		Transform[] livePlayers = new Transform[ numAlive() ];
		int count = 0;
		foreach( Transform car in carArray )
		{
			if( !car.GetComponent<CarController>().isDead )
				livePlayers[ count++ ] = car;
		}
		return livePlayers;
	}
}
