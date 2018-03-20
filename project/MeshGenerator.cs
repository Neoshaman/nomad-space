using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator{

	/**
	 * fit mesh
	 * 
	 * flipmesh
	 * 
	 * addmesh()
	 * copymesh()
	 * 
	 * positionMesh()
	 * RotateMesh()
	 * ScaleMesh()
	 * 
	 * createcone()
	 * createcube()
	 * createsphere()
	 * createsubesphere
	 * 
	 * lightmesh()
	 * paintmesh()
	 * 
	 * loadanimMesh()
	 * 
	 * meshsize (depth, height, width)
	 * 
	 * meshesIntersect()
	 * 
	 * update normals
	 * 
	 * count surfaces()
	 * get surfaces()
	 * ---------------------
	 * addtriangle()
	 * addvertex()
	 * clear surface()
	 * 
	 * countTriangles
	 * countVertex
	 * 
	 * create surface
	 * 
	 * findSurface
	 * 
	 * paintSurface
	 * 
	 * Trianglevertex
	 * 
	 * VertexColors
	 * VertexCoords
	 * vertexNormal
	 * VertexUV
	 * */

	enum cubeface {nul, top, bottom, back, front, side1, side2};
	public enum axis {top, bottom, back, front, side1, side2};

	public static Mesh createCube()
	{
		int division = 10;
		CombineInstance[] cube  = new CombineInstance[6];

		cube [0].mesh = MeshGenerator.createPlane (new Vector3(0f, .5f), true, MeshGenerator.axis.top	,1,division);
		cube [1].mesh = MeshGenerator.createPlane (new Vector3(0f,-.5f), true, MeshGenerator.axis.bottom,1,division);
		cube [2].mesh = MeshGenerator.createPlane (new Vector3(0f,0f, -.5f), true, MeshGenerator.axis.front	,1,division);
		cube [3].mesh = MeshGenerator.createPlane (new Vector3(0f,0f,  .5f), true, MeshGenerator.axis.back	,1,division);
		cube [4].mesh = MeshGenerator.createPlane (new Vector3( .5f,0f), true, MeshGenerator.axis.side1	,1,division);
		cube [5].mesh = MeshGenerator.createPlane (new Vector3(-.5f,0f), true, MeshGenerator.axis.side2	,1,division);

		return MeshGenerator.MergeMesh (cube);
	}

	//_____________________________________________________________________________
	public static Mesh MergeMesh(CombineInstance[] c)
	{
		Mesh merged = new Mesh();
		int mergedVertLength=0;
		int mergedTriLength=0;

		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		foreach (CombineInstance m in c)
		{
			mergedVertLength += m.mesh.vertices.Length;
			mergedTriLength += m.mesh.triangles.Length;
		}

		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		Vector3[] vert = new Vector3[mergedVertLength];
		Vector2[] UV = new Vector2[mergedVertLength];
		int[] tri = new int[mergedTriLength];

		int i = 0, j = 0;
		int prevVertLength=0;
		int prevTriLength=0;

		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		foreach (CombineInstance m in c)
		{
			foreach (Vector3 v in m.mesh.vertices)
			{
				vert[prevVertLength+j] = m.mesh.vertices[j];
				UV[prevVertLength+j] = m.mesh.uv[j];
				j++;
			}

			j = 0;
			//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

			foreach (int t in m.mesh.triangles)
			{
				tri [prevTriLength + j] = m.mesh.triangles [j]+prevVertLength;
				j++;
			}

			j = 0;
			//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

			prevVertLength += m.mesh.vertices.Length;
			prevTriLength += m.mesh.triangles.Length;

			i++;
		}

		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		merged.vertices = vert;
		merged.uv = UV;
		merged.triangles = tri;

		merged.RecalculateNormals();
		merged.RecalculateBounds();
		merged.RecalculateTangents();

		return merged;
	}

	//_____________________________________________________________________________
	public static Mesh createPlane(Vector3 position, bool center, axis face = axis.top, float dimension = 300f, int division = 10){

		//More parameter -> centered origin,(->?) position, uv shift, direction

		int vertnum = division+1;
		int vertnum2 = vertnum*vertnum;

		float div = (float) division;
		float tileSize = dimension/div;

		Vector2 UVoffset;
		Vector3 centerOffset;


		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//set the center to the middle
		if (center) {
			UVoffset		= new Vector2 (0.5f, 0.5f);
			centerOffset	= new Vector3 (
				                       -div * tileSize * 0.5f,
				                       -div * tileSize * 0.5f,
				                       0.0f
			                       );
		} else {
			UVoffset		= Vector2.zero;
			centerOffset	= Vector3.zero;
		}


		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//init mesh attributes
		Mesh m = new Mesh();
		Vector3[] v = new Vector3[vertnum2];
		Vector2[] uv= new Vector2[vertnum2];
		int[] tri = new int[vertnum2*2*3];


		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//building the mesh
		int i = 0;foreach (Vector3 vertex in v) {
			
			//set the x,y of the vertixces in the grid
			Vector2 p = new Vector2( 
				centerOffset.x + (float)(i % vertnum * tileSize),
				centerOffset.y + (float)(i / vertnum * tileSize)
			);

			//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			//permutate to create different orientation ... ADD DIRECTION
			switch (face) 
			{
			case axis.top:
				v [i] = new Vector3 (
					p.x,
					0,
					p.y
				);
				break;

			case axis.bottom:
				v [i] = new Vector3 (
					p.x,
					0,
					-p.y
				);
				break;

			case axis.front:
				v [i] = new Vector3 (
					p.x,
					p.y,
					0f	
				);
				break;

			case axis.back:
				v [i] = new Vector3 (
					-p.x,
					p.y,
					0f	
			);
				break;

			case axis.side1:
				v [i] = new Vector3 (
					0,
					p.y,
					p.x
				);
				break;

			case axis.side2:
				v [i] = new Vector3 (
					0,
					p.y,
					-p.x
				);
				break;
				//z is heightmap using x,y with cos/sin for fast visualization
				//float z = centerOffset.z + Mathf.Cos(10*p.y)*5 + Mathf.Sin(5*p.x)*5;
			}

			//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			//add the actual position
			v [i] += position;

			//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			// set the uv
			uv[i] = new Vector2(
				UVoffset.x + p.x / tileSize / div,
				UVoffset.y + p.y / tileSize / div
			);

			//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			//compute the indices of the quad and assign it to triangles
			int[] triangleIndices = {i, i + 1, i + vertnum, i + vertnum+1};

			bool testSideLimit = (i % vertnum == division) ; 
			bool testBottomLimit = (i >= vertnum2 - vertnum) ;  
			bool finalTest = !( testSideLimit || testBottomLimit );
			if  ( finalTest )
			{
					int t = i * 6;
					tri [t + 0] = triangleIndices [0];
					tri [t + 1] = triangleIndices [2];
					tri [t + 2] = triangleIndices [1];

					tri [t + 3] = triangleIndices [1];
					tri [t + 4] = triangleIndices [2];
					tri [t + 5] = triangleIndices [3];
			} 

			i++;
		}

		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//Final mesh
		m.vertices = v;
		m.uv = uv;
		m.triangles = tri;

		m.RecalculateNormals();
		m.RecalculateBounds();
		m.RecalculateTangents();

		return m;
	}

	public static void RefreshMesh(Mesh m){
		m.RecalculateNormals();
		m.RecalculateBounds();
		m.RecalculateTangents();}

	public class TerrainCache{
		public Vector3[] v ;
		public Vector2[] uv;
		public int[]	 tri;
	}

	public static TerrainCache CreateTerrainCache( bool center, float dimension = 300f, int division = 10){
		int vertnum = division+1;
		int vertnum2 = vertnum*vertnum;
		float div = (float) division;
		float tileSize = dimension/div;
		Vector2 UVoffset;
		Vector3 centerOffset;
		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//set the center to the middle
		if (center) {
			UVoffset		= new Vector2 (0.5f, 0.5f);
			centerOffset	= new Vector3 (
				-div * tileSize * 0.5f,
				-div * tileSize * 0.5f,
				0.0f);}
		else {
			UVoffset		= Vector2.zero;
			centerOffset	= Vector3.zero;}
		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//init mesh attributes
		Vector3[] v = new Vector3[vertnum2];
		//Vector2[] uv= new Vector2[vertnum2];
		//int[]	 tri = new int[vertnum2*2*3];
		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//building the data
		int i = 0;while (i < vertnum2) {
			//set the x,y of the vertixces in the grid
			Vector3 p = new Vector3( 
			centerOffset.x + (float)(i % vertnum * tileSize),
				centerOffset.y + (float)(i / vertnum * tileSize));
			
			v[i]=p;
			////''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			//// set the uv
			//uv[i] = new Vector2(
			//UVoffset.x + p.x / tileSize / div,
			//UVoffset.y + p.y / tileSize / div);
			////''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			////compute the indices of the quad and assign it to triangles
			//int[] triangleIndices = {i, i + 1, i + vertnum, i + vertnum+1};
			//bool testSideLimit = (i % vertnum == division) ; 
			//bool testBottomLimit = (i >= vertnum2 - vertnum) ;  
			//bool finalTest = !( testSideLimit || testBottomLimit );
			//if  ( finalTest ){
			//	int t = i * 6;
			//	tri [t + 0] = triangleIndices [0];
			//	tri [t + 1] = triangleIndices [2];
			//	tri [t + 2] = triangleIndices [1];

			//	tri [t + 3] = triangleIndices [1];
			//	tri [t + 4] = triangleIndices [2];
			//	tri [t + 5] = triangleIndices [3];} 
			i++;}
		//''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		//Final
		TerrainCache m = new TerrainCache();
		m.v = v;
		//m.uv = uv;
		//m.tri = tri;
		return m;
	}
	
	
	
	
	
	public static void UpdateTerrainData(){}
	
	public static Vector3 UpdateData(axis face, Vector2 p){
		
		Vector3 v = Vector3.zero;
		
		switch (face) 
		{
		case axis.top:
			v = new Vector3 (
				p.x,
				0,
				p.y
			);
			break;

		case axis.bottom:
			v = new Vector3 (
				p.x,
				0,
				-p.y
			);
			break;

		case axis.front:
			v = new Vector3 (
				p.x,
				p.y,
				0f	
			);
			break;

		case axis.back:
			v = new Vector3 (
				-p.x,
				p.y,
				0f	
			);
			break;

		case axis.side1:
			v = new Vector3 (
				0,
				p.y,
				p.x
			);
			break;

		case axis.side2:
			v = new Vector3 (
				0,
				p.y,
				-p.x
			);
			break;
			//z is heightmap using x,y with cos/sin for fast visualization
			//float z = centerOffset.z + Mathf.Cos(10*p.y)*5 + Mathf.Sin(5*p.x)*5;
		}
		//Debug.Log (p);
		return v;


	}







	//_____________________________________________________________________________
	public static axis findCubeFace(Vector3 v)
	{
		float ax = Mathf.Abs (v.x);
		float ay = Mathf.Abs (v.y);
		float az = Mathf.Abs (v.z);

		if (ax > ay && ax > az) {
			return (v.x > 0) ? axis.side1	: axis.side2;
		} else if (ay > az) {
			return (v.y > 0) ? axis.top		: axis.bottom;
		} else {
			return (v.z > 0) ? axis.back	: axis.front;
		}

		//		;side1  : N>top  E>back  W>front S>bottom /side2
		//		;side2  : N>top  E>front W>back  S>bottom /side1
		//
		//		;front  : N>top  E>side1 W>side2 S>bottom /back
		//		;back   : N>top  E>side2 W>side1 S>bottom /front
		//
		//		;top    : N>back E>side1 W>side2 S>front  /bottom
		//		;bottom : N>back E>side1 W>side2 S>front  /top


		//return cubeface.nul;
	}

	//_____________________________________________________________________________
	public static Vector3 SQRspherized(Vector3 v)
	{
		Vector3 v2 = new Vector3 (v.x*v.x, v.y*v.y, v.z*v.z);
		Vector3 a = new Vector3 (v2.x/2f, v2.y/2f, v2.z/2f);
		Vector3 s = new Vector3 (
			v.x * Mathf.Sqrt (1f - a.y-a.z + v2.y*v2.z / 3f),
			v.y * Mathf.Sqrt (1f - a.x-a.z + v2.x*v2.z / 3f),
			v.z * Mathf.Sqrt (1f - a.x-a.y + v2.x*v2.y / 3f)
		);
		return s;
	}
	//_____________________________________________________________________________
	public static Mesh CreateCubeSphere()
	{
		Mesh cube = createCube ();
		Vector3[] spherized = new Vector3[cube.vertices.Length];

		int i=0; foreach (Vector3 v in cube.vertices) 
		{
			spherized [i] = MeshGenerator.SQRspherized(v*2f);

			//Debug.Log( );
			i++;
		}

		cube.vertices = spherized;

		return cube;
	}
	 

	//_____________________________________________________________________________
	static Vector3 SQRsphereToCubeHelper(float xi, float yi)
	{
		float x2, y2,
		b, c,
		x,y,z; // pos on cube

		float xxi = xi * xi;
		float yyi = yi * yi;

		b = ( xxi - yyi - 1.5f);
		c = 3f * yyi;

		y2 = -b + Mathf.Sqrt( b*b - 2f*c );
		x2 = 6f * xxi / ( 3f - y2 );

		x = (xi > 0f) ? Mathf.Sqrt( x2 ) : -Mathf.Sqrt( x2 );
		y = (yi > 0f) ? Mathf.Sqrt( y2 ) : -Mathf.Sqrt( y2 );
		z = +1f;

		return new Vector3 (x, y, z);
	}

	//_____________________________________________________________________________
	public static Vector3 SQRsphereToCube(Vector3 v)
	{
		float asx = Mathf.Abs (v.x);
		float asy = Mathf.Abs (v.y);
		float asz = Mathf.Abs (v.z);

		Vector3 result;

		if (asx > asy && asx > asz) {
			// case |sx| is biggest, and sx>0.
			// Face is: +X.
			// Position on the cube: x = +1.

			result = SQRsphereToCubeHelper(v.y, v.z);//x
			result = new Vector3 (result.z, result.x, result.y);
			return result;

		} else if (asy > asz) {//+Y
			
			result = SQRsphereToCubeHelper(v.x,v.z);//y
			result = new Vector3 (result.x, result.z, result.y);
			return result;

		} else {//+Z
			
			result = SQRsphereToCubeHelper(v.x,v.y);//z
			result = new Vector3 (result.x, result.y, result.z);
			return result;

		}	
	}
	//_____________________________________________________________________________
	const float  isqrt2 = 0.70710676908493042f;
	
	
	public static Vector3 cubify( Vector3 s)
		{
			float xx2 = s.x * s.x * 2.0f;
			float yy2 = s.y * s.y * 2.0f;

			Vector2 v = new Vector2(xx2 - yy2, yy2 - xx2);			
			
			float ii = v.y - 3.0f;
			ii *= ii;

			float isqrt = -Mathf.Sqrt(ii - 12.0f * xx2) + 3.0f;
			
			v = new Vector2( 
				Mathf.Sqrt(Mathf.Abs( v.x + isqrt) ),  
				Mathf.Sqrt(Mathf.Abs( v.y + isqrt) ));//v corruption
			v *= isqrt2;
			

			return  new Vector3(
				Mathf.Sign(s.x) * v.x, 
				Mathf.Sign(s.y) * v.y, 
				Mathf.Sign(s.z) * 1.0f);
		}

	
	public static Vector3 sphere2cube( Vector3 sphere)
	{
		Vector3 f = new Vector3 ( 
			Mathf.Abs(sphere.x),
			Mathf.Abs(sphere.y),
			Mathf.Abs(sphere.z));

		bool a = f.y >= f.x && f.y >= f.z;
		bool b = f.x >= f.z;

		 
		Vector3 result;
		
		if (a)
		{ 
			result = cubify( new Vector3(sphere.x,sphere.z,sphere.y));
			result = new Vector3(result.x,result.z,result.y);
		}
		else if (b)
		{
			result = cubify( new Vector3(sphere.y,sphere.z,sphere.x));
			result = new Vector3(result.z,result.x,result.y);
		}
		else
		{
			result = cubify(sphere);
		}
		
		return result;
	}
	//_____________________________________________________________________________
}
//-----------------------------------------------------------------------------
//	;naive method with deformation around pole
//	;sx = normalizeX3D(x,y,z)*radius/2
//	;sy = normalizeY3D(x,y,z)*radius/2
//	;sz = normalizeZ3D(x,y,z)*radius/2

