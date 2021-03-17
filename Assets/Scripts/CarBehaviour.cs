using UnityEngine;


public class CarBehaviour : MonoBehaviour
{
   public WheelCollider wheelColliderFL;
   public WheelCollider wheelColliderFR;
   public WheelCollider wheelColliderRL;
   public WheelCollider wheelColliderRR;
   public float maxTorque = 500;
   public float maxSteerAngle = 45;
   public float sidewaysStiffness = 1.5f;
   public float forewardStiffness = 1.5f;
   public float MaxSpeedKmh = 150f;
   public float MaxReverseSpeedKmh = 30f;
   public Transform centerOfMass;


   private float _currentSpeedKMH;
   private Rigidbody _rigidBody;
   private bool _isHeadingForward;

   void Start() {
      SetWheelFrictionStiffness(forewardStiffness, sidewaysStiffness);
      _rigidBody = GetComponent<Rigidbody>();
      _rigidBody.centerOfMass = new Vector3(centerOfMass.localPosition.x, centerOfMass.localPosition.y, centerOfMass.localPosition.z);
   }

   void FixedUpdate()
   {
      _currentSpeedKMH = _rigidBody.velocity.magnitude * 3.6f;
      _isHeadingForward = Vector3.Angle(_rigidBody.velocity, transform.forward) < 90;

      float accelerationModifyer = _isHeadingForward ? (_currentSpeedKMH > MaxSpeedKmh ? 0 : 1) : (_currentSpeedKMH > MaxReverseSpeedKmh ? 0 : 1);

      if (isBraking())
      {
         SetBreakTorque(5000);
         SetMotorTorque(0);
      }
      else
      {
         SetBreakTorque(0);
         SetMotorTorque(maxTorque * Input.GetAxis("Vertical") * accelerationModifyer);
      }

      SetSteerAngle(maxSteerAngle * Input.GetAxis("Horizontal"));
   }

   void SetSteerAngle(float angle)
   {
      float speedClampedMaxAngle = maxSteerAngle * (_isHeadingForward ? 1 - _currentSpeedKMH / (1.1f * MaxSpeedKmh) : 1 - _currentSpeedKMH / (1.1f * MaxReverseSpeedKmh));
      angle = Mathf.Clamp(angle, -speedClampedMaxAngle, speedClampedMaxAngle);
      wheelColliderFL.steerAngle = angle;
      wheelColliderFR.steerAngle = angle;
   }

   void SetMotorTorque(float amount)
   {
      wheelColliderFL.motorTorque = amount;
      wheelColliderFR.motorTorque = amount;
   }

   void SetBreakTorque(float amount)
   {
      wheelColliderFL.brakeTorque = amount;
      wheelColliderFR.brakeTorque = amount;
      wheelColliderRL.brakeTorque = amount;
      wheelColliderRR.brakeTorque = amount;
   }

   void SetWheelFrictionStiffness(float forewardStiffness, float sidewaysStiffness)
   {
      WheelFrictionCurve f_fwWFC = wheelColliderFL.forwardFriction;
      WheelFrictionCurve f_swWFC = wheelColliderFL.sidewaysFriction;
      f_fwWFC.stiffness = forewardStiffness;
      f_swWFC.stiffness = sidewaysStiffness;
      wheelColliderFL.forwardFriction = f_fwWFC;
      wheelColliderFL.sidewaysFriction = f_swWFC;
      wheelColliderFR.forwardFriction = f_fwWFC;
      wheelColliderFR.sidewaysFriction = f_swWFC;
      wheelColliderRL.forwardFriction = f_fwWFC;
      wheelColliderRL.sidewaysFriction = f_swWFC;
      wheelColliderRR.forwardFriction = f_fwWFC;
      wheelColliderRR.sidewaysFriction = f_swWFC;
   }

   private bool isBraking()
   {
      return _currentSpeedKMH > 0.5f &&
         (Input.GetAxis("Vertical") < 0 && _isHeadingForward ||
         Input.GetAxis("Vertical") > 0 && !_isHeadingForward);
   }
}