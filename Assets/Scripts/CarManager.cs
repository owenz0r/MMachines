using UnityEngine;
using System.Collections;

public class CarManager : MonoBehaviour {

	public Transform[] carArray;

	public void resetAll()
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().Reset();
	}

	public void freezeAll()
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().freeze();
	}

	public void updateAllCheckpoints( Transform checkpoint, int id )
	{
		foreach( Transform car in carArray )
			car.GetComponent<CarController>().setCheckpoint( checkpoint, id );
	}

	public int numAlive()
	{
		int count = 0;
		foreach( Transform car in carArray )
		{
			if( !car.GetComponent<CarController>().isDead() )
				count++;
		}
		return count;
	}

	public Transform getWinner()
	{
		foreach( Transform car in carArray )
		{
			if( !car.GetComponent<CarController>().isDead() )
				return car;
		}
		return null;
	}
}
