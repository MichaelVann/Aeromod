using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Aircraft : MonoBehaviour
{

    [SerializeField] internal Transform m_seatPosition;
    [SerializeField] internal Transform m_exitSeatPosition;

    //UI
    [SerializeField] TextMeshProUGUI m_airSpeedText;
    [SerializeField] TextMeshProUGUI m_throttleText;
    [SerializeField] TextMeshProUGUI m_altitudeText;

    PlayerHandler m_playerRef;
    Rigidbody m_rigidBody;

    [SerializeField]
    List<AeroSurface> m_controlSurfaces = null;
    [SerializeField]
    List<WheelCollider> m_wheels = null;
    [Range(0, 1), SerializeField]
    float m_rollControlSensitivity = 0.2f;
    [Range(0, 1), SerializeField]
    float m_pitchControlSensitivity = 0.2f;
    [Range(0, 1), SerializeField]
    float m_yawControlSensitivity = 0.2f;

    [Range(-1, 1), SerializeField]
    float m_pitch;
    [Range(-1, 1)]
    public float m_yaw;
    [Range(-1, 1)]
    public float m_roll;
    [Range(0, 1)]
    public float m_flap;

    float m_throttle = 0f;
    float m_brakesTorque;

    AircraftPhysics m_aircraftPhysics;

    internal void SetPlayerRef(PlayerHandler a_playerRef) { m_playerRef = a_playerRef; }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_aircraftPhysics = GetComponent<AircraftPhysics>();
        FindControlSurfaces();
    }

    void FindControlSurfaces()
    {
        m_controlSurfaces = new List<AeroSurface>(GetComponentsInChildren<AeroSurface>()); ;
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
            m_pitch = Input.GetAxis("Vertical");
            m_roll = Input.GetAxis("Horizontal");
            m_yaw = Input.GetAxis("Yaw");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_throttle += Time.deltaTime;
                m_throttle = Mathf.Clamp(m_throttle, 0f, 1f);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                m_throttle -= Time.deltaTime;
                m_throttle = Mathf.Clamp(m_throttle, 0f, 1f);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                m_flap = m_flap > 0 ? 0 : 0.3f;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                m_brakesTorque = m_brakesTorque > 0 ? 0 : 100f;
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_airSpeedText.text = "Speed: " + VLib.RoundToDecimalPlaces(m_rigidBody.velocity.magnitude,1).ToString() + " m/s";
        m_throttleText.text = "Throttle: " + VLib.RoundToDecimalPlaces(m_throttle * m_aircraftPhysics.thrust /10f, 1).ToString() + " hp";
        m_altitudeText.text = "Alt: " + ((int)transform.position.y).ToString("D4") + " m";
        //displayText.text = "V: " + ((int)m_rigidBody.velocity.magnitude).ToString("D3") + " m/s\n";
        //displayText.text += "A: " + ((int)transform.position.y).ToString("D4") + " m\n";
        //displayText.text += "T: " + (int)(thrustPercent * 100) + "%\n";
        //displayText.text += brakesTorque > 0 ? "B: ON" : "B: OFF";
    }

    private void FixedUpdate()
    {
        Debug.Log(m_roll);
        SetControlSurfecesAngles(m_pitch, m_roll, m_yaw, m_flap);
        m_aircraftPhysics.SetThrustPercent(m_throttle);
        foreach (var wheel in m_wheels)
        {
            wheel.brakeTorque = m_brakesTorque;
            // small torque to wake up wheel collider
            wheel.motorTorque = 0.01f;
        }
    }

    public void SetControlSurfecesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in m_controlSurfaces)
        {
            if (surface == null || !surface.IsControlSurface) continue;
            switch (surface.InputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * m_pitchControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * m_rollControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * m_yawControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(m_flap * surface.InputMultiplyer);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            FindControlSurfaces();
            SetControlSurfecesAngles(m_pitch, m_roll, m_yaw, m_flap);
        }
    }
}
