using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftPropeller : MonoBehaviour
{
    float m_rpm;
    float m_rotation = 0f;

    internal void SetRPM(float a_rpm)
    {
        m_rpm = a_rpm;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_rotation += m_rpm * 6f * Time.deltaTime;
        transform.localEulerAngles = new Vector3(0f,0f, m_rotation);
    }
}