//  Compensation for pole deformation
//	x2# = x*x
//	y2# = y*y
//	z2# = z*z
//
//	a# = (x2 / 2.0)
//	b# = (y2 / 2.0)
//	c# = (z2 / 2.0)
//
//	sx# =  Sqr(1.0 - b - c + ( (y2 * z2) / 3.0 ) ) * x / 2
//	sy# =  Sqr(1.0 - c - a + ( (x2 * z2) / 3.0 ) ) * y / 2
//	sz# =  Sqr(1.0 - a - b + ( (x2 * y2) / 3.0 ) ) * z / 2

//http://stackoverflow.com/questions/2656899/mapping-a-sphere-to-a-cube
//the original article say it's having precision problem at planet scales, even though they use double to compute it!
/*
		Function SphereToCube.point3D(px#,py#,pz#)

		x# = px
			y# = py
			z# = pz

			;absolute value of coordinate
		fx# = Abs(x)
			fy# = Abs(y)
			fz# = Abs(z)

			inverseSQRT2# = 0.70710676908493042

			If fy => fx And fy => fz Then
			a2# = x*x*2
			b2# = z*z*2
			inner# = -a2+b2-3
			innerSQRT# = -Sqr((inner*inner) - 12*a2)

			If x = 0 Or x =-0 Then
			px = 0
			Else
			px = Sqr(innerSQRT +a2 - b2 +3)* inverseSQRT2 
			EndIf

			If z = 0 Or z =-0 Then
			pz = 0
			Else
			pz = Sqr(innerSQRT -a2 + b2 +3)* inverseSQRT2 
			EndIf

			If px > 1 Then px = 1
			If pz > 1 Then pz = 1

			If x < 0 Then px = -px
			If z < 0 Then pz = -pz

			If y > 0 Then
			py = 1;top face
		Else
		py = -1;bottom face
		EndIf
		ElseIf fx => fy And fx => fz Then
		a2# = y*y*2
			b2# = z*z*2
			inner# = -a2+b2-3
			innerSQRT# = -Sqr((inner*inner) - 12*a2)

			If y = 0 Or y =-0 Then
			py = 0
			Else
			py = Sqr(innerSQRT +a2 - b2 +3)* inverseSQRT2 
			EndIf

			If z = 0 Or z =-0 Then
			pz = 0
			Else
			pz = Sqr(innerSQRT -a2 + b2 +3)* inverseSQRT2 
			EndIf

			If py > 1 Then py = 1
			If pz > 1 Then pz = 1

			If y < 0 Then py = -py
			If z < 0 Then pz = -pz

			If x > 0 Then
			px = 1;right face
		Else
		px = -1;left face
		EndIf
		Else
		a2# = x*x*2
			b2# = y*y*2
			inner# = -a2+b2-3
			innerSQRT# = -Sqr((inner*inner) - 12*a2)

			If x = 0 Or x =-0 Then
			px = 0
			Else
			px = Sqr(innerSQRT +a2 - b2 +3)* inverseSQRT2 
			EndIf

			If y = 0 Or y =-0 Then
			py = 0
			Else
			py = Sqr(innerSQRT -a2 + b2 +3)* inverseSQRT2 
			EndIf

			If px > 1 Then px = 1
			If py > 1 Then py = 1

			If x < 0 Then px = -px
			If y < 0 Then py = -py

			If z > 0 Then
			pz =  1;front face
		Else
		pz = -1;back face
		EndIf
		EndIf

		position.point3d = New point3d
			position\x = px
			position\y = py
			position\z = pz

			Return position

			End Function
			*/





