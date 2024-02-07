using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerHandler : MonoBehaviour
{
    [SerializeField]Camera m_cameraRef;
    float m_walkSpeed = 6f;
    float m_runSpeed = 12f;
    float m_jumpPower = 7f;
    float m_gravity = 9.89f;

    float m_lookSpeed = 2f;
    float m_lookXLimit = 90f;


    Vector3 m_velocity = Vector3.forward;
    float m_velocityFriction = 0.97f;
    float m_rotationX = 0;
    bool m_canMove = true;

    CharacterController m_characterController;
    Rigidbody m_rigidbodyRef;
    Aircraft m_boardedAircraft;
    [SerializeField] Collider m_collider;

    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_rigidbodyRef = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterControllerMovement();

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (m_boardedAircraft == null)
            {
                if (Physics.Raycast(transform.position, m_cameraRef.transform.forward, out hit))
                {
                    Aircraft aircraft = hit.transform.GetComponent<Aircraft>();
                    if (aircraft != null)
                    {
                        BoardAircraft(aircraft);
                    }
                }
            }
            else
            {
                UnboardAircraft();
            }
        }
    }

    void BoardAircraft(Aircraft a_aircraft)
    {
        m_boardedAircraft = a_aircraft;
        transform.position = a_aircraft.m_seatPosition.position;
        transform.parent = a_aircraft.gameObject.transform;
        transform.localEulerAngles = Vector3.zero;
        m_cameraRef.transform.localEulerAngles = Vector3.zero;
        m_collider.enabled = false;
        m_boardedAircraft.SetPlayerRef(this);
    }

    void UnboardAircraft()
    {
        transform.position = m_boardedAircraft.m_exitSeatPosition.position;
        m_collider.enabled = true;
        m_boardedAircraft.SetPlayerRef(null);

        m_boardedAircraft = null;
    }

    void CharacterControllerMovement()
    {
        m_canMove = m_boardedAircraft == null;
        if (m_canMove)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float currentSpeedX = m_canMove ? (isRunning ? m_runSpeed : m_walkSpeed) * Input.GetAxis("Horizontal") : 0f;
            float currentSpeedZ = m_canMove ? (isRunning ? m_runSpeed : m_walkSpeed) * Input.GetAxis("Vertical") : 0f;
            float movementDirectionY = m_velocity.y;
            Vector3 characterMovement = ((forward * currentSpeedZ) + (right * currentSpeedX));

            if (!m_characterController.isGrounded)
            {
                m_velocity.y = movementDirectionY - m_gravity * Time.deltaTime;
            }
            else
            {
                if (Input.GetKey(KeyCode.Space) && m_canMove)
                {
                    Vector3 jumpVector = Vector3.up;
                    //RaycastHit hit = new RaycastHit();
                    //if (Physics.Raycast(transform.position, -transform.up, out hit))
                    //{
                    //    jumpVector = hit.normal;
                    //    //Debug.Log(jumpVector);
                    //}
                    m_velocity += m_jumpPower * jumpVector;// m_jumpPower * jumpVector;
                }
                else
                {
                    m_velocity.y = 0f;
                }
            }

            Debug.Log(m_velocity);

            m_velocity = new Vector3(m_velocity.x * m_velocityFriction, m_velocity.y, m_velocity.z * m_velocityFriction);

            m_characterController.Move((m_velocity + characterMovement) * Time.deltaTime);

            m_rotationX += -Input.GetAxis("Mouse Y") * m_lookSpeed;
            m_rotationX = Mathf.Clamp(m_rotationX, -m_lookXLimit, m_lookXLimit);
            m_cameraRef.transform.localRotation = Quaternion.Euler(m_rotationX, 0f, 0f);
            transform.rotation *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * m_lookSpeed, 0f);
        }
        m_characterController.enabled = m_canMove;
    }
}
