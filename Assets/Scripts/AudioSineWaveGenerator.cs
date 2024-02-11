using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSineWaveGenerator : MonoBehaviour
{
    System.Random m_random = new System.Random();
    [SerializeField] float m_volume = 0.5f;
    [SerializeField] float m_whiteNoiseVolume = 0.5f;
    [Range(0f, 40000)]
    [SerializeField] float m_baseFrequency = 861.62f; //middle C
    float m_frequency;
    float m_baseLowPassFilterPosition = 1300f;
    float m_baseHighPassFilterPosition = 250f;
    [Range(-1f,2f)]
    [SerializeField] float m_bandPassFilterPosition = 0f;

    float m_whiteNoiseStrength = 0.1f;
    float m_bandPassFilterGap = 1000f;

    AudioLowPassFilter m_audioLowPassFilter;
    AudioHighPassFilter m_audioHighPassFilter;

    private double m_phase = 0f;
    private int m_sampleRate;

    // Start is called before the first frame update
    void Start()
    {
        m_sampleRate = AudioSettings.outputSampleRate;
        m_frequency = m_baseFrequency;
        m_audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        m_audioHighPassFilter = GetComponent<AudioHighPassFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        float bandPassFilterPosition = m_bandPassFilterPosition;// Mathf.Sin(Time.time) * 0.1f;
        m_audioLowPassFilter.cutoffFrequency = m_baseLowPassFilterPosition * (1f + bandPassFilterPosition);
        m_audioHighPassFilter.cutoffFrequency = m_baseHighPassFilterPosition * (1f + bandPassFilterPosition);
    }

    void OnAudioFilterRead(float[] a_data, int a_channels)
    {
        double phaseIncrement = m_frequency / m_sampleRate;

        for (int i = 0; i < a_data.Length; i += a_channels)
        {
            float value = 0f;// Mathf.Sin((float) m_phase * 2f * Mathf.PI) * m_volume;
            m_phase = (m_phase + phaseIncrement) % 1;
            float whiteNoise = (float)(m_random.NextDouble() * 2 - 1);
            whiteNoise *= m_whiteNoiseStrength;
            whiteNoise *= m_whiteNoiseVolume;
            value += whiteNoise;
            
            for (int j = 0; j < a_channels; j++)
            {
                a_data[i + j] = value;
            }
        }
    }
}
