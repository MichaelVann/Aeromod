using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioWindGenerator : MonoBehaviour
{
    System.Random m_random = new System.Random();
    [SerializeField] float m_volume = 0.5f;
    float m_baseLowPassFilterPosition = 1300f;
    float m_baseHighPassFilterPosition = 250f;
    [Range(-1f,2f)]
    float m_bandPassFilterPosition = 0f;
    float m_windSpeed = 0f; 

    float m_whiteNoiseStrength = 0.1f;
    float m_bandPassFilterGap = 1000f;

    AudioLowPassFilter m_audioLowPassFilter;
    AudioHighPassFilter m_audioHighPassFilter;

    private double m_phase = 0f;
    private int m_sampleRate;

    internal void SetWindSpeed(float a_windSpeed)
    {
        m_windSpeed = a_windSpeed;
        UpdateBandPassFilterFromWindSpeed();
        m_volume = Mathf.Clamp(a_windSpeed/12f, 0f, 5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_sampleRate = AudioSettings.outputSampleRate;
        m_audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        m_audioHighPassFilter = GetComponent<AudioHighPassFilter>();
    }

    void UpdateBandPassFilterFromWindSpeed()
    {
        m_bandPassFilterPosition = Mathf.Clamp(Mathf.Pow(m_windSpeed, 0.5f) / 9f, 0f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        m_audioLowPassFilter.cutoffFrequency = m_baseLowPassFilterPosition * m_bandPassFilterPosition;
        m_audioHighPassFilter.cutoffFrequency = Mathf.Clamp(m_baseHighPassFilterPosition * m_bandPassFilterPosition, 0f, 10000f);
    }

    void OnAudioFilterRead(float[] a_data, int a_channels)
    {
        for (int i = 0; i < a_data.Length; i += a_channels)
        {
            float whiteNoise = (float)(m_random.NextDouble() * 2 - 1);
            whiteNoise *= m_whiteNoiseStrength;
            whiteNoise *= m_volume;
            
            for (int j = 0; j < a_channels; j++)
            {
                a_data[i + j] = whiteNoise;
            }
        }
    }
}
