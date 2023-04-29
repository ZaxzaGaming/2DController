using CoreSystem;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using static ItemData;
using static UnityEngine.Rendering.DebugUI;

public class Player : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters. If you don't want to use it
    //just paste in all the parameters, though you will need to manuly change all references in this script
    public PlayerData Data;

    #region COMPONENTS
    public Core Core { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public Magnet Magnet { get; private set; }

    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }

    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }
    public bool IsGliding { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool InAir { get; set; }
    public bool CanClimb { get; private set; }

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }


    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;
    public bool _isFastFalling;
    //Wall Jump
    private float _wallJumpStartTime;
    public float _wallJumpTimeout = 1f;
    private float _wallJumpTimer;
    private int _lastWallJumpDir;

    [Header("Abilities")]
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;
    private bool canDoubleJump = false;
    private bool canTripleJump = false;

    [Header("Abilities")]
    public bool doubleJumpAbility;
    public bool tripleJumpAbility;
    public bool wallJumpAbility;
    public bool dashAbility;
    public bool fastFallAbility;
    public bool glideAbility;
    public bool hasMagnet = false;

    [Header("Collision Info")]
    public float groundCheckDistance = 4;
    public float ceilingCheckDistance = 4;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;
    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;
    private bool canGrabLedge = true;

    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)

    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    [Header("Camera Stuff")]
    [SerializeField] private GameObject cameraFollowGO;

    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    private void Awake()
    {
        Core = GetComponentInChildren<Core> ();
        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        InputHandler = GetComponent<InputHandler>();
        Magnet = GetComponentInChildren<Magnet>();
        
    }

    private void Start()
    {
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedDampeningChangeThreshold;
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }
    
    private void Update()
    {
        if(RB.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        if(RB.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
        //CheckForLedges();
        Debug.Log(ledgeDetected);
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        _wallJumpTimer -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput.x = InputHandler.RawMovementInput.x;

        _moveInput.y = InputHandler.RawMovementInput.y;

        if (InputHandler.JumpInput)
        {
            InputHandler.UseJumpInput();
            OnJumpInput();
        }

        if (InputHandler.JumpInputStop)
        {
            InputHandler.UseJumpInput();
            OnJumpUpInput();
        }

        if (InputHandler.DashInput)
        {
            InputHandler.UseDashInput();
            OnDashInput();
        }
        if (InputHandler.GlideInput)
        {
            OnGlideInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsDashing && !IsJumping)
        {
            //Ground Check
            //if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
            if (Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, _groundLayer) && !IsJumping)
            {
                LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
                _wallJumpTimer = _wallJumpTimeout;
            }
            //Ceiling Detect
            if (Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, _groundLayer) && !IsJumping)
            {
                LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
            }

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }

        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping && !IsGliding)
                _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            IsGrounded = true;
            IsGliding = false;
            _isJumpCut = false;

            if (!IsJumping)
                _isJumpFalling = false;

        }
        else
        {
            IsGrounded = false;
        }

        //Fast Falling
        if (_isFastFalling)
        {
            if (IsGrounded)
            {
                GroundSmash();
                _isFastFalling = false;
            }
        }
        if (!IsDashing)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0 && IsGrounded)
            {
                Debug.Log("Jump");
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                _isFastFalling = false;
                if (doubleJumpAbility)
                {
                    canDoubleJump = true;
                }
                Jump(Data.jumpForce);
            }
            else if (canDoubleJump && LastPressedJumpTime > 0 && !IsJumping && !IsGrounded)
            {
                canDoubleJump = false;
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                _isFastFalling = false;
                if (tripleJumpAbility)
                {
                    canTripleJump = true;
                }
                Jump(Data.doubleJumpForce);
            }
            else if (canTripleJump && LastPressedJumpTime > 0 && !IsJumping && !IsGrounded)
            {
                canTripleJump = false;
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                _isFastFalling = false;
                Jump(Data.tripleJumpForce);
            }
            //WALL JUMP
            else if (CanWallJump() && LastPressedJumpTime > 0 && wallJumpAbility)
            {
                IsWallJumping = true;
                IsJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                _isFastFalling = false;
                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
                if (doubleJumpAbility && _wallJumpTimer < 0)
                {
                    canDoubleJump = true;
                }
                WallJump(_lastWallJumpDir);
            }


        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && LastPressedDashTime > 0)
        {
            if (dashAbility)
            {
                //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
                Sleep(Data.dashSleepTime);

                //If not direction pressed, dash forward
                if (_moveInput != Vector2.zero)
                    _lastDashDir = _moveInput;
                else
                    _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



                IsDashing = true;
                IsJumping = false;
                IsWallJumping = false;
                _isJumpCut = false;

                StartCoroutine(nameof(StartDash), _lastDashDir);
            }
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
            if (IsSliding && !IsGliding)
            {
                SetGravityScale(0);
            }
            else if (RB.velocity.y < 0 && _moveInput.y < 0 && !IsGliding)
            {
                if (fastFallAbility)
                {
                    _isFastFalling = true;
                    //Much higher gravity if holding down
                    SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                    //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                    RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
                }
            }
            else if (_isJumpCut && !IsGliding)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold && !IsGliding)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.velocity.y < 0 && !IsGliding)
            {
                //Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            SetGravityScale(0);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        CheckDirectionToFace();
        //Handle Run
        if (!IsDashing)
        {
            if (IsWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (_isDashAttacking)
        {
            Run(Data.dashEndRunLerp);
        }

        //Handle Slide
        if (IsSliding)
        {

            Slide();
        }
    }

    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }
    public void OnGlideInput()
    {
        if (glideAbility)
        {
            IsGliding = true;
        }
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;

        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;
        if (!IsGliding)
        {
            //Gets an acceleration value based on if we are accelerating (includes turning) 
            //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
            if (LastOnGroundTime > 0)
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
            else
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
            #endregion

            #region Add Bonus Jump Apex Acceleration
            //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
            if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                accelRate *= Data.jumpHangAccelerationMult;
                targetSpeed *= Data.jumpHangMaxSpeedMult;
            }

            #endregion

            #region Conserve Momentum
            //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
            if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
            {
                //Prevent any deceleration from happening, or in other words conserve are current momentum
                //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
                accelRate = 0;
            }
            #endregion

            //Calculate difference between current velocity and desired velocity
            float speedDif = +targetSpeed - RB.velocity.x;
            //Calculate force along x-axis to apply to thr player

            float movement = speedDif * accelRate;
            //Convert this to a vector and apply to rigidbody
            RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

            /*
             * For those interested here is what AddForce() will do
             * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
             * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
            */
        }
    }

    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            cameraFollowObject.CallTurn();
        }
    }
    #endregion

    #region JUMP METHODS
    private void Jump(float jumpForce)
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    private void WallJump(int dir)
    {
        if (wallJumpAbility)
        {
            //Ensures we can't call Wall Jump multiple times from one press
            LastPressedJumpTime = 0;
            LastOnGroundTime = 0;
            LastOnWallRightTime = 0;
            LastOnWallLeftTime = 0;
            _wallJumpTimer = _wallJumpTimeout;

            #region Perform Wall Jump
            Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
            force.x *= dir; //apply force in opposite direction of wall

            if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
                force.x -= RB.velocity.x;

            if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
                force.y -= RB.velocity.y;

            //Unlike in the run we want to use the Impulse mode.
            //The default mode will apply are force instantly ignoring masss
            RB.AddForce(force, ForceMode2D.Impulse);
            RB.AddForce(force, ForceMode2D.Impulse);
        }
        #endregion
    }
    // WALL CLIMB JUMP NOT WORKING
    //private void WallClimbJump(int dir)
    //{
    //    if (wallJumpAbility)
    //    {
    //        //Ensures we can't call Wall Jump multiple times from one press
    //        LastPressedJumpTime = 0;
    //        LastOnGroundTime = 0;
    //        LastOnWallRightTime = 0;
    //        LastOnWallLeftTime = 0;

    //        #region Perform Wall Jump
    //        Vector2 force = new Vector2(Data.wallClimbJumpForce.x, Data.wallClimbJumpForce.y);
    //        force.x *= dir; //apply force in opposite direction of wall

    //        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
    //            force.x -= RB.velocity.x;

    //        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
    //            force.y -= RB.velocity.y;

    //        //Unlike in the run we want to use the Impulse mode.
    //        //The default mode will apply are force instantly ignoring masss
    //        RB.AddForce(force, ForceMode2D.Impulse);
    //    }
    //    #endregion
    //}

    #endregion

    #region DASH METHODS
    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {

        //Overall this method of dashing aims to mimic Celeste, if you're looking for
        // a more physics-based approach try a method similar to that used in the jump

        LastOnGroundTime = 0;
        LastPressedDashTime = 0;
        float dirx = dir.x;
        float startTime = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        SetGravityScale(0);

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            float dashDirectionX = dirx * Data.dashSpeed;
            RB.velocity = new Vector2(dashDirectionX, RB.velocity.y);
            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }

        startTime = Time.time;

        _isDashAttacking = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        SetGravityScale(Data.gravityScale);
        RB.velocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }

        //Dash over
        IsDashing = false;

    }

    //Short period before the player is able to dash again
    private IEnumerator RefillDash(int amount)
    {
        //SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
        _dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - RB.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }
    #endregion


    #region CHECK METHODS

    private void CheckForLedges()
    {
        if (ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;
            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;
            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;
            CanClimb = true;
        }
        if (CanClimb)
            transform.position = climbBegunPosition;
    }
    private void LedgeClimbOver()
    {
        CanClimb = false;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", 0.1f);
    }
    private void AllowLedgeGrab() => canGrabLedge = true;
    public void CheckDirectionToFace()
    {
        if(InputHandler.RawMovementInput.x < 0 && IsFacingRight)
        {
            Turn();
        }
        else if(InputHandler.RawMovementInput.x > 0 && !IsFacingRight)
        {
            Turn();
        }
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping && !IsSliding;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }


    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }

    public bool GroundSmash()
    {
        if (_isFastFalling)
            if (IsGrounded)
                return true;
            else return false;
        else
            return false;
    }
    #endregion
    #region Add Pickups
    #region Add Abilities
    public void AddDoubleJump()
    {
        doubleJumpAbility = true;
    }
    public void AddDash()
    {
        dashAbility = true;
    }
    public void AddGlide()
    {
        glideAbility = true;
    }
    public void AddFastFall()
    {
        fastFallAbility = true;
    }
    public void AddMagnet()
    {
        hasMagnet = true;
    }
    #endregion
    #region Add Upgrades
    public void IncreaseMagnet(int value)
    {
        Magnet.SetSize(value);
    }
    public void AddTripleJump()
    {
        tripleJumpAbility = true;
    }
    #endregion
    #endregion
    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceilingCheckDistance));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}