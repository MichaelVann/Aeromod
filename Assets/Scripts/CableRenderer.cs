using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableRenderer : MonoBehaviour
{
    [SerializeField] Transform m_endPosition;
    [SerializeField] LineRenderer m_lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Vector3[] m_cablePositions = new Vector3[2];
        m_cablePositions[0] = new Vector3();
        m_cablePositions[1] = m_endPosition.localPosition;
        m_lineRenderer.SetPositions(m_cablePositions);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
