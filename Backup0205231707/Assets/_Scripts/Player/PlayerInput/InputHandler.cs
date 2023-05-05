using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }
    public bool GlideInput { get; private set; }
    private float jumpInputStartTime;
    private float dashInputStartTime;
    private bool AttackInput;
    private bool AttackHeld;
  

    [SerializeField]
    private float inputHoldTime = 0.5f;

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }
  
        if (context.canceled)
        {
            JumpInputStop = true;
        }
    }
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackInput = true;
        }
        if (context.canceled)
        {
            AttackInput = false;
            AttackHeld = false;
        }
    }
    public void OnDashInput(InputAction.CallbackContext context)    
    {
        if (context.started)
        {
            DashInput = true;
            DashInputStop = false;
            dashInputStartTime = Time.time;
        }
        if (context.canceled)
        {
            DashInput = false;
        }
    }
    public void OnGlideInput(InputAction.CallbackContext context)
    {
        if (context.started)    
        {
            GlideInput = true;
        }
        if (context.canceled)
        {
            GlideInput = false;
        }
    }
    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }
    private void CheckDashInputHoldTime()
    {
        if (Time.time >= dashInputStartTime + inputHoldTime)
        {
            DashInput = false;
        }
    }
}
