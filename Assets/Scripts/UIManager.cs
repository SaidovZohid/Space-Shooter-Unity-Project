using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _ammoText;

    [SerializeField]
    private Image _LivesImage;

    [SerializeField]
    private Sprite[] liveSprites;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private Text _waveText;

    private GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;
        _gameOverText.gameObject.SetActive(false);

        if (_waveText != null)
        {
            _waveText.gameObject.SetActive(false);
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null )
        {
            Debug.LogError("Game Manager is NULL");
        }
    }
    public void PlayerScore(int points)
    {
        _scoreText.text = "Score: " + points.ToString();
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (_ammoText != null)
            _ammoText.text = "Ammo: " + currentAmmo + " / " + maxAmmo;
    }
    public void UpdateLives(int current)
    {
        // Safety check to prevent array index errors
        if (current >= 0 && current < liveSprites.Length)
        {
            _LivesImage.sprite = liveSprites[current];
        }

        if (current < 1)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ShowWave(int waveNumber)
    {
        if (_waveText != null)
        {
            StartCoroutine(WaveTextRoutine(waveNumber));
        }
    }

    IEnumerator WaveTextRoutine(int waveNumber)
    {
        _waveText.text = "WAVE " + waveNumber;
        _waveText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        _waveText.gameObject.SetActive(false);
    }
}
