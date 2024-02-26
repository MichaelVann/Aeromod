using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroWeight : MonoBehaviour
{
    [SerializeField] float m_weight = 0f;

    internal float GetWeight() {  return m_weight; }
    internal void SetWeight(float a_weight) { m_weight = a_weight; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
