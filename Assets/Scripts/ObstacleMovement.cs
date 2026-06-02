using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float baseSpeed = 8f;        // Tốc độ cơ bản (bằng tốc độ Player ban đầu)
    public float currentSpeed;          // Tốc độ hiện tại (sẽ thay đổi theo game)

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        // Di chuyển về phía âm trục Z (lại gần Player)
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
    }

    // Gọi từ GameManager để cập nhật tốc độ khi game khó dần
    public void UpdateSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }

}