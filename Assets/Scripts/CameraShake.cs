using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPosition;
    private bool _isShaking = false;

    void Start()
    {
        _originalPosition = transform.localPosition;
    }

    public void Shake(float duration = 0.15f, float magnitude = 0.1f)
    {
        if (!_isShaking)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        _isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
        _isShaking = false;
    }
}
