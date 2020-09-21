using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoidCoordinator : MonoBehaviour
{
	public GameObject boidPrefab;
	public BinGrid<Boid> boidBins = new BinGrid<Boid>(30.0f, 3.0f);
	public int nBoids = 100;
	new public Camera camera;

	public Plane[] walls = new Plane[6] { new Plane( new Vector3(-1, 0, 0), new Vector3( 15,  0,  0) ),
										  new Plane( new Vector3( 1, 0, 0), new Vector3(-15,  0,  0) ),
										  new Plane( new Vector3( 0,-1, 0), new Vector3(  0, 15,  0) ),
										  new Plane( new Vector3( 0, 1, 0), new Vector3(  0,-15,  0) ),
										  new Plane( new Vector3( 0, 0,-1), new Vector3(  0,  0, 15) ),
										  new Plane( new Vector3( 0, 0, 1), new Vector3(  0,  0,-15) ) };

	public void Start() {

		while(--nBoids != 0) {

			Vector3    pos = Random.insideUnitSphere * 10.0f;
			Quaternion rot = Quaternion.LookRotation( Random.onUnitSphere, Vector3.up );

			Boid newBoid = Instantiate( boidPrefab, pos, rot ).GetComponent<Boid>();
			newBoid.transform.parent = transform;

			int binIndex = boidBins.indexAtPosition( pos );
			LinkedListNode<Boid> listNode = boidBins[binIndex].AddLast( newBoid );

			newBoid.Setup( this, boidBins, binIndex, listNode );
		}
	}
}
