// Player script
using UnityEngine;
using UnityEngine.AI;
public class PlayerController2 : MonoBehaviour
{
    public CharacterController controller;
    public bool isJumping;

    void Update()
    {
        // Your movement logic...
        isJumping = !controller.isGrounded; // true when in air
    }
}