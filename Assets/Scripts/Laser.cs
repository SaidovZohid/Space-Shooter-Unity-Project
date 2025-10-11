using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8.0f;

    [SerializeField]
    private bool _isEnemyLaser = false;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true) {
            Player player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }
            if (transform.parent != null) {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
