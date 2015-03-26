using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour {

	public CarManager carManager;

	Transform m_startCheckpoint;
	int m_totalCheckpoints;
	int m_startId = 22;

	public void processCheckpoints()
	{
		foreach( Transform child in transform )
		{
			if( child.GetComponent<CheckpointController>().id == m_startId )
			{
				m_startCheckpoint = child;
				break;
			}
		}
		carManager.updateAllCheckpoints( m_startCheckpoint, m_startId );
		m_totalCheckpoints = transform.childCount;
	}

	public void checkPointTriggered( Transform checkpoint, Collider2D other )
	{
		int id = checkpoint.GetComponent<CheckpointController>().id;
		int lastId = other.GetComponent<CarController>().lastCheckpointId();

		// adjust the id to match the lap
		int lap = ( lastId + 1 ) / m_totalCheckpoints;
		if( lap >= 1 )
		{
			// we're going backwards
			if( id > 1 )
				lap--;
			id = ( lap * m_totalCheckpoints ) + id;
		}

		if( id == lastId + 1 || id == lastId - 1 )
			other.GetComponent<CarController>().setCheckpoint( checkpoint, id );
	}

	public int getNextCheckpointId( int current )
	{
		return ( current + 1 );
	}
}
