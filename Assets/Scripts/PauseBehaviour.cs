using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseBehaviour : MonoBehaviour
{

    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderRL;
    public WheelCollider wheelColliderRR;

    public Material carMaterial;

    private Prefs _prefs;

    private void Start()
    {
        _prefs = new Prefs();
        _prefs.Load();
        _prefs.SetAll(ref wheelColliderFL, ref wheelColliderFR, ref wheelColliderRL, ref wheelColliderRR, ref carMaterial);
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void OnApplicationQuit()
    {
        _prefs.Save();
    }
}
