using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour {

	public int id;
	private CheckpointManager manager;

	void Start()
	{
		manager = transform.parent.GetComponent<CheckpointManager>();
	}

	void OnTriggerEnter2D( Collider2D other )
	{
		manager.checkPointTriggered( transform, other );
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawCube( GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.size );
	}
}
