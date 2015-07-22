using UnityEngine;
using System.Collections;
using System;
using XboxCtrlrInput;
using UnityEngine.UI;
using UnityEditor;

public class CarController : MonoBehaviour {

	public string playerNumber = "1";
	public int inputNumber = 1;
	public float maxThrust = 15.0f;
	public float maxReverse = 5.0f;
	public float maxTorque = 5.0f;
	public float maxTraction = 5.0f;
	public float minEnginePitch = 0.5f;
	public float maxEnginePitch = 2.0f;
	public AudioSource engineSound = null;
	public AudioSource tyreSqueal = null;

	public CarManager carManager;
	public CameraController camController;
	public CheckpointManager checkpointManager;
	public Text uiScore;
	
	float m_accel;
	float m_reverse;
	float m_steering;
	bool m_falling;
	bool m_freeze;
	bool m_dancing;
	bool m_dead;
	Transform m_lastCheckpoint;
	int m_lastCheckpointId = -1;
	Transform m_startPosition;
	int m_lap = 0;
	IEnumerator m_fallRoutine;
	TrailGenerator m_trail_generator = null;
	Rigidbody2D m_rigidbody = null;

	void Awake()
	{
		m_trail_generator = GetComponent<TrailGenerator>();
		m_rigidbody = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		m_freeze = true;
		m_dead = false;
	}

	public void Reset()
	{
		if( m_rigidbody == null )
			m_rigidbody = GetComponent<Rigidbody2D>();
		StopCoroutine( "Fall" );
		StopCoroutine( "Dance" );
		transform.localScale = m_startPosition.localScale;
		transform.position = m_startPosition.position;
		transform.rotation = m_startPosition.rotation;
		m_rigidbody.velocity = Vector3.zero;
		m_rigidbody.angularVelocity = 0.0f;
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
		adjustEngineSound();
		doTyreSqueal();
	}

	void FixedUpdate()
	{
		// acceleration
		if( m_reverse > 0.0f )
		{
			m_rigidbody.AddRelativeForce( new Vector2( 0.0f, maxReverse * (m_reverse * -1.0f) ) );
		} else {
			m_rigidbody.AddRelativeForce( new Vector2( 0.0f, maxThrust * m_accel ) );
		}

		// turning
		Transform axel_transform = transform.GetChild( 1 );
		axel_transform.localEulerAngles = new Vector3( 0.0f, 0.0f, 45.0f * m_steering );

		Vector2 localVelocity = transform.InverseTransformDirection( m_rigidbody.velocity );
		if( m_rigidbody.velocity.magnitude > 0.5f )
		{
			// reverse steering if we're going backwards
			if( localVelocity.y < 0.0f )
				m_steering *= -1.0f;
			m_rigidbody.AddTorque( maxTorque * m_steering );
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

			m_rigidbody.AddRelativeForce( traction, ForceMode2D.Impulse );
		}

	}

	void processKeyboardInput()
	{
		m_steering = XCI.GetAxis( XboxAxis.LeftStickX, inputNumber ) * -1.0f;
		m_accel = XCI.GetAxis( XboxAxis.RightTrigger, inputNumber );
		m_reverse = XCI.GetAxis( XboxAxis.LeftTrigger, inputNumber );
	}

	void adjustEngineSound()
	{
		float speed = m_rigidbody.velocity.magnitude;
		if( speed > 8.0f )
			speed = 8.0f;
		speed = speed / 8.0f;
		speed *= ( maxEnginePitch - minEnginePitch );
		engineSound.pitch = minEnginePitch + speed;
	}

	void doTyreSqueal()
	{
		float dot = Vector3.Dot( transform.up, m_rigidbody.velocity.normalized );
		if( m_rigidbody.velocity.magnitude > 0.5f && dot < 0.9f && dot > -0.9f )
		{
			if( tyreSqueal.isPlaying == false )
			{
				tyreSqueal.Play();
				m_trail_generator.startTrail();
			}
		} else {
			tyreSqueal.Stop();
			m_trail_generator.stopTrail();
		}
	}

