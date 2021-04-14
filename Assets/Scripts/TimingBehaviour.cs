using System.Collections;
using TMPro;
using UnityEngine;

public class TimingBehaviour : MonoBehaviour
{
    public CarBehaviour carBehaviour;
    public TMP_Text countdownText;
    public int countMax = 3;
    public AudioClip beepSoundClip;

    private int _countDown;
    private AudioSource _beeAudioSource;
    private float _timerStart;
    private bool _countdownRunning = true;

    private float _pastTime = 0;
    private bool _isFinished = false;
    private bool _isStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
        {
            if (!_isStarted)
            {
                _timerStart = Time.time;
                _isStarted = true;
            }
            else
            {
                _isFinished = true;
            }
        }
    }

    private void OnGUI()
    {
        if (!carBehaviour.thrustEnabled)
        {
            countdownText.text = (countMax - _pastTime).ToString("0.0") + " sec.";
        }
        else if(_isStarted)
        {
            countdownText.text = (_pastTime).ToString("0.0") + " sec.";
        }
    }


    private void Start()
    {
        carBehaviour.thrustEnabled = false;

        _beeAudioSource = carBehaviour.gameObject.AddComponent<AudioSource>();
        _beeAudioSource.clip = beepSoundClip;
        _beeAudioSource.loop = false;
        _beeAudioSource.volume = 0.7f;
        _beeAudioSource.playOnAwake = false;
        _beeAudioSource.enabled = false; // Bugfix
        _beeAudioSource.enabled = true; // Bugfix

        //print("Begin Start:" + Time.time);
        _timerStart = Time.time;
        StartCoroutine(GameStart());
        //print("End Start:" + Time.time);
    }

    private void Update()
    {
        if (carBehaviour.thrustEnabled && !_isStarted || _isFinished) { return; }
        _pastTime = Time.time - _timerStart;
    }

    IEnumerator GameStart()
    {
        //print(" Begin GameStart:" + Time.time);

        for (_countDown = countMax; _countDown > 0; _countDown--)
        {
            
            _beeAudioSource.Play();
            yield return new WaitForSeconds(1);
            //print(" WaitForSeconds:" + Time.time);
        }

        //print(" End GameStart:" + Time.time);


        _beeAudioSource.pitch *= 1.5f;
        _beeAudioSource.Play();

        carBehaviour.thrustEnabled = true;

        countdownText.text = "GO!";
    }
}
