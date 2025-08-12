using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroFin : MonoBehaviour
{
    [SerializeField] float m_baseChord = 1.43f;
    [SerializeField] float m_span = 4f;
    [SerializeField] float m_tipChord = 1.43f;
    [SerializeField] float m_tipOffsetRatio = 0f;
    float m_tipOffset;
    [SerializeField] float m_spanRounding = 0.4f;
    const int m_spanRoundingFidelity = 10;
    [SerializeField] float m_tipRounding = 10f;
    const int m_tipRoundingFidelity = 10;
    [SerializeField] float m_finWidth = 0.1f;
    [SerializeField] float m_flapFraction = 0.3f;
    [SerializeField] float m_controlSurfaceFraction = 0.5f;
    float m_centreOfLiftZOffset = 1f/4f;
    [SerializeField] float m_zeroLiftAoA = 0f;
    [SerializeField] ControlInputType m_controlInputType;
    [SerializeField] bool m_leftSide = false;
    [SerializeField] bool m_centred = false;
    int m_mirrorMultiplier = 1;
    float m_rootX = 0f;

    List<AeroSurface> m_aeroSurfaces;

    //Mesh
    [SerializeField] Material m_material;
    Mesh m_mesh;
    MeshFilter m_meshFilter;
    MeshRenderer m_meshRenderer;

    internal List<AeroSurface> GetAeroSurfaces() { return m_aeroSurfaces; }

    // Start is called before the first frame update
    void Awake()
    {
        m_aeroSurfaces = new List<AeroSurface>();
        //Init(2f, 4f, 1f, 1f, true);
        Init(m_baseChord, m_span, m_tipChord, m_tipOffsetRatio, m_spanRounding, m_tipRounding, m_flapFraction, m_controlSurfaceFraction, m_controlInputType, m_leftSide, m_centred);
    }

    internal void Init(float a_baseChord, float a_span, float a_tipChord, float a_tipOffsetRatio, float a_spanRounding, float a_tipRounding, float a_flapFraction, float a_controlSurfaceFraction, ControlInputType a_controlInputType, bool a_leftHanded, bool a_centred)
    {
        m_baseChord = a_baseChord;
        m_span = a_span;
        m_tipChord = a_tipChord;
        m_tipOffsetRatio = a_tipOffsetRatio;
        m_tipOffset = m_tipOffsetRatio * (m_baseChord - m_tipChord)/2f;
        m_spanRounding = a_spanRounding;
        m_tipRounding = a_tipRounding;
        m_flapFraction = a_flapFraction;
        m_controlSurfaceFraction = a_controlSurfaceFraction;
        m_controlInputType = a_controlInputType;
        m_mirrorMultiplier = a_leftHanded ? -1 : 1;
        m_centred = a_centred;

        m_rootX = m_centred ? -m_span / 2f : 0f;

        //Mesh
        SetUpMesh();
        m_meshRenderer.material = m_material;
        gameObject.AddComponent<MeshCollider>().convex = true;

        //Aero Surface
        SetUpAeroSurfaces();
    }

    void SpawnAeroSurface(ControlInputType a_inputType, float a_span, float a_offset)
    {
        GameObject aeroSurfaceObj = Instantiate(new GameObject(), transform);
        AeroSurface aeroSurface = aeroSurfaceObj.AddComponent<AeroSurface>();
        m_aeroSurfaces.Add(aeroSurface);
        float averageChordLength = (m_baseChord + m_tipChord) / 2f;
        AeroSurfaceConfig config = new AeroSurfaceConfig(averageChordLength, a_span, 0.4f);
        aeroSurface.SetAeroSurfaceConfig(config);

        float xPos = m_mirrorMultiplier * (m_rootX + a_offset + a_span / 2f);
        aeroSurface.transform.localPosition = new Vector3(xPos, 0f, averageChordLength * m_centreOfLiftZOffset + m_tipOffset/2f);
        aeroSurface.transform.localEulerAngles = new Vector3(0f, -90f, 0f);

        float controlSurfaceMultiplier = a_inputType == ControlInputType.Roll ? -m_mirrorMultiplier : 1f;
        aeroSurface.SetControlInputType(a_inputType, controlSurfaceMultiplier);
    }

    void SetUpAeroSurfaces()
    {
        float surfaceOffset = 0f;
        if (m_flapFraction > 0f)
        {
            SpawnAeroSurface(ControlInputType.Flap, m_span * m_flapFraction, surfaceOffset);
            surfaceOffset += m_span * m_flapFraction;
        }

        float baseWingFraction = 1f - m_flapFraction - m_controlSurfaceFraction;
        if (baseWingFraction > Mathf.Epsilon)
        {
            SpawnAeroSurface(ControlInputType.None, m_span * baseWingFraction, surfaceOffset);
            surfaceOffset += m_span * baseWingFraction;
        }

        if (m_controlSurfaceFraction > 0f)
        {
            SpawnAeroSurface(m_controlInputType, m_span * m_controlSurfaceFraction, surfaceOffset);
        }
    }

    void AddSpanRounding(List<Vector3> a_vertList, bool a_front)
    {
        if (m_spanRounding > 0f)
        {
            for (int i = 0; i < m_spanRoundingFidelity; i++)
            {
                float spanPos = (float)(i) / (m_spanRoundingFidelity);
                Vector3 vert = new Vector3();
                float lerpA = m_baseChord / 2f;
                float lerpB = m_tipChord / 2f + m_tipOffset;
                float sinPos = Mathf.Sin(Mathf.PI * spanPos);
                if (!a_front)
                {
                    lerpA = -m_tipChord / 2f + m_tipOffset;
                    lerpB = -m_baseChord / 2f;
                    sinPos *= -1f;
                }
                float lerpZPos = Mathf.Lerp(lerpA, lerpB, spanPos);
                vert.z = lerpZPos + sinPos * m_span * m_spanRounding / 100f;
                vert.x = m_rootX;
                vert.x += m_span * (a_front ? spanPos : 1f - spanPos);
                vert.x *= m_mirrorMultiplier;
                vert.y = m_finWidth / 2f;
                a_vertList.Add(vert);
            }
        }
    }

    void AddTriangle(List<int> a_triList, int a_firstVert, int a_secondVert, int a_thirdVert)
    {
        if (m_mirrorMultiplier == 1)
        {
            a_triList.Add(a_firstVert);
            a_triList.Add(a_secondVert);
            a_triList.Add(a_thirdVert);
        }
        else if (m_mirrorMultiplier == -1)
        {
            a_triList.Add(a_thirdVert);
            a_triList.Add(a_secondVert);
            a_triList.Add(a_firstVert);
        }
    }

    void SetUpMesh()
    {
        m_mesh = new Mesh();
        m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
        List<Vector3> meshVerts = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> topVerts = new List<Vector3>();

        //Front edge
        Vector3 frontRootVert = new Vector3(m_rootX * m_mirrorMultiplier, m_finWidth / 2f, m_baseChord / 2f);
        Vector3 frontTipVert = new Vector3(m_mirrorMultiplier * (m_rootX + m_span), m_finWidth / 2f, m_tipChord / 2f);
        frontTipVert.z += m_tipOffset;
        topVerts.Add(frontRootVert);
        AddSpanRounding(topVerts, true);
        topVerts.Add(frontTipVert);

        List<int> topTriangles = new List<int>();
        int m_bottomVertIndexOffset = topVerts.Count;

        //Rounded Tip
        if (m_tipRounding > 0)
        {
            for (int i = 0; i < m_tipRoundingFidelity; i++)
            {
                float chordPos = (i + 1f) / (m_tipRoundingFidelity + 1f);
                Vector3 vert = new Vector3();
                vert.x = m_rootX + m_span + Mathf.Sin(Mathf.PI * chordPos) * m_tipChord * m_tipRounding /100f;
                vert.x *= m_mirrorMultiplier;
                vert.z = m_tipChord / 2f + m_tipOffset;
                vert.z -= m_tipChord * Mathf.Pow(chordPos, 1f + m_tipOffsetRatio/5f);
                vert.y = m_finWidth / 2f;
                topVerts.Add(vert);

                if (i > 0)
                {
                    AddTriangle(topTriangles, m_bottomVertIndexOffset - 1, m_bottomVertIndexOffset + i - 1, m_bottomVertIndexOffset + i);
                }
            }
            AddTriangle(topTriangles, m_bottomVertIndexOffset - 1, m_bottomVertIndexOffset + m_tipRoundingFidelity - 1, m_bottomVertIndexOffset + m_tipRoundingFidelity);
        }

        m_bottomVertIndexOffset = topVerts.Count;

        //Front edge
        Vector3 rearRootVert = new Vector3(m_rootX * m_mirrorMultiplier, m_finWidth / 2f, -m_baseChord / 2f);
        Vector3 rearTipVert = new Vector3(m_mirrorMultiplier * (m_rootX + m_span), m_finWidth / 2f, -m_tipChord / 2f);
        rearTipVert.z += m_tipOffset;

        topVerts.Add(rearTipVert);
        AddSpanRounding(topVerts, false);
        topVerts.Add(rearRootVert);

        //Top wing surface

        int surfaceQuadsNeeded = 1 + (m_spanRounding > 0f ? m_spanRoundingFidelity : 0);

        for (int i = 0; i < surfaceQuadsNeeded; i++)
        {
            int anchorVert = 2 * surfaceQuadsNeeded + 1 - i + (m_tipRounding > 0 ? m_tipRoundingFidelity : 0);

            AddTriangle(topTriangles, i, i + 1, anchorVert);
            AddTriangle(topTriangles, i +1 , anchorVert - 1, anchorVert);
        }


        m_bottomVertIndexOffset = topVerts.Count;

        //Duplicate bottom tris
        List<Vector3> bottomVerts = new List<Vector3>(topVerts);
        for (int i = 0; i < bottomVerts.Count; i++)
        {
            bottomVerts[i] += new Vector3(0f, -m_finWidth/2f, 0f);
        }
        List<int> bottomTriangles = new List<int>(topTriangles);
        for (int i = 0; i < bottomTriangles.Count; i++)
        {
            bottomTriangles[i] += m_bottomVertIndexOffset;
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
            AddTriangle(stitchTriangles, i - 1 + m_bottomVertIndexOffset, i, i - 1);
            AddTriangle(stitchTriangles, i - 1 + m_bottomVertIndexOffset, i + m_bottomVertIndexOffset, i);
        }

        //Close base              
        AddTriangle(stitchTriangles, m_bottomVertIndexOffset + bottomVerts.Count - 1, m_bottomVertIndexOffset, m_bottomVertIndexOffset - 1);
        AddTriangle(stitchTriangles, m_bottomVertIndexOffset, 0, m_bottomVertIndexOffset - 1);

        meshVerts.AddRange(topVerts);
        meshVerts.AddRange(bottomVerts);
        triangles.AddRange(topTriangles);
        triangles.AddRange(bottomTriangles);
        triangles.AddRange(stitchTriangles);

        Vector2[] uvs = new Vector2[meshVerts.Count];
        for (int i = 0; i < meshVerts.Count; i++)
        {
            uvs[i] = new Vector2(meshVerts[i].x, meshVerts[i].z)/10f;
        }

        m_mesh.vertices = meshVerts.ToArray();
        m_mesh.triangles = triangles.ToArray();

        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();
        m_mesh.SetUVs(0, uvs);

        m_meshFilter = gameObject.AddComponent<MeshFilter>();
        m_meshFilter.mesh = m_mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
