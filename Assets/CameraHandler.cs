using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] float m_minZoom;
    [SerializeField] float m_maxZoom;
    float m_shakeAmount = 0f;
    Vector3 m_originalPosition;
    Camera m_cameraRef;

    internal void SetShakeAmount(float a_shake) { m_shakeAmount = a_shake;} 

    internal void ChangeZoom(float a_change)
    {
        m_cameraRef.fieldOfView = Mathf.Clamp( m_cameraRef.fieldOfView + a_change, m_minZoom, m_maxZoom);
    }

    private void Awake()
    {
        m_cameraRef = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_originalPosition, Time.deltaTime * 5f);
        transform.localPosition += m_shakeAmount * Time.deltaTime * VLib.RandomVector3Direction();
    }
}
