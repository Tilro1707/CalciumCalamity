using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioSource audioSource;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        if (audioSource != null)
        {
            audioSource.enabled = false;
        }
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}