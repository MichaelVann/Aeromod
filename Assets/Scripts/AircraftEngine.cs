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
    float m_engineInertia = 0f;
    bool m_engineOn = false;
    float m_throttle = 0f;
    float m_rpm = 0f;
    float m_idleRPM = 300;
    float m_maxRPM = 1200;
    float m_timeToReachDesiredRPM = 12f;
    [SerializeField] float m_maxTorque = 1000f;
    float m_engineToPropellerRPMRatio = 0.5f;
    bool m_transitioning = false;
    Rigidbody m_owningRigidbody;

    //Fuel
    [SerializeField] FuelTank[] m_availableFuelTanks;
    const float m_litresASecond = 0.0108f;

    internal float GetRPM() {  return m_rpm; }
    float GetRPMRatio() { return m_rpm / m_maxRPM; }

    internal float GetTorque() { return m_maxTorque * GetRPMRatio(); }


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

    // Start is called before the first frame update
    void Start()
    {
        m_owningRigidbody = GetComponentInParent<Rigidbody>();
        SetEngineOn(false);
    }

    void UpdateRPM()
    {
        float desiredRPM = m_engineOn ? m_maxRPM * m_throttle : 0f;
        m_rpm = Mathf.Lerp(m_rpm, desiredRPM, Time.deltaTime / m_timeToReachDesiredRPM);
        float rpmRatio = GetRPMRatio();
        m_engineAudioSource.pitch = m_engineAudioSource.volume = 0.75f + rpmRatio * 0.5f;
        m_propellerRef.SetRPM(m_rpm * m_engineToPropellerRPMRatio);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRPM();

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
