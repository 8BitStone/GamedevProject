using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderRL;
    public WheelCollider wheelColliderRR;

    public Material carMaterial;

    public Slider sldDistance;
    public Slider sldForce;
    public Slider sldDampening;
    public Text txtDistance;
    public Text txtForce;
    public Text txtDampening;

    public Slider sldHue;
    public Slider sldSaturation;
    public Slider sldValue;
    public Text txtHue;
    public Text txtSaturation;
    public Text txtValue;

    private Prefs _prefs;

    private void Start()
    {
        _prefs = new Prefs();
        _prefs.Load();
        _prefs.SetAll(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR, ref carMaterial);

        sldDistance.value   = _prefs.suspensionDistance;
        sldForce.value      = _prefs.suspensionForce;
        sldDampening.value  = _prefs.suspensionDampening;

        sldHue.value        = _prefs.hue;
        sldSaturation.value = _prefs.saturation;
        sldValue.value      = _prefs.value;

        OnSliderChangedSuspDistance(sldDistance.value);
        OnSliderChangedSuspForce(sldForce.value);
        OnSliderChangedSuspDampening(sldDampening.value);

        OnSliderChangedHue(sldHue.value);
        OnSliderChangedSaturation(sldSaturation.value);
        OnSliderChangedValue(sldValue.value);
    }

    public void OnStartClick()
    {
        _prefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void OnSliderChangedSuspDistance(float value)
    {
        txtDistance.text = value.ToString("0.00");
        _prefs.suspensionDistance = value;
        _prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
    }

    public void OnSliderChangedSuspForce(float value)
    {
        txtForce.text = value.ToString("0.00");
        _prefs.suspensionForce = value;
        _prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
    }

    public void OnSliderChangedSuspDampening(float value)
    {
        txtDampening.text = value.ToString("0.00");
        _prefs.suspensionDampening = value;
        _prefs.SetWheelColliderSuspension(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR);
    }

    public void OnSliderChangedHue(float value)
    {
        txtHue.text = value.ToString("0.00");
        _prefs.hue = value;
        _prefs.SetColor(ref carMaterial);
    }

    public void OnSliderChangedSaturation(float value)
    {
        txtSaturation.text = value.ToString("0.00");
        _prefs.saturation = value;
        _prefs.SetColor(ref carMaterial);
    }

    public void OnSliderChangedValue(float value)
    {
        txtValue.text = value.ToString("0.00");
        _prefs.value = value;
        _prefs.SetColor(ref carMaterial);
    }

    private void OnApplicationQuit()
    {
        _prefs.Save();
    }
}
