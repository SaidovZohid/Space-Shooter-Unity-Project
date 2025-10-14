using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    [SerializeField]
    private int _movementType = 0;

    [SerializeField]
    private float _ramDistance = 3.0f;

    [SerializeField]
    private float _ramSpeed = 6.0f;

    [SerializeField]
    private bool _canAvoidLasers = false;

    [SerializeField]
    private float _avoidDistance = 2.0f;

    [SerializeField]
    private float _avoidSpeed = 5.0f;

    private float _zigzagDirection = 1f;

    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;

    private SpawnManager _spawnManager;

    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private bool _isDead = false;

    // Shield system
    [SerializeField]
    private GameObject _shieldVisual;
    private bool _hasShield = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            _player = playerObject.GetComponent<Player>();
        }

        if (_player == null )
        {
            Debug.LogError("Player is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null )
        {
            Debug.LogError("Anim is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null )
        {
            Debug.LogError("AudioSource on Enemy is NULL");
        }

        if (_movementType == 2)
        {
            if (transform.position.x > 0)
            {
                _zigzagDirection = -1f;
            }
            else
            {
                _zigzagDirection = 1f;
            }
        }
    }

    public void SetMovementType(int type)
    {
        _movementType = type;

        if (_movementType == 2)
        {
            if (transform.position.x > 0)
            {
                _zigzagDirection = -1f;
            }
            else
            {
                _zigzagDirection = 1f;
            }
        }
    }

    public void EnableShield()
    {
        _hasShield = true;
        if (_shieldVisual != null)
        {
            _shieldVisual.SetActive(true);
        }
    }

    public void EnableLaserAvoidance()
    {
        _canAvoidLasers = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
        {
            return;
        }

        CalculateMovement();

        if (Time.time > _canFire) {
            CheckAndFireAtTarget();
        }
    }

    void CheckAndFireAtTarget()
    {
        GameObject powerup = FindPowerupInFront();

        if (powerup != null)
        {
            _fireRate = 0.5f;
            _canFire = Time.time + _fireRate;
            FireLaser();
        }
        else
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            FireLaser();
        }
    }

    GameObject FindPowerupInFront()
    {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("PowerUp");

        foreach (GameObject powerup in powerups)
        {
            if (powerup.transform.position.y < transform.position.y)
            {
                float distance = Mathf.Abs(powerup.transform.position.x - transform.position.x);
                if (distance < 1.5f)
                {
                    return powerup;
                }
            }
        }
        return null;
    }

    void FireLaser()
    {
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++) {
            lasers[i].AssignEnemyLaser();
        }
    }

    GameObject FindIncomingLaser()
    {
        GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");

        foreach (GameObject laser in lasers)
        {
            Laser laserScript = laser.GetComponent<Laser>();
            if (laserScript != null)
            {
                if (laser.transform.position.y < transform.position.y)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, laser.transform.position);
                if (distance < _avoidDistance)
                {
                    float xDistance = Mathf.Abs(laser.transform.position.x - transform.position.x);
                    if (xDistance < 1.0f)
                    {
                        return laser;
                    }
                }
            }
        }
        return null;
    }

    void AvoidLaser(GameObject laser)
    {
        float laserX = laser.transform.position.x;
        float enemyX = transform.position.x;

        if (enemyX < laserX)
        {
            transform.Translate(Vector3.left * _avoidSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.right * _avoidSpeed * Time.deltaTime);
        }

        transform.Translate(Vector3.down * (_speed * 0.5f) * Time.deltaTime);
    }

    void CalculateMovement()
    {
        if (_canAvoidLasers)
        {
            GameObject laser = FindIncomingLaser();
            if (laser != null)
            {
                AvoidLaser(laser);
                return;
            }
        }

        if (_player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

            if (distanceToPlayer < _ramDistance)
            {
                Vector3 direction = (_player.transform.position - transform.position).normalized;
                transform.position += direction * _ramSpeed * Time.deltaTime;
                return;
            }
        }

        if (_movementType == 0)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if (_movementType == 1)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            transform.Translate(Vector3.right * _zigzagDirection * 2f * Time.deltaTime);

            if (transform.position.x > 9f)
            {
                _zigzagDirection = -1f;
            }
            else if (transform.position.x < -9f)
            {
                _zigzagDirection = 1f;
            }
        }
        else if (_movementType == 2)
        {
            Vector3 direction = new Vector3(_zigzagDirection, -1f, 0);
            transform.Translate(direction * _speed * Time.deltaTime);
        }

        if (transform.position.y < -6f)
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead)
        {
            return;
        }

        if (other.tag == "Player") {
            _isDead = true;

            if (_player != null)
            {
                _player.Damage();
            }

            if (_spawnManager != null)
            {
                _spawnManager.EnemyDestroyed();
            }

            // Disable shield visual before death
            if (_shieldVisual != null)
            {
                _shieldVisual.SetActive(false);
            }

            if (_anim != null)
            {
                _anim.SetTrigger("OnEnemyDeath");
            }
            _speed = 0;

            if (_audioSource != null && _audioSource.enabled)
            {
                _audioSource.Play();
            }
            Destroy(gameObject, 2.4f);
        }
        if (other.tag == "Laser") {
            Destroy(other.gameObject);

            if (_hasShield)
            {
                _hasShield = false;
                if (_shieldVisual != null)
                {
                    _shieldVisual.SetActive(false);
                }
                return;
            }

            _isDead = true;

            if (_player != null) {
                _player.AddScore(10);
            }

            if (_spawnManager != null)
            {
                _spawnManager.EnemyDestroyed();
            }

            if (_shieldVisual != null)
            {
                _shieldVisual.SetActive(false);
            }

            if (_anim != null)
            {
                _anim.SetTrigger("OnEnemyDeath");
            }
            _speed = 0;

            if (_audioSource != null && _audioSource.enabled)
            {
                _audioSource.Play();
            }

            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 2.4f);
        }
    }
}
