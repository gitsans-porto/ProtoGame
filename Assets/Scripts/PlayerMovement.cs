using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;

    // Variabel gravitasi dasar
    Vector3 velocity;
    public float gravity = -9.81f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal"); // Tombol A dan D
        float z = Input.GetAxis("Vertical");   // Tombol W dan S

        // Bergerak relatif terhadap arah hadap karakter
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Terapkan gravitasi agar selalu memijak lantai
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}