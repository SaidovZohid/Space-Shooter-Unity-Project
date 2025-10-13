using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _score;

    public float speed = 5f;
    private float _baseSpeed = 5f;

    public float _speedMultiplier = 2;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShootPrefab;

    [SerializeField]
    private GameObject _waveLaserPrefab;

    [SerializeField]
    private GameObject _homingMissilePrefab;

    [SerializeField]
    private GameObject shield;

    [SerializeField]
    private GameObject _rightEngine, _leftEngine;

    [SerializeField]
    private float _fireRate = 0.5f;

    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;

    private bool _isTripleShootActive = false;
    private bool _isWaveShotActive = false;
    private bool _isShieldActive = false;
    private bool _isHomingMissileActive = false;

    private UIManager _uiManager;
    private CameraShake _cameraShake;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    [SerializeField]
    private UnityEngine.UI.Slider _thrusterSlider;

    [SerializeField]
    private float _thrusterCharge = 1f;

    [SerializeField]
    private float _thrusterDepleteRate = 0.5f;

    [SerializeField]
    private float _thrusterRechargeRate = 0.25f;

    private bool _isThrusterOnCooldown = false;


    [SerializeField]
    private int _shieldStrength = 0;
    [SerializeField]
    private int _maxShieldStrength = 3;

    // Ammo
    [SerializeField]
    private int _maxAmmo = 15;
    private int _currentAmmo;

    [SerializeField]
    private AudioClip _noAmmoClip;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<CameraShake>();

        _currentAmmo = _maxAmmo;

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }
        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
        if (_cameraShake == null)
        {
            Debug.LogError("The CameraShake component is NULL.");
        }

        if (_uiManager != null)
        {
            _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        Thruster();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_currentAmmo > 0)
            {
                ShootLaser();
                _currentAmmo--;

                if (_uiManager != null)
                    _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
            }
            else
            {
                if (_noAmmoClip != null)
                {
                    _audioSource.PlayOneShot(_noAmmoClip);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            MagnetPowerUps();
        }
    }

    void Thruster()
    {
        if (!_isThrusterOnCooldown && Input.GetKey(KeyCode.LeftShift) && _thrusterCharge > 0f)
        {
            speed = _baseSpeed * _speedMultiplier;
            _thrusterCharge -= _thrusterDepleteRate * Time.deltaTime;
            if (_thrusterCharge <= 0f)
            {
                _thrusterCharge = 0f;
                StartCoroutine(ThrusterCooldownRoutine());
            }
        }
        else
        {
            speed = _baseSpeed;
            if (!_isThrusterOnCooldown && _thrusterCharge < 1f)
            {
                _thrusterCharge += _thrusterRechargeRate * Time.deltaTime;
                if (_thrusterCharge > 1f) _thrusterCharge = 1f;
            }
        }

        if (_thrusterSlider != null)
        {
            _thrusterSlider.value = _thrusterCharge;
        }
    }

    IEnumerator ThrusterCooldownRoutine()
    {
        _isThrusterOnCooldown = true;
        yield return new WaitForSeconds(2.0f);
        _isThrusterOnCooldown = false;
    }

    void ShootLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isHomingMissileActive)
        {
            Instantiate(_homingMissilePrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        else if (_isWaveShotActive)
        {
            FireWaveShot();
        }
        else if (_isTripleShootActive)
        {
            Instantiate(_tripleShootPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }

    void FireWaveShot()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, 1.05f, 0);

        GameObject centerLaser = Instantiate(_waveLaserPrefab, spawnPos, Quaternion.identity);
        centerLaser.GetComponent<WaveLaser>().SetDirection(Vector3.up);

        GameObject leftLaser1 = Instantiate(_waveLaserPrefab, spawnPos, Quaternion.identity);
        leftLaser1.GetComponent<WaveLaser>().SetDirection(new Vector3(-0.26f, 0.97f, 0));

        GameObject leftLaser2 = Instantiate(_waveLaserPrefab, spawnPos, Quaternion.identity);
        leftLaser2.GetComponent<WaveLaser>().SetDirection(new Vector3(-0.5f, 0.87f, 0));

        GameObject rightLaser1 = Instantiate(_waveLaserPrefab, spawnPos, Quaternion.identity);
        rightLaser1.GetComponent<WaveLaser>().SetDirection(new Vector3(0.26f, 0.97f, 0));

        GameObject rightLaser2 = Instantiate(_waveLaserPrefab, spawnPos, Quaternion.identity);
        rightLaser2.GetComponent<WaveLaser>().SetDirection(new Vector3(0.5f, 0.87f, 0));
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(movement * speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.5f, 9.5f), transform.position.y, 0);
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldStrength = _maxShieldStrength;
        this.shield.SetActive(true);
        UpdateShieldVisual();
    }

    private void UpdateShieldVisual()
    {
        if (shield == null) return;
        var renderer = shield.GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        switch (_shieldStrength)
        {
            case 2:
                renderer.color = Color.blue;
                break;
            case 1:
                renderer.color = Color.red;
                break;
            default:
                renderer.color = Color.white;
                break;
        }
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _shieldStrength--;
            UpdateShieldVisual();

            if (_shieldStrength <= 0)
            {
                _isShieldActive = false;
                this.shield.SetActive(false);
            }
            return;
        }
        _lives--;

        if (_cameraShake != null)
        {
            _cameraShake.Shake(0.15f, 0.1f);
        }

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void ActiveTripleShoot()
    {
        _isTripleShootActive = true;
        StartCoroutine(TripleShootPowerDownRoutine());
    }

    public void ActiveWaveShot()
    {
        _isWaveShotActive = true;
        StartCoroutine(WaveShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _baseSpeed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator TripleShootPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShootActive = false;
    }

    IEnumerator WaveShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isWaveShotActive = false;
    }

    public void ActiveHomingMissile()
    {
        _isHomingMissileActive = true;
        StartCoroutine(HomingMissilePowerDownRoutine());
    }

    IEnumerator HomingMissilePowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isHomingMissileActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _baseSpeed = 5f;
    }

    public void AddScore(int points)
    {
        this._score += points;
        _uiManager.PlayerScore(this._score);
    }

    public void AddAmmo()
    {
        _currentAmmo += 10;
        if (_currentAmmo > _maxAmmo)
            _currentAmmo = _maxAmmo;

        if (_uiManager != null)
            _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }

    public void HealPlayer()
    {
        if (_lives < 3)
        {
            _lives++;

            if (_lives == 3)
            {
                _leftEngine.SetActive(false);
                _rightEngine.SetActive(false);
            }
            else if (_lives == 2)
            {
                _leftEngine.SetActive(false);
            }

            if (_uiManager != null)
            {
                _uiManager.UpdateLives(_lives);
            }
        }
    }

    public void SlowDownPlayer()
    {
        _baseSpeed = _baseSpeed / 2f;
        StartCoroutine(SlowDownRoutine());
    }

    IEnumerator SlowDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _baseSpeed = 5f;
    }

    void MagnetPowerUps()
    {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerup in powerups)
        {
            PowerUp powerUpScript = powerup.GetComponent<PowerUp>();
            if (powerUpScript != null)
            {
                powerUpScript.ActivateMagnet(transform);
            }
        }
    }
}
