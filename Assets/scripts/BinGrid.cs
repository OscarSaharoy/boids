using UnityEngine;
using System.Collections.Generic;

public class BinGrid<T> {

	public LinkedList<T>[] bins;
	public int nBins;
	public int[] offsets;
	
	private float binLength;
	private float sideLength;
	private int binsPerEdge;

	public BinGrid(float inSideLength, float inBinLength) {

		sideLength = inSideLength;
		binLength  = inBinLength;

		binsPerEdge = (int)( sideLength/binLength + 1 );
		nBins = binsPerEdge*binsPerEdge*binsPerEdge;

		bins = new LinkedList<T>[nBins];

		for(int i=0; i<nBins; ++i) {
			bins[i] = new LinkedList<T>();
		}

		offsets = new int[] {0, 1, -1, binsPerEdge, -binsPerEdge, binsPerEdge*binsPerEdge, -binsPerEdge*binsPerEdge};
	}

	public LinkedList<T> this[int i] => bins[i];

	public int indexAtPosition(Vector3 pos) {
		
		Vector3 fromBottomCorner = pos + Vector3.one * sideLength * 0.5f;

		int row    = (int)( fromBottomCorner.x / binLength );
		int layer  = (int)( fromBottomCorner.y / binLength );
		int column = (int)( fromBottomCorner.z / binLength );

		return row + column * binsPerEdge + layer * binsPerEdge*binsPerEdge;
	}
}