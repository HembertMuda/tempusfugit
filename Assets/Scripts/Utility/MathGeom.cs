using UnityEngine;
using System.Collections.Generic;

public class MathGeom
{

	public static bool LinesIntersection (out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
 
		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross (lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross (lineVec3, lineVec2);
 
		float planarFactor = Vector3.Dot (lineVec3, crossVec1and2);
 
		//is coplanar, and not parrallel
		if (Mathf.Abs (planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			float s = Vector3.Dot (crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = linePoint1 + (lineVec1 * s);
			return true;
		}
		else
		{
			intersection = Vector3.zero;
			return false;
		}
	}

	public static float SignedDotProduct (Vector3 vectorA, Vector3 vectorB, Vector3 normal)
	{
 
		Vector3 perpVector;
		float dot;
 
		//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
		perpVector = Vector3.Cross (normal, vectorA);
 
		//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
		dot = Vector3.Dot (perpVector, vectorB);
 
		return dot;
	}

	// Calculate the median vector of two vector
	public static Vector3 MedianVector (Vector3 vectorA, Vector3 vectorB)
	{
		vectorA = vectorA.normalized;
		vectorB = vectorB.normalized;

		Vector3 median = vectorA + vectorB;
		if (SignedDotProduct (vectorA, vectorB, -Vector3.forward) < 0)
			median = -median;

		return median.normalized;
	}

	public static Vector3 Center (List<Transform> pos)
	{
		Vector3 center = Vector3.zero;
		for (int i = 0; i < pos.Count; i++)
		{
			center += pos [i].position;
		}
		center = center / pos.Count;
		return center;
	}

	// If the return value is positive, then rotate to the left. Else,
	// rotate to the right.
	public static float CalcShortestRot (float from, float to)
	{
		// If from or to is a negative, we have to recalculate them.
		// For an example, if from = -45 then from(-45) + 360 = 315.
		if (from < 0)
		{
			from += 360;
		}

		if (to < 0)
		{
			to += 360;
		}

		// Do not rotate if from == to.
		if (from == to ||
		   from == 0 && to == 360 ||
		   from == 360 && to == 0)
		{
			return 0;
		}

		// Pre-calculate left and right.
		float left = (360 - from) + to;
		float right = from - to;
		// If from < to, re-calculate left and right.
		if (from < to)
		{
			if (to > 0)
			{
				left = to - from;
				right = (360 - to) + from;
			}
			else
			{
				left = (360 - to) + from;
				right = to - from;
			}
		}

		// Determine the shortest direction.
		return ((left <= right) ? left : (right * -1));
	}
}
