using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Aircraft : MonoBehaviour
{

    [SerializeField] internal Transform m_seatPosition;
    [SerializeField] internal Transform m_exitSeatPosition;

    PlayerHandler m_playerRef;
    Rigidbody m_rigidBody;

    internal void SetPlayerRef(PlayerHandler a_playerRef) { m_playerRef = a_playerRef; }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    float GetEngineForce()
    {
        return m_rigidBody.mass * 10000f * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerRef != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_rigidBody.AddForce(transform.forward * GetEngineForce());
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                m_rigidBody.AddForce(-transform.forward * GetEngineForce());
            }

            if (Input.GetKey(KeyCode.S))
            {
                m_rigidBody.AddRelativeTorque(new Vector3(-1000f * m_rigidBody.mass * Time.deltaTime, 0f, 0f));
            }
            else if (Input.GetKey(KeyCode.W))
            {
                m_rigidBody.AddRelativeTorque(new Vector3(1000f * m_rigidBody.mass * Time.deltaTime, 0f, 0f));
            }
        }

        float forwardMagnitude = VLib.CosDeg(Vector3.Angle(transform.forward.normalized, m_rigidBody.velocity.normalized)) * m_rigidBody.velocity.magnitude;

        m_rigidBody.AddForce(forwardMagnitude * m_rigidBody.mass * 10f * Time.deltaTime * transform.up);
    }
}
