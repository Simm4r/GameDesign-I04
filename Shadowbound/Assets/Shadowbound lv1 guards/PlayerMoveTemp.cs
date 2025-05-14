using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerMoveTemp : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ);
        transform.Translate(move * speed * Time.deltaTime, Space.Self);
    }
    // [SerializeField] float speed = 5f;
    // [SerializeField] float gravity = -9.81f;

    // CharacterController controller;
    // Vector3 velocity;

    // void Start()
    // {
    //     controller = GetComponent<CharacterController>();
    // }

    // void Update()
    // {
    //     float moveX = Input.GetAxis("Horizontal"); // A/D
    //     float moveZ = Input.GetAxis("Vertical");   // W/S

    //     Vector3 move = transform.right * moveX + transform.forward * moveZ;
    //     controller.Move(move * speed * Time.deltaTime);

    //     // Applica gravità (se non tocchi il suolo, continui a cadere)
    //     if (!controller.isGrounded)
    //     {
    //         velocity.y += gravity * Time.deltaTime;
    //         controller.Move(velocity * Time.deltaTime);
    //     }
    //     else
    //     {
    //         velocity.y = -2f; // piccolo valore negativo per mantenere grounded
    //     }
    // }
}
