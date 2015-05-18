using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour {

	public CarManager carManager;

	Transform m_startCheckpoint;
	int m_totalCheckpoints;
	int m_startId = 0;
	int m_endId;

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
		Transform leader = carManager.getAlive()[0];
		int highest = 0;
		foreach( Transform car in carManager.getAlive() )
		{
			int lap = car.GetComponent<CarController>().lap;
			int id = car.GetComponent<CarController>().lastCheckpointId();
			int lapId = ( m_totalCheckpoints * lap ) + id;
			if( lapId > highest )
			{
				leader = car;
				highest = lapId;
			}
		}
		return leader;
	}
}
