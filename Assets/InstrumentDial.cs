using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstrumentDial : MonoBehaviour
{
    [SerializeField] Canvas m_canvasRef;
    [SerializeField] GameObject m_needlePrefab;
    [SerializeField] GameObject m_textPrefab;
    [SerializeField] GameObject m_readingNeedleRef;
    [SerializeField] GameObject m_secondReadingNeedleRef;
    [SerializeField] Transform m_outerMarkingsContainerRef;
    [SerializeField] TextMeshProUGUI m_dialTitleTextRef;

    float m_faceRadius = 102f;
    int m_maxReading = 400;
    float m_outerNeedleHalfGap = 45f;
    float m_measuredValue = 0f;
    bool m_showingSecondNeedle = false;
    float m_secondNeedleScale;

    internal void SetValue(float a_value) { m_measuredValue = a_value; RefreshReadingNeedle(); }

    // Start is called before the first frame update
    void Start()
    {
    }

    internal void Init(string a_titleString, int a_majorDivisionAmount, int a_majorDivisions, int a_subDivisions, float a_outerNeedleHalfGap = 45f, bool a_dualNeedle = false, float a_secondNeedleScale = 10f)
    {
        m_dialTitleTextRef.text = a_titleString;
        m_outerNeedleHalfGap = a_outerNeedleHalfGap;
        m_maxReading = a_majorDivisionAmount * (a_majorDivisions-1);
        SpawnFaceMarkings(a_majorDivisions, a_subDivisions);
        m_showingSecondNeedle = a_dualNeedle;
        m_secondReadingNeedleRef.SetActive(m_showingSecondNeedle);
        m_secondNeedleScale = a_secondNeedleScale;
    }

    void SpawnOuterNeedle(float a_angle, float a_size, bool a_spawningText = false, int a_textValue = 0)
    {
        GameObject needle = Instantiate(m_needlePrefab, m_outerMarkingsContainerRef);
        needle.transform.eulerAngles = new Vector3(0f, 0f, a_angle);
        a_angle += 180f;
        Vector3 spawnPos = new Vector3(0f, m_faceRadius, 0f).RotateVector3In2D(a_angle);
        needle.transform.localPosition = spawnPos;
        needle.transform.localScale *= a_size;
        if (a_spawningText)
        {
            TextMeshProUGUI text = Instantiate(m_textPrefab, m_outerMarkingsContainerRef).GetComponent<TextMeshProUGUI>();
            text.gameObject.transform.localPosition = spawnPos * 0.75f;
            text.text = a_textValue.ToString();
        }
    }

    void SpawnFaceMarkings(int a_majorDivisions, int a_subDivisions)
    {
        for (int i = 0; i < a_majorDivisions; i++)
        {
            float angularSpaceUsed = (360f - m_outerNeedleHalfGap * 2f);
            float majorDivisionGap = angularSpaceUsed / (a_majorDivisions - 1);
            float angle = -m_outerNeedleHalfGap - majorDivisionGap * i;
            bool spawningText = m_outerNeedleHalfGap == 0f ? i < a_majorDivisions - 1 : true;
            SpawnOuterNeedle(angle, 1f, spawningText, (i * m_maxReading)/ (a_majorDivisions-1));

            if (i < a_majorDivisions-1)
            {
                for (int j = 0; j < a_subDivisions; j++)
                {
                    SpawnOuterNeedle(angle - (majorDivisionGap/(a_subDivisions+1)) * (j+1), 0.5f);
                }
            }
        }
    }

    void RefreshReadingNeedle()
    {
        float reading = m_measuredValue / m_maxReading;
        if (!m_showingSecondNeedle)
        {
            reading = Mathf.Clamp(reading, 0f, 1f);
        }
        float angle = 0f;
        angle -= m_outerNeedleHalfGap + reading * (360f - m_outerNeedleHalfGap * 2f);
        m_readingNeedleRef.transform.localEulerAngles = new Vector3(0f, 0f, angle + 180f);
        if (m_showingSecondNeedle)
        {
            float secondAngle = 180f + angle / m_secondNeedleScale;
            m_secondReadingNeedleRef.transform.localEulerAngles = new Vector3(0f, 0f, secondAngle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
