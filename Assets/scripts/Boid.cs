using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Boid : MonoBehaviour {

	private BoidCoordinator boidCoordinator;
	private BinGrid<Boid> boidBins;
	private LinkedListNode<Boid> listNode;
	private int binIndex;

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
		
		boidCoordinator = inBoidCoordinator;
		boidBins = inBoidBins;
		listNode = inListNode;
		binIndex = inBinIndex;

		vel = Random.onUnitSphere * 10.0f;
	}

	public void FixedUpdate() {

		uint c = 0;
		foreach(int offset in boidBins.offsets) {

			int i = binIndex + offset;
			if(i < 0 || i >= boidBins.nBins) continue;

			foreach(Boid boid in boidBins[i]) {

				Vector3 dis = boid.pos - pos;
				vel += dis.normalized * attract( dis.magnitude ) * 0.003f;
				vel += boid.vel * 0.003f;

				if(++c > 10) break;
			}

			if(c > 10) break;
		}

		foreach(Plane wall in boidCoordinator.walls) {

			float dist = wall.GetDistanceToPoint(pos);
			vel += 0.2f/dist * wall.normal;
		}

		vel = vel.normalized * 5f;

		pos += vel * Time.deltaTime;


		pos = new Vector3(loop30(pos.x),
						  loop30(pos.y),
						  loop30(pos.z));
		
		rot = Quaternion.LookRotation( vel );


		int newBinIndex = boidBins.indexAtPosition( pos );

		if(newBinIndex != binIndex) {

			boidBins[binIndex].Remove( listNode );
			listNode = boidBins[newBinIndex].AddLast( this );
			binIndex = newBinIndex;
		}
	}
}
