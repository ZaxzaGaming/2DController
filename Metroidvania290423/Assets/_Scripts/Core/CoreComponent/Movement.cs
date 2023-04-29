using System.Collections;
using UnityEngine;

namespace CoreSystem.CoreComponent
{
    public class Movement : CoreComponent
    {
//        public Rigidbody2D RB { get; private set; }
//        public PlayerData Data { get; private set; }
//        public int FacingDirection { get; private set; }
//        public bool CanSetVelocity { get; set; }
//        public Vector2 CurrentVelocity { get; private set; }
//        private Vector2 workspace;

//        protected override void Awake()
//        {
//            base.Awake();
//            RB = GetComponentInParent<Rigidbody2D>();
//            FacingDirection = 1;
//            CanSetVelocity = true;
//        }
//        public void LogicUpdate()
//        {
//            CurrentVelocity = RB.velocity;
//        }
//        public void SetVelocityZero()
//        {
//            RB.velocity = Vector2.zero;
//            SetFinalVelocity();
//        }
//        public void SetVelocityX(float lerpAmount)
//        {
//            //Calculate the direction we want to move in and our desired velocity
//            float targetSpeed = moveInput.x * Data.runMaxSpeed;

//            //We can reduce are control using Lerp() this smooths changes to are direction and speed
//            targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

//            #region Calculate AccelRate
//            float accelRate;
//            if (!IsGliding)
//            {
//                //Gets an acceleration value based on if we are accelerating (includes turning) 
//                //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
//                if (LastOnGroundTime > 0)
//                    accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
//                else
//                    accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
//                #endregion

//                #region Add Bonus Jump Apex Acceleration
//                //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
//                if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
//                {
//                    accelRate *= Data.jumpHangAccelerationMult;
//                    targetSpeed *= Data.jumpHangMaxSpeedMult;
//                }

//                #endregion

//                #region Conserve Momentum
//                //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
//                if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
//                {
//                    //Prevent any deceleration from happening, or in other words conserve are current momentum
//                    //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
//                    accelRate = 0;
//                }
//                #endregion

//                //Calculate difference between current velocity and desired velocity
//                float speedDif = +targetSpeed - RB.velocity.x;
//                //Calculate force along x-axis to apply to thr player

//                float movement = speedDif * accelRate;
//                //Convert this to a vector and apply to rigidbody
//                RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

//                /*
//                 * For those interested here is what AddForce() will do
//                 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
//                 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
//                */


//            }
//        }
//        public void SetVelocity(float velocity, Vector2 angle, int direction)
//        {
//            float accelRate;
//            float speedDif = +targetSpeed - CurrentVelocity.x;
//            float movement = speedDif * accelRate;
//            angle.Normalize();
//            workspace.Set(angle.x * velocity * direction, angle.y * velocity);
//            RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
//            SetFinalVelocity();
//        }
//        public void SetVelocity(float velocity, Vector2 direction)
//        {
//            workspace = direction * velocity;
//            SetFinalVelocity();
//        }

//        public void SetVelocityX(float velocity)
//        {
//            workspace.Set(velocity, CurrentVelocity.y);
//            SetFinalVelocity();
//        }

//        public void SetVelocityY(float velocity)
//        {
//            workspace.Set(CurrentVelocity.x, velocity);
//            SetFinalVelocity();
//        }

//        private void SetFinalVelocity()
//        {
//            if (CanSetVelocity)
//            {
//                RB.velocity = workspace;
//                CurrentVelocity = workspace;
//            }
//        }
//        public void CheckIfShouldFlip(float xInput)
//        {
//            if (xInput != 0 && xInput != FacingDirection)
//            {
//                Flip();
//            }
//        }
//        private void Flip()
//        {
//            FacingDirection *= -1;
//            RB.transform.Rotate(0.0f, 180.0f, 0.0f);
//        }
   }
}