using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        onFoot.Jump.performed += ctx => motor.Jump();
    }
    private void FixedUpdate()
    {
        // tell the playerMotor to move using the value from our movment action
        motor.ProcessMove(onFoot.Movment.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }
    private void OnEnable()
    {
        onFoot.Enable();
    }
}
