using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class vTimer
{
    float m_timer;
    float m_timerMax;
    bool m_onceOff;
    internal bool m_finished;
    bool m_active;
    bool m_resetsTimerOnComplete;
    bool m_usingUnscaledDeltaTime;
    bool m_clampingTimer;

    public float GetTimer() { return m_timer; }
    public bool IsActive() { return m_active; }
    public void SetActive(bool a_active) { m_active = a_active; }
    internal void SetTimer(float a_timer) {m_timer = a_timer; }
    internal void SetTimerMax(float a_timerMax) { m_timerMax = a_timerMax; }
    internal void SetUsingUnscaledDeltaTime(bool a_using) { m_usingUnscaledDeltaTime = a_using; }
    internal void SetClampingTimer(bool a_clamping) { m_clampingTimer = a_clamping; }
    public float GetCompletionPercentage() { return m_timer / m_timerMax; }
    public float GetTimerMax() { return m_timerMax; }

    private void Init(float a_maxTime, bool a_onceOff, bool a_active, bool a_resetsTimerOnComplete, bool a_usingUnscaledDeltaTime, bool a_clampingTimer)
    {
        m_timer = 0f;
        m_timerMax = a_maxTime;
        m_onceOff = a_onceOff;
        m_active = a_active;
        m_resetsTimerOnComplete = a_resetsTimerOnComplete;
        m_usingUnscaledDeltaTime = a_usingUnscaledDeltaTime;
        m_clampingTimer = a_clampingTimer;
    }

    public vTimer(float a_maxTime, bool a_onceOff = false, bool a_active = true, bool a_resetsTimerOnComplete = true, bool a_usingUnscaledDeltaTime = false, bool a_clampingTimer = false)
    {
        Init(a_maxTime, a_onceOff, a_active, a_resetsTimerOnComplete, a_usingUnscaledDeltaTime, a_clampingTimer);
    }

    public void Reset()
    {
        m_timer = 0f;
        m_finished = false;
    }

    public bool Update()
    {
        if (!m_finished && m_active)
        {
            m_timer += m_usingUnscaledDeltaTime ? Time.unscaledDeltaTime: Time.deltaTime;

            //Debug.Log(m_timer);
        }

        if (m_timer >= m_timerMax && m_active)
        {
            if (m_clampingTimer)
            {
                m_timer = Mathf.Clamp(m_timer, 0f, m_timerMax);
            }
            if (m_onceOff)
            {
                m_finished = true;
            }
            if (m_resetsTimerOnComplete)
            {
                m_timer -= m_timerMax;
            }
            return true;
        }
        return false;
    }

    public bool Update(ref float a_timeElapsed)
    {
        bool retVal = Update();
        a_timeElapsed = m_timer;
        return retVal;
    }
}
