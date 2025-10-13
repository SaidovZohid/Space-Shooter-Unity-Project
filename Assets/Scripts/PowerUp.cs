using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    float _speed = 3.0f;

    [SerializeField]
    private int powerUpID;

    [SerializeField]
    private AudioClip _clip;

    [SerializeField]
    private float _magnetSpeed = 10.0f;

    private bool _isMagnetActive = false;
    private Transform _playerTransform;


    // Update is called once per frame
    void Update()
    {
        if (_isMagnetActive && _playerTransform != null)
        {
            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            transform.position += direction * _magnetSpeed * Time.deltaTime;
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    public void ActivateMagnet(Transform playerTransform)
    {
        _isMagnetActive = true;
        _playerTransform = playerTransform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            Player player = other.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null) {
                switch (powerUpID) {
                    case 0:
                        player.ActiveTripleShoot();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AddAmmo();
                        break;
                    case 4:
                        player.HealPlayer();
                        break;
                    case 5:
                        player.ActiveWaveShot();
                        break;
                    case 6:
                        Debug.Log("Slowing down player");
                        player.SlowDownPlayer();
                        break;
                    case 7:
                        player.ActiveHomingMissile();
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
