using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool IsGrounded;
    public float speed = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;

    // Start is called before the first frame update
    async void Start()
    {
        controller = GetComponent<CharacterController>();
        Vector3 loc = await DataBaseManager.instance.levelManager.getInitialPlayerLoc();
        Debug.Log("location is: " + loc);
        controller.transform.position = loc;
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = controller.isGrounded;
    }
    // get the input for out ImputManager and applay them to our character controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        SetLocation(moveDirection);

        //update location in DB if movment detected
        if(moveDirection != Vector3.zero) {
            string loc = transform.position.ToString();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "location", loc }
            };

            DataBaseManager.instance.levelManager.updatePlayerLocation(updates);
        }

    }
    public void Jump()
    {
        if (IsGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    private void SetLocation(Vector3 moveDirection)
    {
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (IsGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
    }
}
