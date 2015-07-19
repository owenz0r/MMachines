using UnityEngine;
using System.Collections;

public class SkidController : MonoBehaviour {

	TrailRenderer m_left;
	TrailRenderer m_right;
	Rigidbody2D m_rigidbody;

	// Use this for initialization
	void Start () {
		m_left = transform.FindChild( "back_left" ).GetComponent<TrailRenderer>();
		m_right = transform.FindChild( "back_right" ).GetComponent<TrailRenderer>();
		m_rigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		print ( m_rigidbody.velocity.magnitude );
		if( m_rigidbody.velocity.magnitude < 5.0f )
		{
			//m_left.enabled = true;
			//m_right.enabled = true;
			m_left.time = 0.3f;
			m_right.time = 0.3f;
		} else {
			//m_left.enabled = false;
			//m_right.enabled = false;
			m_left.time = 0.0f;
			m_right.time = 0.0f;
		}
	}
}
