using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Boid : MonoBehaviour {

	private BoidCoordinator boidCoordinator;
	private BinGrid<Boid> boidBins;
	private LinkedListNode<Boid> listNode;
	private int binIndex;

	// functions to make position constraint within the box easier
	private float mod(float a, float b) => a - b * Mathf.Floor(a / b);
	private float loop(float t, float min, float max) => mod(t - min, max - min) + min;
	private float loop30(float t) => loop(t, -15.0f, 15.0f);

	public Vector3 pos { get => transform.position;
						 set => transform.position = value; }

	private Vector3 vel;

	private Quaternion rot { get => transform.rotation;
							 set => transform.rotation = value;}

	//private float attract(float x) => x > 10.0f ? 0.0f : -7.35f + 4.449f*x - 0.657f*x*x + 0.0286f*x*x*x; * 0.003f
	private float attract(float x) => x > 10.0f ? 0.0f : -20f + 8f*x - 0.9f*x*x + 0.03f*x*x*x;

	public void Setup(BoidCoordinator inBoidCoordinator, BinGrid<Boid> inBoidBins, int inBinIndex, LinkedListNode<Boid> inListNode) {
		
		// just sets certain variables in the boid 
		boidCoordinator = inBoidCoordinator;
		boidBins = inBoidBins;
		listNode = inListNode;
		binIndex = inBinIndex;

		// initial random velocity
		vel = Random.onUnitSphere * 10.0f;
	}

	public void FixedUpdate() {

		// update the boid's velocity based on the boids around it

		uint c = 0; // counts how many surrounding boids we've looked at

		// loop over the nearby bins in the BinGrid
		foreach(int offset in boidBins.offsets) {

			int i = binIndex + offset;

			// ignore indices that are outside the BinGrid
			if(i < 0 || i >= boidBins.nBins) continue;

			// loop over the boids in those bins
			foreach(Boid boid in boidBins[i]) {

				// get displacement vector of the other boid
				Vector3 dis = boid.pos - pos;

				// increment velocity based on boid spacing and velocity alignment
				vel += dis.normalized * attract( dis.magnitude ) * 0.003f;
				vel += boid.vel * 0.003f;

				// if we've considered more than 10 surrounding boids already then end the loop
				if(++c > 10) break;
			}

			// break out of lower loop if we've looked at 10 surrounding boids
			if(c > 10) break;
		}

		// create a repulsion force from the walls approaching infinity if the boid is at the wall
		foreach(Plane wall in boidCoordinator.walls) {

			float dist = wall.GetDistanceToPoint(pos);
			vel += 0.2f/dist * wall.normal;
		}

		// fix the boid's speed to be 5m/s and update the position
		vel = vel.normalized * 5f;

		pos += vel * Time.deltaTime;


		// constrain the position to be within a 30x30x30 cube
		pos = new Vector3(loop30(pos.x),
						  loop30(pos.y),
						  loop30(pos.z));
		
		// set te rotation to be aligned with the velocity vector
		rot = Quaternion.LookRotation( vel );


		// update which bin the boid is in
		int newBinIndex = boidBins.indexAtPosition( pos );

		// if the boid has crossed into a new bin then remove it's listNode from the original bin
		// and make a new one for it at the end of the new bin's linked list
		if(newBinIndex != binIndex) {

			boidBins[binIndex].Remove( listNode );
			listNode = boidBins[newBinIndex].AddLast( this );
			binIndex = newBinIndex;
		}
	}
}