//function find_pos_on_cube( float sx, sy, sz )
//{
//
//	// case |sx| is biggest, and sx>0.
//	// Face is: +X.
//	// Position on the cube: x = +1.
//	// 
//
//	float syy = sy*sy;
//	float szz = sz*sz;
//
//	float b = (syy-szz-1.5);
//	float c = 3*szz;
//
//	float zz = -b + sqrt( b*b - 2*c);
//	float yy = 6*syy/(3-zz);
//
//	float x,y,z; // pos on cube
//	x = +1;
//	z = (sz>0)? sqrt(zz) : -sqrt(zz);
//	y = (sy>0)? sqrt(yy) : -sqrt(yy);
//
//}


/** 
		Function cubeface$(x,y,z)

		ax = Abs(x)
		ay = Abs(y)
		az = Abs(z)

		If ax > ay And ax > az Then
			If x > 0 Then 
				Return "side1";
			Else
				Return "side2"
			EndIf
			
		Else If ay > az Then
			If y > 0 Then 
				Return "top";
			Else
				Return "bottom"
			EndIf

		Else
			If z > 0 Then 
				Return "back";
			Else
				Return "front"
			EndIf

		EndIf

		;side1  : N>top  E>back  W>front S>bottom /side2
		;side2  : N>top  E>front W>back  S>bottom /side1

		;front  : N>top  E>side1 W>side2 S>bottom /back
		;back   : N>top  E>side2 W>side1 S>bottom /front

		;top    : N>back E>side1 W>side2 S>front  /bottom
		;bottom : N>back E>side1 W>side2 S>front  /top

		End Function
		**/


