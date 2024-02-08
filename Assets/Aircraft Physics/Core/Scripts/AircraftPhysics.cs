using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AircraftPhysics : MonoBehaviour
{
    const float PREDICTION_TIMESTEP_FRACTION = 0.5f;

    [SerializeField] 
    internal float thrust = 0;
    [SerializeField] 
    List<AeroSurface> aerodynamicSurfaces = null;
    [SerializeField]
    List<AeroWeight> m_aeroWeights = null;

    Rigidbody m_rigidbody;
    float m_thrustPercent;
    float m_stallShake = 0f;
    BiVector3 currentForceAndTorque;

    internal List<AeroSurface> GetAeroSurfaces()
    {
        return aerodynamicSurfaces;
    }

    internal float GetStallShake()
    {
        float stallShake = 0f;
        return stallShake;
    }

    public void SetThrustPercent(float percent)
    {
        m_thrustPercent = percent;
    }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        aerodynamicSurfaces = new List<AeroSurface>(GetComponentsInChildren<AeroSurface>());
        float mass = 0f;
        for (int i = 0; i < m_aeroWeights.Count; i++)
        {
            mass += m_aeroWeights[i].GetWeight();
        }
        m_rigidbody.mass = mass;
    }

    private void FixedUpdate()
    {
        BiVector3 forceAndTorqueThisFrame = 
            CalculateAerodynamicForces(m_rigidbody.velocity, m_rigidbody.angularVelocity, Vector3.zero, 1.2f, m_rigidbody.worldCenterOfMass);

        Vector3 velocityPrediction = PredictVelocity(forceAndTorqueThisFrame.p
            + transform.forward * thrust * m_thrustPercent + Physics.gravity * m_rigidbody.mass);
        Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.q);

        BiVector3 forceAndTorquePrediction = 
            CalculateAerodynamicForces(velocityPrediction, angularVelocityPrediction, Vector3.zero, 1.2f, m_rigidbody.worldCenterOfMass);

        currentForceAndTorque = (forceAndTorqueThisFrame + forceAndTorquePrediction) * 0.5f;
        m_rigidbody.AddForce(currentForceAndTorque.p);
        m_rigidbody.AddTorque(currentForceAndTorque.q);

        Vector3 forwardForce = transform.forward * thrust * m_thrustPercent;
        m_rigidbody.AddForce(forwardForce);
    }

    private BiVector3 CalculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 wind, float airDensity, Vector3 centerOfMass)
    {
        BiVector3 forceAndTorque = new BiVector3();
        foreach (var surface in aerodynamicSurfaces)
        {
            Vector3 relativePosition = surface.transform.position - centerOfMass;
            forceAndTorque += surface.CalculateForces(-velocity + wind -Vector3.Cross(angularVelocity, relativePosition), airDensity, relativePosition);
        }
        return forceAndTorque;
    }

    private Vector3 PredictVelocity(Vector3 force)
    {
        return m_rigidbody.velocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION * force / m_rigidbody.mass;
    }

    private Vector3 PredictAngularVelocity(Vector3 torque)
    {
        Quaternion inertiaTensorWorldRotation = m_rigidbody.rotation * m_rigidbody.inertiaTensorRotation;
        Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque;
        Vector3 angularVelocityChangeInDiagonalSpace;
        angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / m_rigidbody.inertiaTensor.x;
        angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / m_rigidbody.inertiaTensor.y;
        angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / m_rigidbody.inertiaTensor.z;

        return m_rigidbody.angularVelocity + Time.fixedDeltaTime * PREDICTION_TIMESTEP_FRACTION
            * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
    }

    private Vector3 CalculateCentreOfMass()
    {
        Vector3 centreOfMass = new Vector3();
        float sumMass = 0f;
        for (int i = 0; i < m_aeroWeights.Count; i++)
        {
            centreOfMass += m_aeroWeights[i].transform.localPosition * m_aeroWeights[i].GetWeight();
            sumMass += m_aeroWeights[i].GetWeight();
        }

        centreOfMass /= sumMass;

        return centreOfMass;
    }

#if UNITY_EDITOR
    // For gizmos drawing.
    public void CalculateCenterOfLift(out Vector3 center, out Vector3 force, Vector3 displayAirVelocity, float displayAirDensity)
    {
        Vector3 com;
        BiVector3 forceAndTorque;
        if (aerodynamicSurfaces == null)
        {
            center = Vector3.zero;
            force = Vector3.zero;
            return;
        }


        if (m_rigidbody == null)
        {
            GetComponent<Rigidbody>().centerOfMass = CalculateCentreOfMass();
            com = GetComponent<Rigidbody>().worldCenterOfMass;
            forceAndTorque = CalculateAerodynamicForces(-displayAirVelocity, Vector3.zero, Vector3.zero, displayAirDensity, com);
        }
        else
        {
            m_rigidbody.centerOfMass = CalculateCentreOfMass();
            com = m_rigidbody.worldCenterOfMass;
            forceAndTorque = currentForceAndTorque;
        }

        force = forceAndTorque.p;
        center = com + Vector3.Cross(forceAndTorque.p, forceAndTorque.q) / forceAndTorque.p.sqrMagnitude;
    }
#endif
}


