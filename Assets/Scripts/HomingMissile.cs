using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;

    [SerializeField]
    private float _rotateSpeed = 200.0f;

    private Transform _target;

    void Start()
    {
        FindClosestEnemy();
    }

    void Update()
    {
        if (_target == null)
        {
            FindClosestEnemy();
        }

        if (_target != null)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }

        if (transform.position.y > 8.0f || transform.position.y < -8.0f ||
            transform.position.x > 11.0f || transform.position.x < -11.0f)
        {
            Destroy(gameObject);
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            _target = closestEnemy.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Destroy(other.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