	IEnumerator Dance( Action callback = null )
	{
//		print ( "DANCE" );
		m_dancing = true;
		m_rigidbody.velocity = Vector2.zero;
		m_rigidbody.AddTorque( 40.0f );
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
		if( m_falling )
			StopFalling();
		if( !m_falling && !m_dancing && !m_freeze )
		{
			StartCoroutine( Dance ( callback ) );
			uiScore.text = (int.Parse( uiScore.text ) + 1).ToString();
		}
	}

	IEnumerator Fall( Action callback = null )
	{
//		print( "FALL" );
		m_falling = true;
		m_rigidbody.AddTorque( 40.0f );
		for( float f = 1.0f; f >= 0.0f; f -= 0.1f )
		{
			transform.localScale = new Vector3( f, f, f);
			yield return new WaitForSeconds(0.1f);
		}

		// if we can reset, or we're the last active player
		if( !camController.IsOutOfBounds( m_startPosition.position ) || ( carManager.numAlive() == 1 && m_dead == false ) )
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
		{
			m_fallRoutine = Fall ( callback );
			StartCoroutine( m_fallRoutine );
		}
	}

	public void StopFalling()
	{
		StopCoroutine( m_fallRoutine );
		m_falling = false;
	}

	public void freeze( bool stopMovement=true )
	{
		m_freeze = true;
		if( stopMovement )
		{
			m_rigidbody.velocity = Vector3.zero;
			m_rigidbody.angularVelocity = 0.0f;
		}
	}

	public void unfreeze()
	{
		m_freeze = false;
	}

	public void setCheckpoint( Transform checkpoint )
	{
		m_lastCheckpoint = checkpoint;
		m_lastCheckpointId = checkpoint.GetComponent<CheckpointController>().id;
		if( playerNumber == "1" )
		{
			m_startPosition = checkpoint.GetChild( 0 );
		}
		else if ( playerNumber == "2" )
		{
			m_startPosition = checkpoint.GetChild( 1 );
		}
		else if ( playerNumber == "3" )
		{
			m_startPosition = checkpoint.GetChild( 2 );
		}
		else if ( playerNumber == "4" )
		{
			m_startPosition = checkpoint.GetChild( 3 );
		}
	}
	
	public void matchCheckpoint( CarController other )
	{
		m_lastCheckpoint = other.lastCheckpoint();
		m_lastCheckpointId = other.lastCheckpointId();
		lap = other.lap;
//		print ( m_lastCheckpointId );
		if( playerNumber == "1" )
		{
			m_startPosition = m_lastCheckpoint.GetChild( 0 );
		}
		else if ( playerNumber == "2" )
		{
			m_startPosition = m_lastCheckpoint.GetChild( 1 );
		}
		else if ( playerNumber == "3" )
		{
			m_startPosition = m_lastCheckpoint.GetChild( 2 );
		}
		else if ( playerNumber == "4" )
		{
			m_startPosition = m_lastCheckpoint.GetChild( 3 );
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

	public bool isActive()
	{
		return !m_dead && !m_freeze && !m_falling && gameObject.activeSelf;
	}

	public bool isDead
	{
		get{ 
			if( gameObject.activeSelf == true )
			{
				return m_dead;
			} else {
				return true;
			}
		}
		set{ m_dead = value; this.freeze(); }
	}

	public int lap
	{
		get { return m_lap; }
		set { m_lap = value; }
	}

	public bool isFalling
	{
		get { return m_falling; }
	}

	public float distanceFromCheckpoint()
	{
		CheckpointController cpc = m_lastCheckpoint.GetComponent<CheckpointController>();
		Vector3 dir = transform.position - m_lastCheckpoint.position;
		float dot = Vector3.Dot( dir.normalized, cpc.forward );
		float mag = dir.sqrMagnitude;
		return mag * dot;
	}

	void OnDrawGizmos()
	{
		if( m_lastCheckpoint )
		{
			float mag = distanceFromCheckpoint();
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.green;
			style.fontSize += 15;
			Vector3 textPos = transform.position;
			textPos.y = textPos.y - 0.1f;
			Handles.Label( textPos, mag.ToString(), style );
		}
	}
}
