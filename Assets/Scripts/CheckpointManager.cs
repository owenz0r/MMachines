using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour {

	public CarManager carManager;

	Transform m_startCheckpoint;
	int m_totalCheckpoints;
	int m_startId = 0;
	int m_endId;
	Vector3 m_leaderPos = Vector3.zero;

	public Transform getNextCheckpoint( int id )
	{
		int nextId = 0;
		if( id != m_endId )
			nextId = id + 1;
		return getCheckpointById( nextId );
	}

	public Transform getCheckpointById( int id )
	{
		foreach( Transform child in transform )
		{
			if( child.GetComponent<CheckpointController>().id == id )
				return child.transform;
		}
		return null;
	}

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
		carManager.updateAllCheckpoints( m_startCheckpoint );
		m_totalCheckpoints = transform.childCount;
		m_endId = transform.childCount - 1;
	}

	public void checkPointTriggered( Transform checkpoint, Collider2D other )
	{
		CarController car = other.GetComponent<CarController>();
		int id = checkpoint.GetComponent<CheckpointController>().id;
		int lastId = car.lastCheckpointId();

		int lastLapId = ( car.lap * m_totalCheckpoints ) + lastId;

		if( lastId == m_endId && id == 0 )
		{
			car.lap++;
		} 
		else if ( lastId == 0 && id == m_endId )
		{
			car.lap--;
		}

		int lapId = ( car.lap * m_totalCheckpoints ) + id;

		if( lapId == lastLapId + 1 || lapId == lastLapId - 1 )
			car.setCheckpoint( checkpoint );
	}

	public int getNextCheckpointId( int current )
	{
		return ( current + 1 );
	}

	public Transform getLeader()
	{
		Transform[] alive = carManager.getAlive ();
		int[] checkpointIds = new int[ alive.Length ];
		//Transform leader = alive[0];
		int highest = 0;
		for( int i=0; i < alive.Length; i++ )
		{
			Transform car = alive[i];
			int lap = car.GetComponent<CarController>().lap;
			int id = car.GetComponent<CarController>().lastCheckpointId();
			int lapId = ( m_totalCheckpoints * lap ) + id;
			checkpointIds[i] = lapId;

			if( lapId > highest )
				highest = lapId;
		}

		// count number of cars past latest checkpoint
		List<int> leaders = new List<int>();
		for( int i=0; i < alive.Length; i++ )
		{
			if( checkpointIds[i] == highest )
				leaders.Add( i );
		}

		// we have a single definite leader
		if( leaders.Count == 1 )
		{
			m_leaderPos = alive[ leaders[0] ].position;
			return alive[ leaders[0] ];
		// need to check who has travelled furthest after the checkpoint
		} else {
			int checkpointId = alive[ leaders[0] ].GetComponent<CarController>().lastCheckpointId();
			Transform checkpoint = getCheckpointById( checkpointId );
			CheckpointController cpc = checkpoint.GetComponent<CheckpointController>();

			// get distances along forward vector
			float[] dist = new float[ leaders.Count ];
			for( int i=0; i < leaders.Count; i++ )
			{
				Transform car = alive[ leaders[i] ];
				Vector3 dir = car.position - checkpoint.position;
				float dot = Vector3.Dot ( dir.normalized, cpc.forward );
				Vector3 scale = Vector3.Scale( dir, cpc.forward );
				float mag = dir.sqrMagnitude;
				dist[i] = mag * dot;
			}

			// furthest distance is the leader
			int first = 0;
			for( int i=1; i < leaders.Count; i++ )
			{
				if( dist[i] > dist[first] )
					first = i;
			}

			m_leaderPos = alive[ leaders[first] ].position;
			return alive[ leaders[first] ];
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere( m_leaderPos, 0.1f );
	}
}
