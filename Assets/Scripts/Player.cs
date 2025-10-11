using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _score;

    public float speed = 5f;

    public float _speedMultiplier = 2;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShootPrefab;

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
    private bool _isShieldActive = false;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

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
        } else {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShootActive)
        {
            Instantiate(_tripleShootPrefab, transform.position, Quaternion.identity);
        }
        else { 
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
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

    public void Damage()
    {
        if (_isShieldActive)
        {
            this._isShieldActive = false;
            this.shield.SetActive(false);
            return;
        }
        _lives--;

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

    public void ActiveTripleShoot() {
        _isTripleShootActive = true;
        StartCoroutine(TripleShootPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        this.shield.SetActive(true);
    }

    IEnumerator TripleShootPowerDownRoutine() {
        yield return new WaitForSeconds(5.0f);
        _isTripleShootActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        speed /= _speedMultiplier;
    }

    public void AddScore(int points)
    {
        this._score += points;
        _uiManager.PlayerScore(this._score);
    }
}
