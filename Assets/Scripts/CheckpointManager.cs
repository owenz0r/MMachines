using UnityEngine;
using System.Collections;

public class CheckpointManager : MonoBehaviour {

	public Transform startCheckpoint;

	public void checkPointTriggered( Transform checkpoint, Collider2D other )
	{
		other.GetComponent<CarController>().setCheckpoint( checkpoint );
	}
}
