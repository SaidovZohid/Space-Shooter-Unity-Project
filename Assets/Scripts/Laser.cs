using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8.0f;

    [SerializeField]
    private bool _isEnemyLaser = false;

    private bool _hasHit = false;

    void Update()
    {
        if (!_isEnemyLaser) {
            MoveUp();
        } else {
            MoveDown();
        }
    }

    void MoveDown() { 
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null) {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    void MoveUp() { 
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > 8.0f)
        {
            if (transform.parent != null) {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    public void AssignEnemyLaser() {
        _isEnemyLaser = true;
    }

    public bool IsEnemyLaser() {
        return _isEnemyLaser;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyLaser)
        {
            if (other.tag == "Player")
            {
                if (_hasHit)
                {
                    return;
                }
                _hasHit = true;

                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                }
                if (transform.parent != null)
                {
                    Laser[] siblingLasers = transform.parent.GetComponentsInChildren<Laser>();
                    foreach (Laser laser in siblingLasers)
                    {
                        laser._hasHit = true;
                    }
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
            else if (other.tag == "PowerUp")
            {
                if (_hasHit)
                {
                    return;
                }
                _hasHit = true;

                Destroy(other.gameObject);
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
