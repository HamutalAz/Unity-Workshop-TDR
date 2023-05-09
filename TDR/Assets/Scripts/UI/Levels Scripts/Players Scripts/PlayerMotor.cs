using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool IsGrounded;
    public float speed = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;
    private static FirebaseFirestore dbReference;
    private DocumentReference userDoc;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        dbReference = FirebaseFirestore.DefaultInstance;
        userDoc = dbReference.Collection("Users").Document(DataBaseManager.userID);
        //userDoc = dbReference.Collection("Users").Document("JQD1GEkcogVOfGodZ1Y5"); // todo: delete & uncomment above line

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
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (IsGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        //update location in DB if movment detected
        if(moveDirection != Vector3.zero) {
            string loc = transform.position.ToString();
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "location", loc }
            };

            userDoc.UpdateAsync(updates).ContinueWithOnMainThread(task =>
            {
                //Debug.Log("player's location updated to: " + loc);
            });
        }

    }
    public void Jump()
    {
        if (IsGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}
