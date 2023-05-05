using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;
    [HideInInspector] public Controls controls;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool dashInput;
    [HideInInspector] public bool glideInput;
    [HideInInspector] public bool grappleInput;
    [HideInInspector] public bool attackInput;


    [SerializeField]
    private float inputHoldTime = 0.5f;
    private void Awake()
    {
        controls = new Controls();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); 
        controls.Dash.Dash.performed += ctx => dashInput = ctx.performed; 
        controls.Glide.Glide.performed += ctx => glideInput = ctx.performed; 
        controls.Grapple.Grapple.performed += ctx => grappleInput = ctx.performed; 
        controls.Attack1.Attack.performed += ctx => attackInput = ctx.performed; 
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}