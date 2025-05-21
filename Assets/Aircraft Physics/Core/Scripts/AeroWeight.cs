using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroWeight : MonoBehaviour
{
    [SerializeField] float m_weight = 0f;
    Vector3 m_position;

    internal float GetWeight() {  return m_weight; }
    internal Vector3 GetCOMPosition() { return m_position; }
    internal void SetWeight(float a_weight) { m_weight = a_weight; }

    // Start is called before the first frame update
    void Awake()
    {
        m_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
