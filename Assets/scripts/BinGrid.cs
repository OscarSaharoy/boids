using UnityEngine;
using System.Collections.Generic;

public class BinGrid<T> {

	// class representing a cube array of bins which segment space into many cubes
	// centered on the origin

	public LinkedList<T>[] bins;
	public int nBins;
	public int[] offsets;
	
	private float binLength;
	private float sideLength;
	private int binsPerEdge;

	public BinGrid(float inSideLength, float inBinLength) {

		sideLength = inSideLength;
		binLength  = inBinLength;

		// calculate the number of bins needed to cover a cube of sideLength
		binsPerEdge = (int)( sideLength/binLength + 1 );
		nBins = binsPerEdge*binsPerEdge*binsPerEdge;

		// populate the bins array with linked lists of T
		bins = new LinkedList<T>[nBins];

		for(int i=0; i<nBins; ++i) {
			bins[i] = new LinkedList<T>();
		}

		// these integer offsets are what needs to be added to an index of a bin to get to the 6 bordering bins
		offsets = new int[] {0, 1, -1, binsPerEdge, -binsPerEdge, binsPerEdge*binsPerEdge, -binsPerEdge*binsPerEdge};
	}

	// allows the BinGrid object to be indexed directly returning the bin at that index
	public LinkedList<T> this[int i] => bins[i];

	// calculate the index of the bin that pos is within
	public int indexAtPosition(Vector3 pos) {
		
		Vector3 fromBottomCorner = pos + Vector3.one * sideLength * 0.5f;

		int row    = (int)( fromBottomCorner.x / binLength );
		int layer  = (int)( fromBottomCorner.y / binLength );
		int column = (int)( fromBottomCorner.z / binLength );

		return row + column * binsPerEdge + layer * binsPerEdge*binsPerEdge;
	}
}