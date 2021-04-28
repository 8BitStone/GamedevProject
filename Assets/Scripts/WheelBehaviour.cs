using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehaviour : MonoBehaviour
{
    public WheelCollider wheelCol; // wheel colider object
    public SkidmarkBehaviour skidmarks;
    private int _skidmarkLast;
    private Vector3 _skidmarkLastPos;

    private void Start() {
        _skidmarkLast = -1;
    }

    private void Update()
    {
        // Get the wheel position and rotation from the wheelcolider
        Quaternion quat;
        Vector3 position;
        wheelCol.GetWorldPose(out position, out quat);
        transform.position = position;
        transform.rotation = quat;
    }

    // Creates skidmarks if handbraking
    public void DoSkidmarking(bool doSkidmarking)
    {
        if (!doSkidmarking)
        {
            _skidmarkLast = -1;
            return;
        }

        WheelHit hit;
        if (!wheelCol.GetGroundHit(out hit)) return;

        // absolute velocity at wheel in world space
        Vector3 wheelVelo = wheelCol.attachedRigidbody.GetPointVelocity(hit.point);
        if (Vector3.Distance(_skidmarkLastPos, hit.point) > 0.1f)
        {
            _skidmarkLast = skidmarks.Add(
                hit.point + wheelVelo * Time.deltaTime,
                hit.normal,
                (hit.force - 1000) / 10000,
                _skidmarkLast
            );
            _skidmarkLastPos = hit.point;
        }
    }
}