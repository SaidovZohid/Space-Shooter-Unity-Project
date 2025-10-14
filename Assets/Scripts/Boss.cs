using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private int _health = 20;

    [SerializeField]
    private float _moveSpeed = 2.0f;

    [SerializeField]
    private Vector3 _targetPosition = new Vector3(0, 3, 0);

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _fireRate = 2.0f;

    private float _canFire = -1;
    private bool _isInPosition = false;
    private bool _isDead = false;

    private Player _player;
    private SpawnManager _spawnManager;
    private Animator _anim;
    private AudioSource _audioSource;

    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            _player = playerObject.GetComponent<Player>();
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        if (!_isInPosition)
        {
            MoveToPosition();
        }
        else
        {
            if (Time.time > _canFire)
            {
                _canFire = Time.time + _fireRate;
                Attack();
            }
        }
    }

    void MoveToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            _isInPosition = true;
        }
    }

    void Attack()
    {
        int attackType = Random.Range(0, 3);

        if (attackType == 0)
        {
            FireStraightLaser();
        }
        else if (attackType == 1)
        {
            FireTripleLaser();
        }
        else
        {
            FireSpreadLaser();
        }
    }

    void FireStraightLaser()
    {
        GameObject laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
        Laser laserScript = laser.GetComponent<Laser>();
        if (laserScript != null)
        {
            laserScript.AssignEnemyLaser();
        }
    }

    void FireTripleLaser()
    {
        Vector3 leftPos = transform.position + new Vector3(-1, -1, 0);
        Vector3 centerPos = transform.position + new Vector3(0, -1, 0);
        Vector3 rightPos = transform.position + new Vector3(1, -1, 0);

        GameObject laser1 = Instantiate(_laserPrefab, leftPos, Quaternion.identity);
        GameObject laser2 = Instantiate(_laserPrefab, centerPos, Quaternion.identity);
        GameObject laser3 = Instantiate(_laserPrefab, rightPos, Quaternion.identity);

        laser1.GetComponent<Laser>().AssignEnemyLaser();
        laser2.GetComponent<Laser>().AssignEnemyLaser();
        laser3.GetComponent<Laser>().AssignEnemyLaser();
    }

    void FireSpreadLaser()
    {
        for (int i = 0; i < 5; i++)
        {
            float xOffset = -2 + (i * 1);
            Vector3 spawnPos = transform.position + new Vector3(xOffset, -1, 0);
            GameObject laser = Instantiate(_laserPrefab, spawnPos, Quaternion.identity);
            laser.GetComponent<Laser>().AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead)
        {
            return;
        }

        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }
        }

        if (other.tag == "Laser")
        {
            Laser laserScript = other.GetComponent<Laser>();
            if (laserScript != null && laserScript.IsEnemyLaser())
            {
                return;
            }

            Destroy(other.gameObject);
            _health--;

            if (_health <= 0)
            {
                _isDead = true;

                if (_player != null)
                {
                    _player.AddScore(100);
                }

                if (_spawnManager != null)
                {
                    _spawnManager.EnemyDestroyed();
                }

                if (_anim != null)
                {
                    _anim.SetTrigger("OnEnemyDeath");
                }

                if (_audioSource != null && _audioSource.enabled)
                {
                    _audioSource.Play();
                }

                Destroy(GetComponent<Collider2D>());
                Destroy(gameObject, 2.4f);
            }
        }
    }
}
