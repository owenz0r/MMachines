using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailManager : MonoBehaviour {

	public MeshFilter m_trail_filter = null; 

	List<List<Vector3>> m_trails;

	// Use this for initialization
	void Start () {
		m_trails = new List<List<Vector3>>();
	}
	
	// Update is called once per frame
	void Update () {
		generateMesh();
	}

	public void generateMesh()
	{
		if( m_trails.Count > 0 )
		{
			int vert_count = 0;
			foreach( List<Vector3> trail in m_trails )
			{
				vert_count += trail.Count;
			}

			Vector3[] verts = new Vector3[ vert_count ];
			Vector2[] uvs = new Vector2[ vert_count ];

			// subtract 2 faces for each trail after the first
			int num_faces = (vert_count / 2) - 2 - ( (m_trails.Count - 1) * 2 );
			int[] tris = new int[ num_faces * 3 ];

			int vert_pointer = 0;
			int uv_pointer = 0;
			int tri_counter = 0;
			foreach( List<Vector3> trail in m_trails )
			{
				for( int i=0; i < trail.Count; i+=4 )
				{
					uvs[ uv_pointer + i] = new Vector2(0,0);
					uvs[ uv_pointer + i+1] = new Vector2(1,0);
					uvs[ uv_pointer + i+2] = new Vector2(0,1);
					uvs[ uv_pointer + i+3] = new Vector2(1,1);
				}
				uv_pointer += trail.Count;

				int trail_faces = ( trail.Count / 2 ) - 2;
				int trail_tris = trail_faces * 3;

				// j for trianges, i for faces
				for( int i=2, j=0; j < trail_tris; i+=4, j+=6 )
				{
					// first triangle
					tris[ tri_counter + j ] = vert_pointer + i;
					tris[ tri_counter + j+1 ] = vert_pointer + i+1;
					tris[ tri_counter + j+2 ] = vert_pointer + i+2;
					
					// second triangle
					tris[ tri_counter + j+3 ] = vert_pointer + i+1;
					tris[ tri_counter + j+4 ] = vert_pointer + i+3;
					tris[ tri_counter + j+5 ] = vert_pointer + i+2;
				}
				tri_counter += trail_tris;

				foreach( Vector3 point in trail )
					verts[ vert_pointer++ ] = point;
			}

			Mesh trail_mesh = new Mesh();
			trail_mesh.vertices = verts;
			trail_mesh.triangles = tris;
			trail_mesh.uv = uvs;
			
			trail_mesh.RecalculateBounds();
			trail_mesh.RecalculateNormals();
			
			m_trail_filter.mesh = trail_mesh;
		}
	}

	public void addTrail( List<Vector3> new_trail )
	{
		m_trails.Add ( new_trail );
	}
}
