using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroFin : MonoBehaviour
{
    float m_baseChord;
    float m_span;
    float m_tipChord;
    float m_tipOffsetRatio;
    float m_tipOffset;
    bool m_roundedTip;
    float m_finWidth;

    //Mesh
    [SerializeField] Material m_material;
    Mesh m_mesh;
    MeshFilter m_meshFilter;
    MeshRenderer m_meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //Init(2f, 4f, 1f, 1f, true);
        Init(1.2f, 2f, 1.1f, 0f, true);
    }

    internal void Init(float a_baseChord, float a_span, float a_tipChord, float a_tipOffsetRatio, bool a_roundedTip)
    {
        m_baseChord = a_baseChord;
        m_span = a_span;
        m_tipChord = a_tipChord;
        m_tipOffsetRatio = a_tipOffsetRatio;
        m_tipOffset = m_tipOffsetRatio * (m_baseChord - m_tipChord)/2f;
        m_roundedTip = a_roundedTip;
        m_finWidth = 0.1f;
        SetUpMesh();
        m_meshRenderer.material = m_material;
    }

    void SetUpMesh()
    {
        m_mesh = new Mesh();
        m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
        List<Vector3> meshVerts = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> topVerts = new List<Vector3>();
        int m_topMeshVertIndex = 0;

        for (int i = 0; i < 4; i++)
        {
            bool isFrontVert = (i == 1 || i == 2);
            bool isRootVert = i <= 1;
            Vector3 vert = new Vector3();
            vert.x = isRootVert ? 0f : m_span;
            vert.z = isRootVert ? m_baseChord/2f : m_tipChord/2f;
            vert.z *= isFrontVert ? 1f : -1f;
            vert.z += isRootVert ? 0f : m_tipOffset;
            vert.y = m_finWidth/2f;

            topVerts.Add(vert);
        }
        List<int> topTriangles = new List<int> { 0, 1, 2, 2, 3, 0 };
        m_topMeshVertIndex = topVerts.Count;

        //Rounded Tip
        if (m_roundedTip)
        {
            int roundedPoints = 10;
            for (int i = 0; i < roundedPoints; i++)
            {
                float chordPos = (i + 1f) / (roundedPoints + 1f);
                Vector3 vert = new Vector3();
                vert.x = m_span + Mathf.Sin(Mathf.PI * chordPos) * m_tipChord/3f;
                vert.x += m_tipChord/10f;
                vert.z = m_tipChord / 2f + m_tipOffset;
                vert.z -= m_tipChord * Mathf.Pow(chordPos, 1f + m_tipOffsetRatio/5f);
                vert.y = m_finWidth / 2f;
                topVerts.Add(vert);

                topTriangles.Add(m_topMeshVertIndex - 1);
                topTriangles.Add(i > 0 ? m_topMeshVertIndex + (i - 1) : m_topMeshVertIndex - 2);
                topTriangles.Add(m_topMeshVertIndex + i);
            }
        }
        m_topMeshVertIndex = topVerts.Count;

        //Duplicate bottom tris
        List<Vector3> bottomVerts = new List<Vector3>(topVerts);
        for (int i = 0; i < bottomVerts.Count; i++)
        {
            bottomVerts[i] += new Vector3(0f, -m_finWidth/2f, 0f);
        }
        List<int> bottomTriangles = new List<int>(topTriangles);
        for (int i = 0; i < bottomTriangles.Count; i++)
        {
            bottomTriangles[i] += m_topMeshVertIndex;
        }

        for (int i = 0; i < bottomTriangles.Count; i += 3)
        {
            int[] originalVerts = new int[] { bottomTriangles[i], bottomTriangles[i+1], bottomTriangles[i+2]};
            bottomTriangles[i] = originalVerts[2];
            bottomTriangles[i+1] = originalVerts[1];
            bottomTriangles[i+2] = originalVerts[0];
        }

        List<int> stitchTriangles = new List<int>();

        //Stitch top and bottom
        for (int i = 1; i < topVerts.Count; i++)
        {
            stitchTriangles.Add(i-1 + m_topMeshVertIndex);
            stitchTriangles.Add(i);
            stitchTriangles.Add(i-1);

            stitchTriangles.Add(i - 1 + m_topMeshVertIndex);
            stitchTriangles.Add(i + m_topMeshVertIndex);
            stitchTriangles.Add(i);
        }


        meshVerts.AddRange(topVerts);
        meshVerts.AddRange(bottomVerts);
        triangles.AddRange(topTriangles);
        triangles.AddRange(bottomTriangles);
        triangles.AddRange(stitchTriangles);

        m_mesh.vertices = meshVerts.ToArray();
        m_mesh.triangles = triangles.ToArray();

        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();

        m_meshFilter = gameObject.AddComponent<MeshFilter>();
        m_meshFilter.mesh = m_mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
