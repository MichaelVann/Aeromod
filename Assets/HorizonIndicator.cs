using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonIndicator : MonoBehaviour
{
    [SerializeField] GameObject m_gimbalRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {

        //m_gimbalRef.transform.rotation = Quaternion.identity;// - transform.rotation;
        //Vector3 newEuler = m_gimbalRef.transform.localEulerAngles;
        //newEuler.x *= -1f;
        m_gimbalRef.transform.rotation = Quaternion.identity;
        //Debug.Log("HI Euler: " + newEuler.x + ", " + newEuler.y + ", " + newEuler.z);
    }
}
