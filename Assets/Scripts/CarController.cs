using UnityEngine;
using System.Collections;
using System;

public class CarController : MonoBehaviour {

	public string playerNumber = "1";
	public float maxThrust = 15.0f;
	public float maxReverse = 5.0f;
	public float maxTorque = 5.0f;
	public float maxTraction = 5.0f;
	public Transform startPosition;
	public CarManager carManager;
	public CameraController camController;
	
	float m_accel;
	float m_reverse;
	float m_steering;
	bool m_falling;
	bool m_freeze;
	bool m_dancing;
	bool m_dead;
	Transform m_lastCheckpoint;
	int m_lastCheckpointId = -1;

	void Start()
	{
		m_freeze = true;
		m_dead = false;
		Reset();
	}

	public void Reset()
	{
		StopCoroutine( "Fall" );
		StopCoroutine( "Dance" );
		transform.localScale = startPosition.localScale;
		transform.position = startPosition.position;
		transform.rotation = startPosition.rotation;
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
		m_falling = false;
		m_dancing = false;
		m_dead = false;
	}

	// Update is called once per frame
	void Update () {
		if( m_falling || m_freeze || m_dancing )
		{
			m_steering = 0.0f;
			m_accel = 0.0f;
			m_reverse = 0.0f;
		} else {
			processKeyboardInput();
		}
	}

	void FixedUpdate()
	{
		// acceleration
		if( m_reverse > 0.0f )
		{
			GetComponent<Rigidbody2D>().AddRelativeForce( new Vector2( 0.0f, maxReverse * (m_reverse * -1.0f) ) );
		} else {
			GetComponent<Rigidbody2D>().AddRelativeForce( new Vector2( 0.0f, maxThrust * m_accel ) );
		}

		// turning
		Transform axel_transform = transform.GetChild( 1 );
		axel_transform.localEulerAngles = new Vector3( 0.0f, 0.0f, 45.0f * m_steering );

		Vector2 localVelocity = transform.InverseTransformDirection( GetComponent<Rigidbody2D>().velocity );
		if( GetComponent<Rigidbody2D>().velocity.magnitude > 0.5f )
		{
			// reverse steering if we're going backwards
			if( localVelocity.y < 0.0f )
				m_steering *= -1.0f;
			GetComponent<Rigidbody2D>().AddTorque( maxTorque * m_steering );
		}

		// traction
		Vector2 tractionForce = new Vector2( localVelocity.x * -1.0f, 0.0f );
		if( localVelocity.magnitude > 0.1f )
		{
			float tractionScalar = 1.0f / localVelocity.magnitude;
			Vector2 traction = tractionScalar * tractionForce;

			// enforce max traction
			float negMaxTraction = maxTraction * -1.0f;
			if( traction.x > maxTraction )
				traction.x = maxTraction;
			if( traction.x < negMaxTraction )
				traction.x = negMaxTraction;

			GetComponent<Rigidbody2D>().AddRelativeForce( traction, ForceMode2D.Impulse );
		}

	}

	void processKeyboardInput()
	{
		m_steering = Input.GetAxis( "Horizontal_" + playerNumber );
		m_accel = Input.GetAxis( "Vertical_" + playerNumber );
		m_reverse = Input.GetAxis( "Brake_" + playerNumber );
	}
	
	IEnumerator Dance( Action callback = null )
	{
		print ( "DANCE" );
		m_dancing = true;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GetComponent<Rigidbody2D>().AddTorque( 40.0f );
		for( float f = 1.0f; f >= 0.0f; f -= 0.1f )
		{
			yield return new WaitForSeconds(0.1f);
		}
//		carManager.updateAllCheckpoints( m_lastCheckpoint );
//		carManager.resetAll();
		if( callback != null )
			callback();
	}
	
	public void StartDance( Action callback = null )
	{
		if( !m_falling && !m_dancing && !m_freeze )
			StartCoroutine( Dance ( callback ) );
	}

	IEnumerator Fall( Action callback = null )
	{
		print( "FALL" );
		m_falling = true;
		GetComponent<Rigidbody2D>().AddTorque( 40.0f );
		for( float f = 1.0f; f >= 0.0f; f -= 0.1f )
		{
			transform.localScale = new Vector3( f, f, f);
			yield return new WaitForSeconds(0.1f);
		}

		if( !camController.IsOutOfBounds( startPosition.position ) )
		{
			Reset ();
		} else {
			m_dead = true;
		}
		if( callback != null )
			callback();
	}

	public void StartFall( Action callback = null )
	{
		if( !m_falling && !m_dancing && !m_freeze )
			StartCoroutine( Fall ( callback ) );
	}

	public void freeze()
	{
		m_freeze = true;
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
	}

	public void unfreeze()
	{
		m_freeze = false;
	}

	public bool isFalling()
	{
		return m_falling;
	}

	public void setCheckpoint( Transform checkpoint )
	{
		m_lastCheckpoint = checkpoint;
		m_lastCheckpointId = checkpoint.GetComponent<CheckpointController>().id;
		if( playerNumber == "1" )
		{
			startPosition = checkpoint.GetChild( 0 );
		}
		else if ( playerNumber == "2" )
		{
			startPosition = checkpoint.GetChild( 1 );
		}
	}

	public Transform lastCheckpoint()
	{
		return m_lastCheckpoint;
	}

	public int lastCheckpointId()
	{
		return m_lastCheckpointId;
	}

	public bool isDead()
	{
		return m_dead;
	}

	public bool isActive()
	{
		return !m_dead && !m_freeze && !m_falling;
	}
}
