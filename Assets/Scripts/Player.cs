using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    private float xInput;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

    }
}
