using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoidCoordinator : MonoBehaviour
{
	public GameObject boidPrefab;
	public BinGrid<Boid> boidBins = new BinGrid<Boid>(30.0f, 3.0f); // segment the space into 3x3x3 bins
	public int nBoids = 100;
	new public Camera camera;

	// planes which define the walls of the box
	public Plane[] walls = new Plane[6] { new Plane( new Vector3(-1, 0, 0), new Vector3( 15,  0,  0) ),
										  new Plane( new Vector3( 1, 0, 0), new Vector3(-15,  0,  0) ),
										  new Plane( new Vector3( 0,-1, 0), new Vector3(  0, 15,  0) ),
										  new Plane( new Vector3( 0, 1, 0), new Vector3(  0,-15,  0) ),
										  new Plane( new Vector3( 0, 0,-1), new Vector3(  0,  0, 15) ),
										  new Plane( new Vector3( 0, 0, 1), new Vector3(  0,  0,-15) ) };

	public void Start() {

		// create number of boids equal to nBoids
		while(--nBoids != 0) {

			// random posititon and rotation for boid
			Vector3    pos = Random.insideUnitSphere * 10.0f;
			Quaternion rot = Quaternion.LookRotation( Random.onUnitSphere, Vector3.up );

			// create new boid and set parent to this
			Boid newBoid = Instantiate( boidPrefab, pos, rot ).GetComponent<Boid>();
			newBoid.transform.parent = transform;

			// set the initial bin that the boid is in
			int binIndex = boidBins.indexAtPosition( pos );
			LinkedListNode<Boid> listNode = boidBins[binIndex].AddLast( newBoid );

			// set variables within the boid
			newBoid.Setup( this, boidBins, binIndex, listNode );
		}
	}
}
