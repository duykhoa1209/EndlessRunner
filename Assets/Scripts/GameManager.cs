using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Game Settings")]
    public bool isGameOver = false;
    public int winScore = 0;  // Không dùng cho endless runner, để 0

    [Header("Audio")]
    public AudioSource bgMusic;
    public AudioSource sfxSource;
    public AudioClip jumpSound;
    public AudioClip gameOverSound;
    public AudioClip hitSound;

    private float currentScore = 0;
    private int highScore = 0;
    private bool isPaused = false;
    private PlayerController playerController;
    private ObstacleSpawner obstacleSpawner;

    void Start()
    {
        // Load High Score đã lưu
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();

        // Tìm các component cần thiết
        playerController = FindObjectOfType<PlayerController>();
        obstacleSpawner = FindObjectOfType<ObstacleSpawner>();

        // Ẩn các panel khi bắt đầu
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Phát nhạc nền
        if (bgMusic != null)
            bgMusic.Play();
    }

    void Update()
    {
        // Xử lý pause khi nhấn ESC
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            TogglePause();
        }

        // Nếu chưa game over và không pause, tăng điểm theo thời gian
        if (!isGameOver && !isPaused)
        {
            currentScore += Time.deltaTime;  // Mỗi giây +1 điểm
            UpdateScoreUI();
        }
    }

    public void AddScore(int points)
    {
        if (isGameOver || isPaused) return;

        currentScore += points;
        UpdateScoreUI();

        // Cập nhật High Score
        int intScore = Mathf.FloorToInt(currentScore);
        if (intScore > highScore)
        {
            highScore = intScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Phát âm thanh game over
        PlayGameOverSound();

        // Dừng nhạc nền (tuỳ chọn)
        if (bgMusic != null)
            bgMusic.Stop();

        // Dừng nhân vật
        if (playerController != null)
            playerController.StopMoving();

        // Dừng spawner
        if (obstacleSpawner != null)
            obstacleSpawner.StopSpawning();

        // Cập nhật High Score lần cuối
        int intScore = Mathf.FloorToInt(currentScore);
        if (intScore > highScore)
        {
            highScore = intScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }

        // Hiển thị panel Game Over
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Reset thời gian về 1 (tránh lỗi Time.timeScale)
        Time.timeScale = 1f;

        // Reload lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void TogglePause()
    {
        if (isGameOver) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(currentScore);
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "Best: " + highScore;
    }

    // ========== AUDIO METHODS ==========

    public void PlayJumpSound()
    {
        if (sfxSource != null && jumpSound != null)
            sfxSource.PlayOneShot(jumpSound);
    }

    public void PlayHitSound()
    {
        if (sfxSource != null && hitSound != null)
            sfxSource.PlayOneShot(hitSound);
    }

    public void PlayGameOverSound()
    {
        if (sfxSource != null && gameOverSound != null)
            sfxSource.PlayOneShot(gameOverSound);
    }
}