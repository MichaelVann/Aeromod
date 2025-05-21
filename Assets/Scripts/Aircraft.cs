using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;

public class Aircraft : MonoBehaviour
{

    [SerializeField] internal Transform m_seatPosition;
    [SerializeField] internal Transform m_exitSeatPosition;

    //UI
    [SerializeField] TextMeshProUGUI m_airSpeedText;
    [SerializeField] TextMeshProUGUI m_throttleText;
    [SerializeField] TextMeshProUGUI m_altitudeText;
    [SerializeField] InstrumentDial m_airspeedDialRef;
    [SerializeField] InstrumentDial m_rpmDialRef;
    [SerializeField] InstrumentDial m_altitudeDialRef;
    [SerializeField] InstrumentDial m_fuelDialRef;
    [SerializeField] InstrumentDial m_climbDialRef;

    [SerializeField] AircraftEngine[] m_aircraftEngineRefs;

    [SerializeField] CameraHandler m_cameraHandlerRef;

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

    float m_prevAlt = 0f;

    AircraftPhysics m_aircraftPhysics;

    internal float GetSpeed()
    {
        return m_rigidBody.linearVelocity.magnitude;
    }

    internal void SetPlayerRef(PlayerHandler a_playerRef) { m_playerRef = a_playerRef; }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_aircraftPhysics = GetComponent<AircraftPhysics>();
        FindControlSurfaces();

        m_prevAlt = transform.position.y;

        InitialiseInstruments();
    }

    void InitialiseInstruments()
    {
        m_airspeedDialRef.Init("Airspeed mph", 25, 9, 3);
        m_rpmDialRef.Init("RPM", (int)m_aircraftEngineRefs[0].GetMaxRPM()/4, 5, 4);
        m_altitudeDialRef.Init("Altitude 100m", 100, 11, 3, 0f, 0f, true);
        m_fuelDialRef.Init("Fuel Litres", (int)(m_aircraftEngineRefs[0].GetFuelCapacity()/10f), 11, 1);
        m_climbDialRef.Init("Climb", 5, 9, 5, -20f);
    }

    void FindControlSurfaces()
    {
        m_controlSurfaces = new List<AeroSurface>(GetComponentsInChildren<AeroSurface>()); ;
    }

    internal float GetShakeAmount()
    {
        float shakeAmount = 0f;
        for (int i = 0; i < m_aircraftEngineRefs.Length; i++)
        {
            shakeAmount += m_aircraftEngineRefs[i].GetTorque() / 15000f;
        }
        return shakeAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerRef != null)
        {
            m_pitch = Mathf.Clamp(Input.GetAxis("Pitch") + Input.GetAxis("PitchTrim"), -1f, 1f);
            m_roll = Input.GetAxis("Roll");
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
                m_brakesTorque = m_brakesTorque > 0 ? 0 : 1000f;
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                for (int i = 0; i < m_aircraftEngineRefs.Length; i++)
                {
                    m_aircraftEngineRefs[i].ToggleEngine();
                }
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_airSpeedText.text = "Speed: " + VLib.RoundToDecimalPlaces(VLib._msToMph * m_rigidBody.linearVelocity.magnitude,1).ToString("f1") + " mph";
        m_throttleText.text = "Throttle: " + VLib.RoundToDecimalPlaces(m_throttle * 100f, 1).ToString() + "%";
        m_altitudeText.text = "Alt: " + ((int)transform.position.y).ToString("D4") + " m";
        m_airspeedDialRef.SetValue(VLib._msToMph * m_rigidBody.linearVelocity.magnitude);
        m_rpmDialRef.SetValue(m_aircraftEngineRefs[0].GetRPM());
        m_altitudeDialRef.SetValue(transform.position.y);
        m_fuelDialRef.SetValue(m_aircraftEngineRefs[0].GetFuelLevel());
        m_climbDialRef.SetValue((transform.position.y - m_prevAlt)/Time.deltaTime);
        m_prevAlt = transform.position.y;
        //displayText.text = "V: " + ((int)m_rigidBody.velocity.magnitude).ToString("D3") + " m/s\n";
        //displayText.text += "A: " + ((int)transform.position.y).ToString("D4") + " m\n";
        //displayText.text += "T: " + (int)(thrustPercent * 100) + "%\n";
        //displayText.text += brakesTorque > 0 ? "B: ON" : "B: OFF";
    }

    private void FixedUpdate()
    {
        SetControlSurfacesAngles(m_pitch, m_roll, m_yaw, m_flap);
        for (int i = 0; i < m_aircraftEngineRefs.Length; i++)
        {
            m_aircraftEngineRefs[i].SetThrottle(m_throttle);
        }
        foreach (var wheel in m_wheels)
        {
            wheel.brakeTorque = m_brakesTorque;
            // small torque to wake up wheel collider
            wheel.motorTorque = 0.01f;
        }
    }

    public void SetControlSurfacesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in m_controlSurfaces)
        {
            if (surface == null || !surface.m_isControlSurface) continue;
            switch (surface.m_inputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * m_pitchControlSensitivity * surface.m_inputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * m_rollControlSensitivity * surface.m_inputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * m_yawControlSensitivity * surface.m_inputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(m_flap * surface.m_inputMultiplyer);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            FindControlSurfaces();
            SetControlSurfacesAngles(m_pitch, m_roll, m_yaw, m_flap);
        }
    }
}
