using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailGenerator : MonoBehaviour {

	public float minDistance = 1.0f;
	public float width = 1.0f;
	public MeshFilter trailMeshFilter = null;

	List<Vector3> m_point_list = null;
	bool m_generating = false;
	Vector3 m_last_point;

	// Use this for initialization
	void Start () {
		m_point_list = new List<Vector3>();
	}
	
	// Update is called once per frame
	void Update () {

		if( m_generating == true )
		{
			addPoints();
			if( Input.GetKeyDown( KeyCode.S ) )
				m_generating = false;
		} else {
			if( Input.GetKeyDown( KeyCode.S ) )
				m_generating = true;
		}
		generateMesh();
	}

	void addPoints()
	{
		// if we have a starting point
		if( m_point_list.Count > 0 )
		{
			// check we're far enough away from the last point drop
			float dist = (transform.position - m_last_point).magnitude;
			if( dist >= minDistance )
			{
				addCurrentPosition();
			}
		} else {
			addCurrentPosition();
		}
	}

	void addCurrentPosition()
	{
		// last position where we added points
		m_last_point = new Vector3( transform.position.x, transform.position.y, transform.position.z );

		// actual points to be added
		Vector3 right_pos = transform.position + ( transform.right  * width );
		right_pos.z += 1.0f;
		Vector3 left_pos = transform.position + ( transform.right * -1.0f * width);
		left_pos.z += 1.0f;
		m_point_list.Add( right_pos );
		m_point_list.Add( left_pos );
		m_point_list.Add( right_pos );
		m_point_list.Add( left_pos );
	}

	void generateMesh()
	{
		// dont bother unless we have 8 points
		if( m_point_list.Count >= 8 )
		{
			Vector3[] verts = new Vector3[ m_point_list.Count ];

			// have to convert to world space
			for( int i=0; i < m_point_list.Count; i++ )
				verts[i] = transform.InverseTransformPoint( m_point_list[i] );

			// uv mapping
			Vector2[] uvs = new Vector2[ m_point_list.Count ];
			for( int i=0; i < m_point_list.Count; i+=4 )
			{
				uvs[i] = new Vector2(0,0);
				uvs[i+1] = new Vector2(1,0);
				uvs[i+2] = new Vector2(0,1);
				uvs[i+3] = new Vector2(1,1);
			}

			int num_faces = ( m_point_list.Count / 2 ) - 2;

			int[] tris = new int[ num_faces * 3 ];
			// j for trianges, i for faces
			for( int i=2, j=0; j < tris.Length; i+=4, j+=6 )
			{
				// first triangle
				tris[ j ] = i;
				tris[ j+1 ] = i+1;
				tris[ j+2 ] = i+2;

				// second triangle
				tris[ j+3 ] = i+1;
				tris[ j+4 ] = i+3;
				tris[ j+5 ] = i+2;
			}

			Mesh trail_mesh = new Mesh();
			trail_mesh.vertices = verts;
			trail_mesh.triangles = tris;
			trail_mesh.uv = uvs;

			trail_mesh.RecalculateBounds();
			trail_mesh.RecalculateNormals();

			trailMeshFilter.mesh = trail_mesh;
		}
	}

	void OnDrawGizmos()
	{
		if( m_point_list != null && m_point_list.Count > 0 )
		{
			foreach( Vector3 point in m_point_list )
				Gizmos.DrawSphere( point, 0.02f );
		}
	}
}
