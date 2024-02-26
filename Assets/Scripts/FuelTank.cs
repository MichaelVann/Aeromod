using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : MonoBehaviour
{
    [SerializeField] float m_capacity = 140L;
    const float m_kgPerL = 0.740f;
    float m_fuel;

    internal float GetFuel() {  return m_fuel; }

    internal float GetCapacity() { return m_capacity;}

    internal void ChangeFuelAmount(float a_change) { m_fuel += a_change; ClampFuel(); }

    void ClampFuel() { m_fuel = Mathf.Clamp(m_fuel, 0f, m_capacity); }

    // Start is called before the first frame update
    void Awake()
    {
        m_fuel = m_capacity;
        GetComponent<AeroWeight>().SetWeight(m_fuel * m_kgPerL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
