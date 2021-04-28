using TMPro;
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
    public RectTransform speedPointerTransform;
    public TMP_Text speedText;
    public AudioClip engineSingleRPMSoundClip;
    public ParticleSystem smokeL;
    public ParticleSystem smokeR;
    public ParticleSystem[] dustEmissions;
    public bool thrustEnabled;
    public float fullBrakeTorque = 5000;
    public float maxBrakeTorque = 4000;
    public AudioClip brakeAudioClip;
    public WheelBehaviour[] wheelBehaviours = new WheelBehaviour[0];

    private float _currentSpeedKMH;
    private Rigidbody _rigidBody;
    private bool _isHeadingForward;
    private AudioSource _engineAudioSource;
    private Transform _transform;
    private ParticleSystem.EmissionModule _smokeLEmission;
    private ParticleSystem.EmissionModule _smokeREmission;
    private bool _carIsOnDrySand = false;
    private bool _carIsOnStone = false;
    private bool _carSlips = false;
    private string _groundTagFL;
    private string _groundTagRL;
    private int _groundTextureFL;
    private int _groundTextureRL;
    private bool _doSkidmarking;
    private AudioSource _brakeAudioSource;


    void Start()
    {
        SetWheelFrictionStiffness(forewardStiffness, sidewaysStiffness);
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.centerOfMass = new Vector3(centerOfMass.localPosition.x, centerOfMass.localPosition.y, centerOfMass.localPosition.z);

        // Configure AudioSource component by program
        _engineAudioSource = gameObject.AddComponent<AudioSource>();
        _engineAudioSource.clip = engineSingleRPMSoundClip;
        _engineAudioSource.loop = true;
        _engineAudioSource.volume = 0.7f;
        _engineAudioSource.playOnAwake = true;
        _engineAudioSource.enabled = false; // Bugfix
        _engineAudioSource.enabled = true; // Bugfix

        // Configure brake audiosource component by program
        _brakeAudioSource = gameObject.AddComponent<AudioSource>();
        _brakeAudioSource.clip = brakeAudioClip;
        _brakeAudioSource.loop = true;
        _brakeAudioSource.volume = 0.7f;
        _brakeAudioSource.playOnAwake = false;

        _transform = gameObject.GetComponent<Transform>();

        _smokeLEmission = smokeL.emission;
        _smokeREmission = smokeR.emission;
        _smokeLEmission.enabled = true;
        _smokeREmission.enabled = true;
    }

    void FixedUpdate()
    {
        _currentSpeedKMH = Vector3.Project(_rigidBody.velocity, _transform.forward).magnitude * 3.6f; //To prevent falling in some random direction from being accounted towards the current speed
        _isHeadingForward = Vector3.Angle(_rigidBody.velocity, transform.forward) < 90;

        WheelHit hitFL = GetGroundInfos(ref wheelColliderFL, ref _groundTagFL, ref _groundTextureFL);
        WheelHit hitRL = GetGroundInfos(ref wheelColliderRL, ref _groundTagRL, ref _groundTextureRL);
        _carIsOnDrySand = _groundTagFL.CompareTo("Terrain") == 0 && _groundTextureFL == 0;
        _carIsOnStone = _groundTagFL.CompareTo("Terrain") == 0 && _groundTextureFL == 2;
        _carSlips = hitRL.sidewaysSlip > 0.3f || hitRL.sidewaysSlip < -0.3f;

        _doSkidmarking = false;

        if (isFullBraking())
        {
            _doSkidmarking = _carIsOnStone && _currentSpeedKMH > 20.0f;

            SetBreakTorque(fullBrakeTorque);
            SetMotorTorque(0);

        }
        else if (thrustEnabled)
        {
            if (isBraking())
            {
                SetBreakTorque(maxBrakeTorque);
                SetMotorTorque(0);
            }
            else
            {
                float accelerationModifyer = _isHeadingForward ? (_currentSpeedKMH > MaxSpeedKmh ? 0 : 1) : (_currentSpeedKMH > MaxReverseSpeedKmh ? 0 : 1);
                SetBreakTorque(0);
                SetMotorTorque(maxTorque * Input.GetAxis("Vertical") * accelerationModifyer);
            }
        }

        SetBrakeSound(_doSkidmarking);
        SetSkidmarking(_doSkidmarking || _carSlips);

        SetSteerAngle(maxSteerAngle * Input.GetAxis("Horizontal"));

        int gearNum = 0;
        float engineRPM = kmh2rpm(_currentSpeedKMH, out gearNum);
        SetEngineSound(engineRPM);

        SetParticleSystems(engineRPM);
    }

    void OnGUI()
    {
        // Speedpointer rotation
        float degAroundZ = -34 - 292/140 * _currentSpeedKMH;
        speedPointerTransform.rotation = Quaternion.Euler(0, 0, degAroundZ);
        // SpeedText show current KMH
        speedText.text = _currentSpeedKMH.ToString("0") + " km/h";
    }


    void SetSteerAngle(float angle)
    {
        float speedClampedMaxAngle = Mathf.Clamp(maxSteerAngle * (1 - _currentSpeedKMH / (1.2f * MaxSpeedKmh)), 0, maxSteerAngle);
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

    private bool isFullBraking()
    {
        return Input.GetKey("space") && _currentSpeedKMH > 0.5f;
    }

    private void SetEngineSound(float engineRPM)
    {
        if (_engineAudioSource == null) return;
        float minRPM = 800;
        float maxRPM = 8000;
        float minPitch = 0.3f;
        float maxPitch = 3.0f;
        float pitch = minPitch + (engineRPM - minRPM) / (maxRPM - minRPM) * (maxPitch - minPitch);
        _engineAudioSource.pitch = pitch;
    }

    private void SetBrakeSound(bool doBrakeSound)
    {
        if (doBrakeSound)
        {
            _brakeAudioSource.volume = _currentSpeedKMH / 100.0f;
            _brakeAudioSource.Play();
        }
        else
            _brakeAudioSource.Stop();
    }


    private float kmh2rpm(float kmh, out int gearNum)
    {
        Gear[] gears =
        { 
            new Gear(  1,  900,  12, 1400),
            new Gear( 12,  900,  25, 2000),
            new Gear( 25, 1350,  45, 2500),
            new Gear( 45, 1950,  70, 3500),
            new Gear( 70, 2500, 112, 4000),
            new Gear(112, 3100, 180, 5000)
        };

        for (int i = 0; i < gears.Length; ++i)
        {
            if (gears[i].speedFits(kmh))
            {
                gearNum = i + 1;
                return gears[i].interpolate(kmh);
            }
        }
        gearNum = 1;
        return 800;
    }

    private void SetParticleSystems(float engineRPM)
    {
        float smokeRate = engineRPM / 50.0f;
        _smokeLEmission.rateOverDistance = new ParticleSystem.MinMaxCurve(smokeRate);
        _smokeREmission.rateOverDistance = new ParticleSystem.MinMaxCurve(smokeRate);

        // Set wheels dust
        float dustRate = 0;
        if (_currentSpeedKMH > 10.0f && _carIsOnDrySand)
        {
            dustRate = _currentSpeedKMH / 2;
        }

        foreach (var particleSystem in dustEmissions)
        {
            ParticleSystem.EmissionModule em = particleSystem.emission;
            em.rateOverTime = new ParticleSystem.MinMaxCurve(dustRate);
        }

    }

    private WheelHit GetGroundInfos(ref WheelCollider wheelCol,
                                     ref string groundTag,
                                     ref int groundTextureIndex)
    { 
        // Default values
        groundTag = "InTheAir";
        groundTextureIndex = -1;
        // Query ground by ray shoot on the front left wheel collider
        WheelHit wheelHit;
        wheelCol.GetGroundHit(out wheelHit);
        // If not in the air query collider
        if (wheelHit.collider)
        {
            groundTag = wheelHit.collider.tag;
            if (wheelHit.collider.CompareTag("Terrain"))
                groundTextureIndex = TerrainSurface.GetMainTexture(transform.position);
        }
        return wheelHit;
    }

    void SetSkidmarking(bool doSkidmarking)
    {
        foreach (var wheel in wheelBehaviours)
            wheel.DoSkidmarking(doSkidmarking);
    }
}

class Gear
{
    private float _minRPM;
    private float _minKMH;
    private float _maxRPM;
    private float _maxKMH;

    public Gear(float minKMH, float minRPM, float maxKMH, float maxRPM)
    {
        _minRPM = minRPM;
        _minKMH = minKMH;
        _maxRPM = maxRPM;
        _maxKMH = maxKMH;
    }
    
    public bool speedFits(float kmh)
    {
        return kmh >= _minKMH && kmh <= _maxKMH;
    }

    public float interpolate(float kmh)
    {
        return _minRPM + (kmh - _minKMH) / (_maxKMH - _minKMH) * (_maxRPM - _minRPM);
    }
}
