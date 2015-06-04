using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {

	public int id;

	private CheckpointManager m_manager;
	private Transform m_nextCheckpoint;
	private Vector3 m_forward;

	void Start()
	{
		m_manager = transform.parent.GetComponent<CheckpointManager>();
		m_nextCheckpoint = m_manager.getNextCheckpoint( id );
		m_forward = m_nextCheckpoint.position - transform.position;
		m_forward.Normalize();
	}

	void OnTriggerEnter2D( Collider2D other )
	{
		m_manager.checkPointTriggered( transform, other );
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube( GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.size );
		Gizmos.DrawLine( transform.position, transform.position + m_forward );
	}

	public Vector3 forward
	{
		get { return m_forward; }
	}
}
