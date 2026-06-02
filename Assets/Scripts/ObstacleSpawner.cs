using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefab & Pool")]
    public GameObject obstaclePrefab;      // Prefab chướng ngại vật
    public int poolSize = 20;               // Số lượng object trong pool

    [Header("Spawn Settings")]
    public float spawnInterval = 0.8f;      // Thời gian giữa các lần spawn (ban đầu)
    public float minSpawnInterval = 0.4f;   // Spawn interval tối thiểu (khó nhất)
    public float spawnDistance = 18f;       // Khoảng cách spawn phía trước Player
    public float destroyZ = -10f;           // Xóa khi ra khỏi Z này

    [Header("Lane Settings")]
    public float[] lanes = { -2.5f, 0f, 2.5f };  // 3 làn đường: trái, giữa, phải

    [Header("Difficulty Settings")]
    public float speedIncreaseRate = 0.01f; // Tốc độ giảm spawn interval mỗi giây

    private Queue<GameObject> obstaclePool;  // Hàng đợi lưu object
    private float currentSpawnInterval;
    private float spawnTimer;
    private Transform playerTransform;       // Tham chiếu đến Player
    private bool isSpawning = true;
    void Start()
    {

        // Tìm Player trong scene bằng tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Không tìm thấy Player! Hãy gán tag 'Player' cho nhân vật.");
        }

        // Khởi tạo pool
        obstaclePool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab);
            obj.SetActive(false);
            obstaclePool.Enqueue(obj);
        }

        currentSpawnInterval = spawnInterval;
        spawnTimer = 0f;
    }

    void Update()
    {
        if (!isSpawning || playerTransform == null)
        {
            Debug.Log("Spawner đang dừng: isSpawning=" + isSpawning + ", playerTransform=" + (playerTransform != null));
            return;
        }


        if (!isSpawning || playerTransform == null) return; 
        // Chỉ spawn khi có Player và game chưa kết thúc
        if (playerTransform == null) return;

        // Timer spawn
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0;
        }

        // Giảm dần spawn interval theo thời gian (tăng độ khó)
        if (currentSpawnInterval > minSpawnInterval)
        {
            currentSpawnInterval -= speedIncreaseRate * Time.deltaTime;
            if (currentSpawnInterval < minSpawnInterval)
                currentSpawnInterval = minSpawnInterval;
        }

        // Kiểm tra và hủy obstacle đã ra khỏi màn hình
        CheckAndRecycleObstacles();
    }

    void SpawnObstacle()
    {
        if (playerTransform == null) return;
        Debug.Log("Đang spawn obstacle...");

        // Lấy object từ pool
        GameObject obstacle = GetPooledObject();
        if (obstacle != null)
        {
            // Chọn làn đường ngẫu nhiên
            float randomX = lanes[Random.Range(0, lanes.Length)];

            // Spawn ở phía trước Player
            Vector3 spawnPosition = new Vector3(randomX, obstaclePrefab.transform.position.y, playerTransform.position.z + spawnDistance);

            obstacle.transform.position = spawnPosition;

            // Reset vận tốc nếu có Rigidbody (tránh bị trôi)
            Rigidbody rb = obstacle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            obstacle.SetActive(true);
        }
    }

    GameObject GetPooledObject()
    {
        // Tìm object đang inactive trong pool
        foreach (GameObject obj in obstaclePool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Nếu pool full, tạo mới (phòng trường hợp nhiều obstacle hơn pool size)
        GameObject newObj = Instantiate(obstaclePrefab);
        newObj.SetActive(false);
        obstaclePool.Enqueue(newObj);
        return newObj;
    }

    void CheckAndRecycleObstacles()
    {

        foreach (GameObject obj in obstaclePool)
        {
            if (obj.activeInHierarchy && obj.transform.position.z < destroyZ)
            {
                obj.SetActive(false);
            }
        }
    }

    // Public method để reset spawn interval (khi game over hoặc restart)
    public void ResetSpawnInterval()
    {
        currentSpawnInterval = spawnInterval;
        spawnTimer = 0f;
    }

    // Public method để lấy spawn interval hiện tại (hiển thị UI độ khó)
    public float GetCurrentSpawnInterval()
    {
        return currentSpawnInterval;
    }
    public void StopSpawning()
    {
        isSpawning = false;
    }

}