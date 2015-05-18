using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	bool m_tracking = true;
	//Transform m_restartCheckpoint;

	public float minZoom = 0.5f;
	public float maxZoom = 5.0f;
	public Transform[] players;
	public Transform ui;
	public float xEdgeMin = 0.05f;
	public float xEdgeMax = 0.95f;
	public float yEdgeMin = 0.05f;
	public float yEdgeMax = 0.95f;
	public CarManager carManager;
	public CheckpointManager checkpointManager;

	void Start()
	{
		checkpointManager.processCheckpoints();
		carManager.resetAll();
		StartCoroutine( Countdown () );
	}

	// Update is called once per frame
	void Update () {

		// keep track of leader each frame
		Transform leader = checkpointManager.getLeader();

		// if we have a winner
		if( carManager.numAlive() == 1 && m_tracking == true )
		{
			StartCoroutine( CenterOn( leader.position, 1.0f, callback: resumeAfterScore ) );
			leader.GetComponent<CarController>().StartDance();
		} else {
			// move camera
			if( m_tracking )
				transform.Translate( averagePosition() * -1.0f );
			// check for player going out of view
			foreach( Transform player in players )
			{
				// only check cars that are not falling already
				if( player.GetComponent<CarController>().isActive() )
				{
					if( IsOutOfBounds( player.position ) && player != leader )
					{
						player.GetComponent<CarController>().isDead = true;
						StartCoroutine( ReLock( 1.0f ) );
					}
				}
			}

		}
	}

	Vector3 averagePosition()
	{
		int live_players = 0;
		Vector3 averagePosition = Vector3.zero;
		for( int i=0; i < players.Length; i++ )
		{
			if( !players[i].GetComponent<CarController>().isDead )
			{
				averagePosition += players[i].position;
				live_players++;
			}
		}
		averagePosition /= live_players;

		Transform leader = checkpointManager.getLeader();
		averagePosition += leader.position * 2;
		averagePosition /= 3;

		averagePosition.z = 0.0f;
		return averagePosition;
	}

	void resumeAfterScore()
	{
		carManager.matchAllCheckpoints( carManager.getWinner().GetComponent<CarController>() );
		carManager.resetAll();
		carManager.freezeAll();
		StartCoroutine( CenterOn( averagePosition(), 1.0f, wait: 0.0f, callback: startCountdown ) );
	}

	public void resumeAfterFall()
	{
		m_tracking = false;
		if( carManager.numAlive() == 1 )
		{
			// celebrate
			StartCoroutine( CenterOn( averagePosition(), 1.0f, callback: resumeAfterScore ) );
			carManager.getWinner().GetComponent<CarController>().StartDance();
		} else {
			// continue with the race
			StartCoroutine( CenterOn( averagePosition(), 0.2f, wait: 0.0f, callback: () => { m_tracking = true; } ) );
		}
	}

	public bool IsOutOfBounds( Vector3 pos )
	{
		Vector3 viewportPos = Camera.main.WorldToViewportPoint( pos );
		if( viewportPos.x < xEdgeMin || viewportPos.x > xEdgeMax )
			return true;
		if( viewportPos.y < yEdgeMin || viewportPos.y > yEdgeMax )
			return true;
		return false;
	}

	IEnumerator Countdown()
	{
		foreach( Transform player in players )
			player.GetComponent<CarController>().freeze();
		m_tracking = true;

		Text text = ui.GetComponent<Text>();
		for( int i = 3; i > 0; i-- )
		{
			text.text = i.ToString();
			yield return new WaitForSeconds(1.0f);
		}
		text.text = "GO!";
		foreach( Transform player in players )
			player.GetComponent<CarController>().unfreeze();
		yield return new WaitForSeconds(1.0f);
		text.text = "";
	}
	
	IEnumerator CenterOn( Vector3 target, float seconds, float wait = 1.0f, Action callback = null )
	{
		m_tracking = false;
		Vector3 dist = new Vector3( target.x, target.y, 0.0f ) * -1.0f;
		Vector3 startPos = transform.position;
		float totalTime = 0.0f;
		while( totalTime < seconds )
		{
			float step = totalTime / seconds;
			transform.position = startPos + ( dist * step );
			yield return null;
			totalTime += Time.deltaTime;
		}
		yield return new WaitForSeconds( wait );
		if( callback != null )
			callback();
	}

	IEnumerator ReLock( float seconds  )
	{
		m_tracking = false;
		float totalTime = 0.0f;
		while( totalTime < seconds )
		{
			Vector3 dist = averagePosition() * -1.0f;
			float step = totalTime / seconds;
			transform.position = transform.position + ( dist * step );
			yield return null;
			totalTime += Time.deltaTime;
		}
		m_tracking = true;
	}

	public void startCountdown()
	{
		StartCoroutine( Countdown() );
	}

}
