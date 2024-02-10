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
    float m_maxRPM = 1200;
    [SerializeField] float m_maxTorque = 1000f;
    float m_engineToPropellerRPMRatio = 0.5f;
    bool m_transitioning = false;
    Rigidbody m_owningRigidbody;

    float GetRPMRatio() { return m_rpm / m_maxRPM; }

    internal float GetTorque() { return m_maxTorque * GetRPMRatio(); }

    internal void ToggleEngine() { SetEngineOn(!m_engineOn); }

    internal void SetThrottle(float a_throttle) { m_throttle = a_throttle; }

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

    // Update is called once per frame
    void Update()
    {
        if (m_engineOn && !m_engineAudioSource.isPlaying)
        {
            m_engineAudioSource.clip = m_engineRunningAudio;
            m_engineAudioSource.loop = true;
            m_engineAudioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        Vector3 forwardForce = transform.forward * GetTorque();
        m_owningRigidbody.AddForce(forwardForce);
    }

    internal void UpdateFromAircraft()
    {
        float desiredRPM = m_engineOn ? m_maxRPM * m_throttle : 0f;
        m_rpm = Mathf.Lerp(m_rpm, desiredRPM, Time.deltaTime/4f);
        float rpmRatio = GetRPMRatio();
        m_engineAudioSource.pitch = m_engineAudioSource.volume = 0.75f + rpmRatio * 0.5f;
        m_propellerRef.SetRPM(m_rpm * m_engineToPropellerRPMRatio);
    }
}
