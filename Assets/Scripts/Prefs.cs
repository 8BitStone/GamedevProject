using UnityEngine;
public class Prefs
{
    public float suspensionDistance;
    public float suspensionForce;
    public float suspensionDampening;

    public float hue;
    public float saturation;
    public float value;

    public void Load()
    {
        suspensionDistance = PlayerPrefs.GetFloat("suspensionDistance", 0.2f);
        suspensionForce = PlayerPrefs.GetFloat("suspensionForce", 0.2f);
        suspensionDampening = PlayerPrefs.GetFloat("suspensionDampening", 0.2f);

        hue = PlayerPrefs.GetFloat("carHue", 0.2f);
        saturation = PlayerPrefs.GetFloat("carSaturation", 0.2f);
        value = PlayerPrefs.GetFloat("carValue", 0.2f);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("suspensionDistance", suspensionDistance);
        PlayerPrefs.SetFloat("suspensionForce", suspensionForce);
        PlayerPrefs.SetFloat("suspensionDampening", suspensionDampening);

        PlayerPrefs.SetFloat("carHue", hue);
        PlayerPrefs.SetFloat("carSaturation", saturation);
        PlayerPrefs.SetFloat("carValue", value);
    }

    public void SetAll(ref WheelCollider wheelFL, ref WheelCollider wheelFR, ref WheelCollider wheelRL, ref WheelCollider wheelRR, ref Material carMaterial)
    {
        SetWheelColliderSuspension(ref wheelFL, ref wheelFR, ref wheelRL, ref wheelRR);
        SetColor(ref carMaterial);
    }

    public void SetWheelColliderSuspension(ref WheelCollider wheelFL, ref WheelCollider wheelFR, ref WheelCollider wheelRL, ref WheelCollider wheelRR)
    {
        wheelFL.suspensionDistance = suspensionDistance;
        wheelFR.suspensionDistance = suspensionDistance;
        wheelRL.suspensionDistance = suspensionDistance;
        wheelRR.suspensionDistance = suspensionDistance;

        Debug.Log(wheelFL.suspensionSpring.damper);

        wheelFL.suspensionSpring = new JointSpring { spring = suspensionForce, damper = suspensionDampening, targetPosition = wheelFL.suspensionSpring.targetPosition };
        wheelFR.suspensionSpring = new JointSpring { spring = suspensionForce, damper = suspensionDampening, targetPosition = wheelFR.suspensionSpring.targetPosition };
        wheelRL.suspensionSpring = new JointSpring { spring = suspensionForce, damper = suspensionDampening, targetPosition = wheelRL.suspensionSpring.targetPosition };
        wheelRR.suspensionSpring = new JointSpring { spring = suspensionForce, damper = suspensionDampening, targetPosition = wheelRR.suspensionSpring.targetPosition };
    }

    public void SetColor(ref Material carMaterial)
    {
        carMaterial.color = Color.HSVToRGB(hue, saturation, value);
    }
}