/*
dimension# = 290

	size_x = 3 ;n +1
size_y = 3
	; row enumeration

size_total = size_x * size_y

	plane = CreateMesh()
	surface = CreateSurface (plane)


	Tilesize# = dimension /size_x;10; x and y

;axis selection?

offset_x# = -( (size_x-1) * tilesize ) / 2
	offset_y# = -( (size_y-1) * tilesize ) / 2
	offset_z# = 0

	offset_u# = 0.5
	offset_v# = 0.5

	*/


	/*
	For i=0 To size_total 

	x# = offset_x + ( i Mod size_x )	*tilesize
	y# = offset_y + ( i / size_x )		*tilesize
	z# = offset_z + Cos(10*y)*5 + Sin(5*x)*5

	u# = offset_u + x / tilesize / ( size_x-1 )
	v# = offset_v + y / tilesize / ( size_x-1 )
	w# = 0

	AddVertex (surface, x,y,z,  u,v,w)

	w1 = i
	w2 = i+1
	w3 = i+size_x +1
	w4 = i+size_x 

	If i Mod size_x = size_x-1 Then Goto skip
	If i => size_total-size_x Then Goto skip

	AddTriangle (surface, w1,w3,w2)
	AddTriangle (surface, w1,w4,w3)

	.skip
	Next


	;Print CountTriangles (surface):WaitKey()

UpdateNormals plane

*/




