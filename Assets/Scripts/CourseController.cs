using UnityEngine;
using System.Collections;

public class CourseController : MonoBehaviour {

	public CameraController camController;

	void OnTriggerStay2D( Collider2D other )
	{
		foreach( BoxCollider2D boxCollider in GetComponents<BoxCollider2D>() )
		{
			if( !other.GetComponent<CarController>().isFalling && !other.GetComponent<CarController>().isDead )
			{
				Vector3 min = other.bounds.min;
				min.z = 0.0f;
				Vector3 max = other.bounds.max;
				max.z = 0.0f;
				if( boxCollider.bounds.Contains( min ) && boxCollider.bounds.Contains( max ) )
					other.GetComponent<CarController>().StartFall( camController.resumeAfterFall );
			}
		}
	}
}
