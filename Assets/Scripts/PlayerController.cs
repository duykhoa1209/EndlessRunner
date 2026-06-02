using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float runSpeed = 8f;           // Tốc độ chạy tới
    public float jumpForce = 7f;          // Lực nhảy

    [Header("Lane Movement")]
    public float laneDistance = 2.5f;     // Khoảng cách giữa các làn
    public float laneChangeSpeed = 15f;   // Tốc độ chuyển làn
    public float maxLaneOffset = 5f;      // Giới hạn làn (laneDistance * 2)

    [Header("Ground Check")]
    public Transform groundCheck;         // Vị trí kiểm tra đất
    public float groundDistance = 0.4f;   // Bán kính kiểm tra
    public LayerMask groundMask;          // Lớp của mặt đất

    [Header("Game State")]
    [HideInInspector] public bool isGameOver = false;

    private Rigidbody rb;
    private bool isGrounded;
    private float targetX;                // Vị trí X mục tiêu
    private GameManager gameManager;      // Tham chiếu đến GameManager

    void Start()
    {
        // Lấy component Rigidbody
        rb = GetComponent<Rigidbody>();

        // Khóa xoay để nhân vật không bị nghiêng ngã
        rb.freezeRotation = true;

        // Khởi tạo vị trí X ban đầu
        targetX = transform.position.x;

        // Tìm GameManager trong scene
        gameManager = FindObjectOfType<GameManager>();

        // Tự động tạo GroundCheck nếu chưa có
        if (groundCheck == null)
        {
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.SetParent(transform);
            checkObj.transform.localPosition = new Vector3(0, -0.8f, 0);
            groundCheck = checkObj.transform;
        }
    }

    void Update()
    {
        // Giới hạn dựa trên vị trí tường
        float leftWallX = -5.5f;   // Vị trí tường trái (cộng thêm nửa chiều rộng)
        float rightWallX = 5.5f;   // Vị trí tường phải
        targetX = Mathf.Clamp(targetX, leftWallX, rightWallX);
        // Nếu game over thì không xử lý input
        if (isGameOver) return;

        // Kiểm tra chạm đất
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // XỬ LÝ CHUYỂN LÀN (A/D hoặc mũi tên trái/phải)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetX -= laneDistance;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetX += laneDistance;
        }

        // Giới hạn targetX trong khoảng hợp lý
        targetX = Mathf.Clamp(targetX, -maxLaneOffset, maxLaneOffset);

        // XỬ LÝ NHẢY
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Reset vận tốc Y trước khi nhảy
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Phát âm thanh nhảy
            if (gameManager != null)
            {
                gameManager.PlayJumpSound();
            }
        }
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        // DI CHUYỂN NGANG (có kiểm tra va chạm tường)
        Vector3 targetPos = new Vector3(targetX, rb.position.y, rb.position.z);
        Vector3 newPos = Vector3.Lerp(rb.position, targetPos, laneChangeSpeed * Time.fixedDeltaTime);

        // Kiểm tra va chạm tường trước khi di chuyển
        RaycastHit hit;
        if (Physics.Linecast(rb.position, newPos, out hit))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // Không di chuyển nếu va chạm tường
                return;
            }
        }

        rb.MovePosition(newPos);

        // DI CHUYỂN DỌC
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, runSpeed);
    }

    // HÀM DỪNG NHÂN VẬT (gọi từ GameManager)
    public void StopMoving()
    {
        isGameOver = true;
        runSpeed = 0;
        jumpForce = 0;
        rb.linearVelocity = Vector3.zero;
    }

    // HÀM LẤY TỐC ĐỘ HIỆN TẠI (cho các script khác)
    public float GetCurrentSpeed()
    {
        return runSpeed;
    }

    // Vẽ Gizmo để debug (kiểm tra vùng ground check)
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}