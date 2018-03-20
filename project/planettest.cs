using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planettest : MonoBehaviour {


	//get player()
	//get player position()
	//get player distance to planet()
	//get player distance to ground0()
	//get player distance to atmosphere()
	//get player geosphere face()
	//get player current tiles()
	//compute horizon size()

	//generate planet proxy()
	//update planet tiling()
	//set tile size()
	//set tile resolution
	//set tile array()
	//generate tiles()
	//
	//tile array coordinate
	//tile sphere projected coordinates

	//get global area size()
	//set planet radius in area()
	//set planet position()


	//https://forums.tigsource.com/index.php?topic=58921.0
	//Cube sphere coordinate
	
	//ground terrain + player : should certainly be a static data

	//create planet proxy
	//planet: radius (nombre de tile(planet specific) * taille d'une tile (project specific), 
	//loop:---------------------> responsability of this class?
	//	get player position 
	//	if position changed:
	//		generate tile
	//		generate lod
	
	
	
	
	static int Tilesize;
	
	
	//data
	int hashes;
	float worldsize;//radius
	float tilesize;
	float halftile;
	
	int radius;
	int atmosRadius;
	int maxAltitude;

	int type;

	int groundradius = 2;
	int groundsize;



	Mesh plane;
	Mesh terrain;
	MeshGenerator.TerrainCache Tcache; 
	
	public GameObject terrainTile;
	public GameObject player;
	
	GameObject[,] groundGrid;
	Mesh[,] meshGrid;
	
	
	public Vector3 playerRelativeToPlanetPosition;
	public Vector3 playerSurfacePosition;
	Vector3 terrainPosition;
	MeshGenerator.axis terrainFacing;
	
	
	//_____________________________________________________________________________
	void GetPlayerPosition(){
		playerRelativeToPlanetPosition = this.player.transform.position - this.gameObject.transform.position;
		playerSurfacePosition = playerRelativeToPlanetPosition.normalized;}
			
	void OffsetTile(){
		Vector3 offset = this.terrainTile.transform.position - this.gameObject.transform.position;
		Vector3[] Varray = new Vector3[this.terrain.vertices.Length];//temp array to iterate
		Vector3[] mvert = this.terrain.vertices;					;//temp vertice array to read
		for (int i = 0; this.terrain.vertices.Length > i; i++){			//loop all vertices
			Varray[i] = MeshGenerator.SQRspherized(mvert[i]+offset )-offset;//project on sphere
		}
		this.terrain.vertices = Varray;}
		
	void Offsetgrid(){
		for (int i = 0; i< this.groundGrid.Length;i++){
			int ax = i%groundsize;
			int ay = i/groundsize;
			
			Vector3 offset = this.groundGrid[ax,ay].transform.position - this.gameObject.transform.position;
			Debug.Log( this.gameObject.transform.position );
			
			Vector3[] Varray = new Vector3[this.meshGrid[ax,ay].vertices.Length];//temp array to iterate
			Vector3[] mvert = this.meshGrid[ax,ay].vertices;					;//temp vertice array to read
			for (int iv = 0; this.meshGrid[ax,ay].vertices.Length > iv; iv++){			//loop all vertices
				Varray[iv] = MeshGenerator.SQRspherized(mvert[iv]+offset )-offset;//project on sphere
			}
			this.meshGrid[ax,ay].vertices = Varray;}} 
		
	void FindSurfacePosition(){
		this.terrainPosition = HashSpherePosition(playerSurfacePosition);
		this.terrainFacing = MeshGenerator.findCubeFace(this.terrainPosition);		
		this.terrainTile.transform.position = this.terrainPosition;}
		
	void updateTerrainPosition(){
		for (int i = 0; i<this.groundsize*groundsize  ;i++){
			int ax = i%groundsize;
			int ay = i/groundsize;
			Vector3 Newposition = this.groundGrid[ay,ax].transform.position;
			Newposition.x =  terrainTile.transform.position.x + ((float)ax-groundradius)/this.hashes;
			Newposition.y =  terrainTile.transform.position.y + ((float)ay-groundradius)/this.hashes;
			this.groundGrid[ay,ax].transform.position = Newposition;}}
			
	void initTile(){
		for (int i = 0; i<groundsize*groundsize  ;i++){
			int ax = i%groundsize;
			int ay = i/groundsize;
			this.groundGrid[ay,ax] = Instantiate(this.terrainTile);
		}}
			
	void updatetile(){
		for (int i = 0; i< this.groundGrid.Length;i++){
			int ax = i%groundsize;
			int ay = i/groundsize;
			
			Vector3[] tc = this.meshGrid[ax,ay].vertices;											//cache the vertices of terrain
			for (int itc = 0; tc.Length > itc ;itc++){										//loop the cache
				tc[itc] = MeshGenerator.UpdateData(this.terrainFacing,this.Tcache.v[itc]);	//using tcache update the plane
			}
			this.meshGrid[ax,ay].vertices = tc;}}
			
	void updateGrid(){
		Vector3[] tc = this.terrain.vertices;											//cache the vertices of terrain
		for (int itc = 0; tc.Length > itc ;itc++){										//loop the cache
			tc[itc] = MeshGenerator.UpdateData(this.terrainFacing,this.Tcache.v[itc]);	//using tcache update the plane
		}
		this.terrain.vertices = tc;}
		
	void generateTilemesh(){
		//mesh --> note: generate tcache first, use tcache to create tri and uv, reuse data for each tile
		this.Tcache = MeshGenerator.CreateTerrainCache(true,this.tilesize,this.hashes*2);						//create a cache of plane position
		this.terrain = MeshGenerator.createPlane(Vector3.zero,true, this.terrainFacing,this.tilesize,this.hashes*2); //generate mesh plane put it in member mesh
		for (int  i = 0 ; i< this.groundGrid.Length;i++){
			int ax = i%groundsize;
			int ay = i/groundsize;
			this.meshGrid[ax,ay] = MeshGenerator.createPlane(Vector3.zero,true, this.terrainFacing,this.tilesize,this.hashes*2);
			this.groundGrid[ax,ay].gameObject.GetComponent<MeshFilter> ().mesh =  this.meshGrid[ax,ay];}	}
	
	//_____________________________________________________________________________
	void CreateProxyPlanet(){
		this.plane = MeshGenerator.CreateCubeSphere();
		//this.cubeMesh = MeshGenerator.CreateCubeSphere();
		MeshGenerator.RefreshMesh (this.plane);
		this.gameObject.GetComponent<MeshFilter> ().mesh = this.plane;}
		
		
	void GenerateTile(){
		GetPlayerPosition();
		FindSurfacePosition();
		initTile();
		updateTerrainPosition();
		//mesh
		generateTilemesh();//generate tcache
		OffsetTile();																							//to sphere
		this.terrainTile.GetComponent<MeshFilter> ().mesh = this.terrain;	}									//set the mesh renderer

	
	void UpdateTerrain(){
		GetPlayerPosition();
		FindSurfacePosition();
		updateTerrainPosition();
		//mesh
		updatetile();	updateGrid();//use tcache
		OffsetTile();	Offsetgrid();														//to sphere
		this.terrainTile.GetComponent<MeshFilter> ().mesh = this.terrain;}		//set the mesh

	
	void InitData(){
		this.hashes = 5;
		this.worldsize = 1f;//radius
		this.tilesize = worldsize / hashes;
		this.halftile = tilesize / 2f;
		this.groundsize	= this.groundradius * 2 +1;
		this.groundGrid = new GameObject[groundsize,groundsize];
		this.meshGrid = new Mesh[groundsize,groundsize];}
	
	
	
	//____________________________________________________________________________
	// flow control
	void Awake (){
		InitData();
		CreateProxyPlanet();
		GenerateTile();}
		
	void Update(){
		UpdateTerrain();}
		
		
		
	//_____________________________________________________________________________
	// utils
	Vector3 SnapRound(Vector3 v){
		return new Vector3(
			Mathf.Floor(v.x),
			Mathf.Floor(v.y),
			Mathf.Floor(v.z));}
			
	Vector3 mulVector(Vector3 a,Vector3 b){
		return new Vector3(a.x * b.x,a.y * b.y,a.z * b.z);
	}
			
	Vector3 VectorPlane(MeshGenerator.axis a){
		if (a == MeshGenerator.axis.back ||  a == MeshGenerator.axis.front) return new Vector3(1,1,0);
		if (a == MeshGenerator.axis.bottom ||  a == MeshGenerator.axis.top) return new Vector3(1,0,1);
		return new Vector3(0,1,1);}
	
	Vector2 GroundPlane(MeshGenerator.axis a, Vector3 v) {
		if (a == MeshGenerator.axis.back	||  a == MeshGenerator.axis.front) return new Vector2 (v.x,v.y);//Vector3(1,1,0);
		if (a == MeshGenerator.axis.bottom	||  a == MeshGenerator.axis.top) return new Vector2 (v.x,v.z);//Vector3(1,0,1);
		return new Vector2 (v.y,v.z);//Vector3(0,1,1);
	}
	
	Vector3 PlaneToCubeFace(MeshGenerator.axis a, Vector2 plane){
		if (a == MeshGenerator.axis.front ) return new Vector3(plane.x,plane.y, 1);
		if (a == MeshGenerator.axis.back  ) return new Vector3(plane.x,plane.y,-1);
		if (a == MeshGenerator.axis.top   ) return new Vector3(plane.x, 1,plane.y);
		if (a == MeshGenerator.axis.bottom) return new Vector3(plane.x,-1,plane.y);
		if (a == MeshGenerator.axis.side2 ) return new Vector3(-1,plane.x,plane.y);
		return new Vector3(1,plane.x,plane.y);}
	
	Vector3 HashSpherePosition(Vector3 v){
		Vector3 cubed = MeshGenerator.sphere2cube (v);
		return VectorPlane(MeshGenerator.findCubeFace(cubed))
			* halftile + SnapRound( cubed * hashes ) / hashes;}
		
		
	//void DrawsCubeMesh(){
	//	Vector3[] arrayVertices = new Vector3[this.cubeMesh.vertices.Length];
	//	int i = 0; foreach (Vector3 v in this.plane.vertices) {
	//		arrayVertices[i] = MeshGenerator.sphere2cube (v);i++;}
	//	this.cubeMesh.vertices = arrayVertices;
	//	MeshGenerator.RefreshMesh (this.cubeMesh);}
		
		
		
		
		
		
		
		
	void plaincoordinate(Vector3 pos,MeshGenerator.axis face, int hash){//parameter need replace with member

		Vector3 facepose  = new Vector3(
			Mathf.Floor(pos.x*hash)+hash,
			Mathf.Floor(pos.y*hash)+hash,
			Mathf.Floor(pos.z*hash)+hash);
			
		Vector3 faceplane = mulVector( VectorPlane(face),facepose);	//get the coordinate to a plane
		Vector2 groundPosition = GroundPlane(face,faceplane);		//turn coordinate into 2d 
		
		int groundsize	= this.groundradius * 2 +1;//is a member, obsolete
		int terrainsize = this.hashes * 2; //should be member?
		if (groundsize > terrainsize){groundsize = (terrainsize/ 2)-1;}//error handling, rescale to terrainsize
		
		//find overlap
		Vector2 tileFaceSize0 = Vector2.zero;
		Vector2 tileFacesize1 = new Vector2(groundsize,groundsize);
		
		int leftbound	= -this.groundradius + (int)groundPosition.x;
		int rightbound	=  this.groundradius + (int)groundPosition.x;
		int upperbound	=  this.groundradius + (int)groundPosition.y;
		int bottombound	= -this.groundradius + (int)groundPosition.y;
		
		//iterate grid base on bound, assign tile to face base on coordinate check
		
		//---------------------------------
		//old way of figure out zone cut around limit
		//horizontal bound
		if (leftbound < 0){//Debug.Log("l<0");
			tileFaceSize0.x = Mathf.Abs((float)leftbound);
			tileFacesize1.x = groundsize - tileFaceSize0.x;}
		else if (rightbound > terrainsize-1){//Debug.Log("r>s");
			tileFaceSize0.x = (float)(rightbound - terrainsize+1);
			tileFacesize1.x = (float) groundsize - tileFaceSize0.x;}
		//vertical bound
		if (upperbound > terrainsize-1){//Debug.Log("u>s");
			tileFaceSize0.y = (float)(upperbound - terrainsize+1);
			tileFacesize1.y = (float) groundsize - tileFaceSize0.y;}
		else if (bottombound < 0){//Debug.Log("b<0");
			tileFaceSize0.y = Mathf.Abs((float)bottombound);
			tileFacesize1.y = groundsize - tileFaceSize0.y;}
	
		//int Hbound;
		//if leftbound then Hbound = 1
		//else if rightbound then Hbound = -1
		//else Hbound = 0
	
		//int Vbound;
		//if bottombound then Vbound = 1
		//else if upperbound then Vbound = -1
		//else Vbound = 0
	
		//return new vector2 (Hbound, Vbound)
	
		//return new Vector4(tileFacesize1.x,tileFacesize1.y, tileFaceSize0.x,tileFaceSize0.y);
		
		//create terrain
		//	 face 0		->	x1,y1
		//if face 1		->	x0,y1
		//if face 2		->	x1,y0
		//if disable	->	x0,y0
			
		//Debug.Log("size: "+hashes*2);
		//Debug.Log("right: " + rightbound + " center: " + groundPosition + " left: " +leftbound);
		//Debug.Log("up: " + upperbound + " center: " + groundPosition + " down: " + bottombound);
		//Debug.Log(facepose );
		
		//Debug.Log(
		//	"   x0: " + tileFaceSize0.x + " - y0: " + tileFaceSize0.y +
		//	" ----- x1: " + tileFacesize1.x + " - y1: " + tileFacesize1.y// +
		//);
		
		//new strategy:
		//iterate all tiles, if coordinate outside, assign face accordingly
	}	
		
	void tiletesting(){
		circular2dArray tiletest = new circular2dArray();
		tiletest.initGrid(10,10);
		tiletest.populateGrid();
		tiletest.ScanGrid();
		tiletest.setOffset(tiletest.grid.GetLength(0)/2,tiletest.grid.GetLength(1)/2);
		tiletest.ReadGrid();
	}
		
	class tile{
	//tile array (tcache)
		//object tile
		//position
		//facing
		//disable
		//mesh
		Vector3 position;
		MeshGenerator.axis facing;
		bool isdisable;
		Mesh mesh;
	}
	
	class circular2dArray{
		public int[,] grid;
		
		int offsetx;
		int offsety;
		
		public void initGrid(int sizex,int sizey){this.grid = new int[sizex,sizey];}
		
		public void populateGrid(){
			int i = 0;while (i < grid.GetLength(0)){
				int j = 0;while (j < grid.GetLength(1)){
					grid[i,j] = i*grid.GetLength(0)+j;
					j++;}
				i++;}}
				
		public void setCell(int x, int y, int v){
			x = (x + offsetx) % grid.GetLength(0);
			y = (y + offsety) % grid.GetLength(1);
			grid[x,y] = v;}
			
		public int getCell(int x, int y){
			x = (x + offsetx) % grid.GetLength(0);
			y = (y + offsety) % grid.GetLength(1);
			return this.grid[x,y];}
			
		public void setOffset(int x, int y){offsetx = x;offsety = y;}
		public void getOffset(int x, int y){Debug.Log("> "+offsetx+" : "+offsety);}

		//shiftgridup
		//shiftgriddown
		//shiftgridright
		//shiftgridleft
		
		//updateshiftgridup
		//updateshiftgriddown
		//updateshiftgridright
		//updateshiftgridleft
		
		public void ScanGrid(){
			string scan = "";
			int i = 0;while (i < grid.GetLength(0)){
				
				int j = 0;while (j < grid.GetLength(1)){
					scan += grid[i,j].ToString() + " ";
					j++;}
					
				//Debug.Log("scanned: " + scan);scan = "";
				i++;}}
				
		public void ReadGrid(){
			string scan = "";
			int i = 0;while (i < grid.GetLength(0)){
				
				int j = 0;while (j < grid.GetLength(1)){
					scan += getCell(i,j).ToString() + " ";
					j++;}
					
				//Debug.Log("read: " + scan);scan = "";
				i++;}}
	}
		

		
	//_____________________________________________________________________________
	// debug
	
	void OnDrawGizmos(){
		//debug display
		Vector3 gizmosize = Vector3.up *.02f;
		Vector3 sized = new Vector3 (0.1f, 0.1f, 0.1f);
		
		//hash
		Vector3 cubed = MeshGenerator.sphere2cube (playerSurfacePosition);	//cube projection
		Vector3 bucket = VectorPlane(MeshGenerator.findCubeFace(cubed)) 
			* halftile + SnapRound( cubed * hashes ) / hashes;				//hash the position 

		//Debug.Log(MeshGenerator.findCubeFace(cubed));
		
		//----------------------------------------------------------------------------------------------
		//display the debug
		foreach (Vector3 v in this.plane.vertices) {//for each vertex in mesh add a gizmo to their cube projected position
			Vector3 vc = MeshGenerator.sphere2cube (v); Gizmos.color = Color.white;
			Gizmos.DrawLine (vc - gizmosize, vc + gizmosize );}
			
		//Gizmos.color = Color.white;Gizmos.DrawWireCube (ProxyRelativePosition, sized);//draw proxy
		Gizmos.color = Color.white;Gizmos.DrawWireSphere (playerSurfacePosition, 0.1f);//draw proxy on the surface
		Gizmos.color = Color.red;Gizmos.DrawCube (bucket, (sized/2) - 0.02f*Vector3.one);//draw bucketed proxy on the cube
		Gizmos.color = Color.green;Gizmos.DrawWireCube (cubed, sized/2);//draw proxy on the cube
		//Gizmos.color = Color.white;Gizmos.DrawLine (this.transform.position, ProxyRelativePosition);//draw to proxy on the cube
		//Debug.Log (playerSurfacePosition);
	}
}

