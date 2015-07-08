using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	bool m_tracking = true;
	bool m_racing = false;
	GameObject m_leader = null;
	IEnumerator m_relockRoutine;
	IEnumerator m_countdownRoutine;
	IEnumerator m_centerOnRoutine;

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

	void Awake()
	{
		checkpointManager.processCheckpoints();
		carManager.resetAll();
		StartCoroutine( Countdown () );
	}

	// Update is called once per frame
	void Update () {
		Transform leader = null;
		// keep track of leader each frame
		if( m_racing )
		{
			leader = checkpointManager.getLeader();
			if( leader.gameObject != m_leader )
			{
				m_leader = leader.gameObject;
				print( "LEADER CHANGED" );
				startRelock( 1.0f );
			}
		}

		// if we have a winner
		if( carManager.numAlive() == 1 && m_tracking == true )
		{
			print( "WINNER" );
			startCenterOn( leader.position, 1.0f, callback: resumeAfterScore );
			leader.GetComponent<CarController>().StartDance();
			m_racing = false;
			m_leader = null;
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
						startRelock( 1.0f );
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
		averagePosition += leader.position * live_players;
		averagePosition /= live_players + 1;

		averagePosition.z = 0.0f;
		return averagePosition;
	}

	void resumeAfterScore()
	{
		carManager.matchAllCheckpoints( carManager.getWinner().GetComponent<CarController>() );
		carManager.resetAll();
		carManager.freezeAll();
		m_leader = checkpointManager.getLeader().gameObject;
		Transform checkpoint = m_leader.GetComponent<CarController>().lastCheckpoint();
		startCenterOn( checkpoint.position, 1.0f, wait: 0.0f, callback: startCountdown );
	}

	public void resumeAfterFall()
	{
		m_tracking = false;
		if( carManager.numAlive() == 1 )
		{
			// celebrate
			startCenterOn( averagePosition(), 1.0f, callback: resumeAfterScore );
			carManager.getWinner().GetComponent<CarController>().StartDance();
		} else {
			// continue with the race
			startCenterOn( averagePosition(), 0.2f, wait: 0.0f, callback: () => { m_tracking = true; } );
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
		m_tracking = false;
		foreach( Transform player in players )
			player.GetComponent<CarController>().freeze();
		m_leader = null;

		Text text = ui.GetComponent<Text>();
		for( int i = 3; i > 0; i-- )
		{
			text.text = i.ToString();
			yield return new WaitForSeconds(1.0f);
		}
		text.text = "GO!";
		m_racing = true;
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
		if( m_countdownRoutine != null )
			StopCoroutine( m_countdownRoutine );
		m_countdownRoutine = Countdown();
		StartCoroutine( m_countdownRoutine );
	}

	void startRelock( float time )
	{
		print( "RELOCK" );
		if( m_relockRoutine != null )
			StopCoroutine( m_relockRoutine );
		if( m_centerOnRoutine != null )
			StopCoroutine( m_centerOnRoutine );
		m_relockRoutine = ReLock( time );
		StartCoroutine( m_relockRoutine );
	}

	void startCenterOn( Vector3 target, float seconds, float wait = 1.0f, Action callback = null )
	{
		print( "CENTER ON" );
		if( m_centerOnRoutine != null )
			StopCoroutine( m_centerOnRoutine );
		if( m_relockRoutine != null )
			StopCoroutine( m_relockRoutine );
		m_centerOnRoutine = CenterOn( target, seconds, wait, callback );
		StartCoroutine( m_centerOnRoutine );
	}


}
