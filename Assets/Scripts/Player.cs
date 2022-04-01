using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D player;
    public float hSpeed;
    public float vSpeed;

    float horizontalSpeed;
    float verticalSpeed;

    // Update is called once per frame
    void Update() {
        horizontalSpeed = Input.GetAxisRaw("Horizontal") * hSpeed;
        verticalSpeed = Input.GetAxisRaw("Vertical") * vSpeed;
    }

    void FixedUpdate()
    {
        Vector2 newPosition = player.position;
        player.MovePosition(new Vector2(player.position.x + horizontalSpeed, player.position.y + verticalSpeed));

    }
}
