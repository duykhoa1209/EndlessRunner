using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem có va chạm với Player không
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tìm GameManager trong scene
            GameManager gm = FindObjectOfType<GameManager>();

            // Nếu tìm thấy, gọi âm thanh và game over
            if (gm != null)
            {
                gm.PlayHitSound();      // Phát âm thanh va chạm
                gm.GameOver();          // Kết thúc game
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameManager trong scene!");
            }
        }
    }
}