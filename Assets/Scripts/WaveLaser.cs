using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLaser : MonoBehaviour
{
    public float speed = 8.0f;

    [SerializeField]
    private Vector3 _direction = Vector3.up;

    void Update()
    {
        // Move in the assigned direction
        transform.Translate(_direction * speed * Time.deltaTime, Space.World);

        // Destroy if out of bounds
        if (transform.position.y > 8.0f ||
            transform.position.x > 11.0f ||
            transform.position.x < -11.0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;

        // Rotate the laser sprite to match the direction
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
