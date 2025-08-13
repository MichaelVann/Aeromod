using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftEngine : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip m_engineStartUpAudio;
    [SerializeField] AudioClip m_engineRunningAudio;
    [SerializeField] AudioClip m_engineShutdownAudio;
    [SerializeField] AudioSource m_engineAudioSource;
    [SerializeField] AircraftPropeller m_propellerRef;
    [SerializeField] Light m_engineOnIndicator;
    bool m_engineOn = false;
    float m_throttle = 0f;
    float m_rpm = 0f;
    [SerializeField] float m_maxRPM;
    [SerializeField] float m_torquePerRPM;
    float m_engineToPropellerRPMRatio = 0.5f;
    Rigidbody m_owningRigidbody;

    //Power
    [SerializeField] float m_power;
    [SerializeField] float m_inertia;
    [SerializeField] float m_friction;

    const float m_idealStoichiometricRatio = 14.7f;

    //Fuel
    [SerializeField] FuelTank[] m_availableFuelTanks;
    const float m_litresASecond = 0.0108f;

    internal float GetRPM() {  return m_rpm; }
    internal float GetMaxRPM() { return m_maxRPM; }
    float GetRPMRatio() { return m_rpm / m_maxRPM; }

    internal float GetTorque() { return m_torquePerRPM * m_rpm; }


    internal void ToggleEngine() { SetEngineOn(!m_engineOn); }

    internal void SetThrottle(float a_throttle) { m_throttle = a_throttle; }

    internal float GetFuelCapacity() 
    {
        float capacity = 0f;
        for (int i = 0; i < m_availableFuelTanks.Length; i++)
        {
            capacity += m_availableFuelTanks[i].GetCapacity();
        }
        return capacity;
    }


    internal float GetFuelLevel()
    {
        float totalFuel = 0f;
        for (int i = 0; i < m_availableFuelTanks.Length; i++)
        {
            totalFuel += m_availableFuelTanks[i].GetFuel();
        }
        return totalFuel;
    }

    internal void SetEngineOn(bool a_on)
    {
        if (a_on && !m_engineOn)
        {
            m_engineAudioSource.clip = m_engineStartUpAudio;
            m_engineAudioSource.loop = false;
            m_engineAudioSource.Play();
        }
        if (!a_on && m_engineOn)
        {
            m_engineAudioSource.clip = m_engineShutdownAudio;
            m_engineAudioSource.loop = false;
            m_engineAudioSource.Play();
        }
        m_engineOn = a_on;
        m_engineOnIndicator.enabled = m_engineOn;
    }

    internal void Init(float a_power = 120f, float a_inertia = 2000f, float a_friction = 0.08f, float a_maxRpm = 8000f, float a_torquePerRpm = 2f)
    {
        m_power = a_power;
        m_inertia = a_inertia;
        m_friction = a_friction;
        m_maxRPM = a_maxRpm;
        m_torquePerRPM = a_torquePerRpm;
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        m_owningRigidbody = GetComponentInParent<Rigidbody>();
        SetEngineOn(false);
    }

    void UpdateRPMEffects()
    {
        //float desiredRPM = m_engineOn ? m_maxRPM * m_throttle : 0f;
        //m_rpm = Mathf.Lerp(m_rpm, desiredRPM, Time.deltaTime / m_timeToReachDesiredRPM);
        //m_rpm = m_rpm > 1200 ? 1100: m_rpm;
        float rpmRatio = GetRPMRatio();
        m_engineAudioSource.pitch = m_rpm / 1200;
        m_engineAudioSource.volume = 0.75f + rpmRatio * 0.5f;
        m_propellerRef.SetRPM(m_rpm * m_engineToPropellerRPMRatio);
    }

    void UpdateForces()
    {
        float appliedPower = m_engineOn ? m_throttle * m_power : 0f;
        appliedPower *= m_rpm;

        if (Input.GetKey(KeyCode.K))
        {
            appliedPower += 5000f * m_inertia / (Mathf.Max(m_rpm, 100));
        }

        appliedPower -= m_friction * Mathf.Pow(m_rpm, 2f);

        m_rpm += (Time.fixedDeltaTime * appliedPower / m_inertia);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateRPM();
        UpdateForces();
        UpdateRPMEffects();
        if (m_engineOn)
        {
            if (!m_engineAudioSource.isPlaying)
            {
                m_engineAudioSource.clip = m_engineRunningAudio;
                m_engineAudioSource.loop = true;
                m_engineAudioSource.Play();
            }

            FuelTank usedFuelTank = null;

            for (int i = 0; i < m_availableFuelTanks.Length && usedFuelTank == null; i++)
            {
                if (m_availableFuelTanks[i].GetFuel() > 0f)
                {
                    usedFuelTank = m_availableFuelTanks[i];
                }
            }

            if (usedFuelTank != null)
            {
                usedFuelTank.ChangeFuelAmount(-m_litresASecond * Time.deltaTime * m_throttle);
            }
            else
            {
                ToggleEngine();
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 forwardForce = transform.forward * GetTorque();
        m_owningRigidbody.AddForce(forwardForce);
    }
}
