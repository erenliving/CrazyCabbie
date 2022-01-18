using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float ImpulseRangeX = 0.0f;
    public float ImpulseRangeZ = 0.0f;
    public float ImpulseY = 0.0f;
    public float SuspensionLength = 0.0f;
    public float SuspensionStrength = 0.0f;
    public float SuspensionDampening = 0.0f;
    public float MaxMotorTorque = 0.0f;
    public float MaxSteeringAngle = 0.0f;

    private Rigidbody carBody;
    private float carMass;
    private Vector3 extents;
    private float verticalInput;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        carBody = GetComponent<Rigidbody>();
        carMass = carBody.mass;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        extents = meshFilter.mesh.bounds.extents;
        // Debug.Log("Extents: " + extents);
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Debug button to flip car
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // TODO: should make this more of a random rotation/torque about the transform origin point, with some vertical force added
            float impulseX = Random.Range(-ImpulseRangeX, ImpulseRangeX);
            float impulseZ = Random.Range(-ImpulseRangeZ, ImpulseRangeZ);
            Vector3 impulse = new Vector3(impulseX, ImpulseY, impulseZ);
            carBody.AddForce(impulse, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Get current wheel positions in world space
        Vector3 wheelFL = transform.TransformPoint(-extents.x, -extents.y, extents.z);
        Vector3 wheelFR = transform.TransformPoint(extents.x, -extents.y, extents.z);
        Vector3 wheelBL = transform.TransformPoint(-extents.x, -extents.y, -extents.z);
        Vector3 wheelBR = transform.TransformPoint(extents.x, -extents.y, -extents.z);
        // Debug.Log("WheelFL " + wheelFL);

        // Get the down direction in world space
        Vector3 dwn = transform.TransformDirection(Vector3.down);
        // Debug.Log("dwn world vector " + dwn);

        // TODO: figure out dampened spring mechanics
        RaycastHit wheelFLHit;
        RaycastHit wheelFRHit;
        RaycastHit wheelBLHit;
        RaycastHit wheelBRHit;
        if (Physics.Raycast(wheelFL, dwn, out wheelFLHit, SuspensionLength))
        {
            Debug.DrawRay(wheelFL, dwn * wheelFLHit.distance, Color.red);
            // Debug.Log("Wheel FL hit " + wheelFLHit.distance);
            float x = SuspensionLength - wheelFLHit.distance;
            float velocity = carBody.GetPointVelocity(wheelFL).y;
            float springForce = CalculateDampenedSpringForce(x, velocity);
            // Debug.Log("springConstant: " + springConstant + ", x: " + x + ", springForce: " + springForce);
            Vector3 wheelFLForce = dwn * springForce;
            // Debug.Log("wheelFLForce " + wheelFLForce);
            carBody.AddForceAtPosition(wheelFLForce, wheelFL, ForceMode.Force);
        }
        if (Physics.Raycast(wheelFR, dwn, out wheelFRHit, SuspensionLength))
        {
            Debug.DrawRay(wheelFR, dwn * wheelFRHit.distance, Color.red);
            // Debug.Log("Wheel FR hit " + wheelFRHit.distance);
            float x = SuspensionLength - wheelFRHit.distance;
            float velocity = carBody.GetPointVelocity(wheelFR).y;
            float springForce = CalculateDampenedSpringForce(x, velocity);
            // Debug.Log("springConstant: " + springConstant + ", x: " + x + ", springForce: " + springForce);
            Vector3 wheelFRForce = dwn * springForce;
            // Debug.Log("wheelFRForce " + wheelFRForce);
            carBody.AddForceAtPosition(wheelFRForce, wheelFR, ForceMode.Force);
        }
        if (Physics.Raycast(wheelBL, dwn, out wheelBLHit, SuspensionLength))
        {
            Debug.DrawRay(wheelBL, dwn * wheelBLHit.distance, Color.red);
            // Debug.Log("Wheel BL hit " + wheelBLHit.distance);
            float x = SuspensionLength - wheelBLHit.distance;
            float velocity = carBody.GetPointVelocity(wheelBL).y;
            float springForce = CalculateDampenedSpringForce(x, velocity);
            // Debug.Log("springConstant: " + springConstant + ", x: " + x + ", springForce: " + springForce);
            Vector3 wheelBLForce = dwn * springForce;
            // Debug.Log("wheelBLForce " + wheelBLForce);
            carBody.AddForceAtPosition(wheelBLForce, wheelBL, ForceMode.Force);
        }
        if (Physics.Raycast(wheelBR, dwn, out wheelBRHit, SuspensionLength))
        {
            Debug.DrawRay(wheelBR, dwn * wheelBRHit.distance, Color.red);
            // Debug.Log("Wheel BR hit " + wheelBRHit.distance);
            float x = SuspensionLength - wheelBRHit.distance;
            float velocity = carBody.GetPointVelocity(wheelBR).y;
            float springForce = CalculateDampenedSpringForce(x, velocity);
            // Debug.Log("springConstant: " + springConstant + ", x: " + x + ", springForce: " + springForce);
            Vector3 wheelBRForce = dwn * springForce;
            // Debug.Log("wheelBRForce " + wheelBRForce);
            carBody.AddForceAtPosition(wheelBRForce, wheelBR, ForceMode.Force);
        }

        // TODO: apply Input controls and gas/brake/reverse physics
        float motor = MaxMotorTorque * verticalInput;
        float steering = MaxSteeringAngle * horizontalInput;
        carBody.AddForce(transform.forward * motor, ForceMode.Acceleration);
        carBody.AddForce(transform.right * steering, ForceMode.Force);

    }

    private float CalculateDampenedSpringForce(float x, float velocity)
    {
        // Note: dampening is inverted here due to suspension pushing down on ground instead of up on car body
        return -1.0f * SuspensionStrength * x + (SuspensionDampening * velocity);
    }
}
