using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum AccelerationState{
        // The players speed is constant
        NO_ACCEL,
        // The player is accelerating towards the maximum speed in the positive direction
        INCREASING_TO_POS_MAX,
        // The player is accelerating towards the maximum speed in the negative direction
        DECREASING_TO_NEG_MAX,
        // The player is decelerating from a speed in the negative direction to zero
        INCREASING_TO_ZERO,
        // The player is decelerating from a speed in the positive direction to zero
        DECREASING_TO_ZERO
    }


    /// <summary>
    /// The max speed of the player
    /// </summary> 
    public float MaxSpeed = 10;

    /// <summary>
    /// How long in seconds it takes to player to move from stopped to full speed (or vice versa)
    /// </summary>
    public float AccelTime = 1;

    // What time the player started accelerating along each axis 
    // Forward / back = y side to side = x
    private Vector2 _currAccelTime = Vector2.zero;

    // What state the players acceleration / deceleration is in each direction
    // Index 0 is forward / backward and index 1 is side to side
    private AccelerationState[] _accelStates = { AccelerationState.NO_ACCEL, AccelerationState.NO_ACCEL };

    // The current speed the player is moving at along each axis
    private Vector2 _currSpeed;

    /// <summary>
    /// How high the player can jump
    /// </summary>
    public float JumpHeight = 1;

    // How long the player takes to reach the maximum jump height (In seconds)
    // Calulated based on gravity
    private float _jumpTime;

    // Track how much the player has jumped 
    private float _currJumpHeight = 0;

    // Track if the player is currently jumping
    private bool _jumping = false;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate jump time based on gravity
        _jumpTime = Math.Abs(JumpHeight / Physics.gravity.y);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Add mouse look

        // Decide how to move based on current state and input

        // Forward / Back axis
        // If the player is accelerating forward
        bool playerGrounded = Utils.IsGrounded(gameObject);
        if(_accelStates[0] == AccelerationState.INCREASING_TO_POS_MAX){
            // If the w key is down and the player is on the ground continue moving forward
            if(Input.GetKey(KeyCode.W) && playerGrounded){
                float currTimeAccelerating  = Time.time - _currAccelTime.y;
                // If the player's acceleration has ended start moving at full speed
                if(currTimeAccelerating >= AccelTime){
                    _currSpeed.y = MaxSpeed;
                    _accelStates[0] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue accelerating 
                else {
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.y += MaxSpeed * thisFrameAccelPrcnt;
                }
            }
            // Otherwise start slowing down to zero
            else{
                _accelStates[0] = AccelerationState.DECREASING_TO_ZERO;
                _currAccelTime.y = Time.time;
            }
        }
        // If the player is slowing to zero
        else if(_accelStates[0] == AccelerationState.DECREASING_TO_ZERO){
            // If the w key is down and the player is on the ground switch to forward acceleration
            if(Input.GetKey(KeyCode.W) && playerGrounded){
                _accelStates[0] = AccelerationState.INCREASING_TO_POS_MAX;
            }
            // Otherwise continue slowing down
            else{
                // If the player's acceleration has ended stop moving
                if(_currSpeed.y <= 0){
                    _currSpeed.y = 0;
                    _accelStates[0] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue slowing down
                else{
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.y -= MaxSpeed * thisFrameAccelPrcnt;
                }

            }
        }
        // If the player isn't accelerating 
        else if(_accelStates[0] == AccelerationState.NO_ACCEL){
            // If the player isn't moving and on the ground
            if(_currSpeed.y == 0 && playerGrounded){
                // If the w key is pressed start accelerating forward
                if(Input.GetKey(KeyCode.W)){
                    _accelStates[0] = AccelerationState.INCREASING_TO_POS_MAX;
                    _currAccelTime.y = Time.time;
                } 
                // If the s key is pressen start accelerating backward
                else if (Input.GetKey(KeyCode.S)){
                    _accelStates[0] = AccelerationState.DECREASING_TO_NEG_MAX;
                    _currAccelTime.y = Time.time;

                }
            }
            // if the player is moving forwards
            else if(_currSpeed.y > 0){
                // If the player isn't holding w or is in the air start slowing
                if(!Input.GetKey(KeyCode.W) || !playerGrounded){
                    _accelStates[0] = AccelerationState.DECREASING_TO_ZERO;
                }
            }
            // if the player is moving backwards
            else if(_currSpeed.y < 0){
                // If the player isn't holding s or is in the air start slowing
                if(!Input.GetKey(KeyCode.S) || !playerGrounded){
                    _accelStates[0] = AccelerationState.INCREASING_TO_ZERO;
                }
            }
        }
        // If the player is accelerating backwards
        else if(_accelStates[0] == AccelerationState.DECREASING_TO_NEG_MAX){
            // If the s key is down and the player is on the ground continue moving backward
            if(Input.GetKey(KeyCode.S) && playerGrounded){
                float currTimeAccelerating  = Time.time - _currAccelTime.y;
                // If the player's acceleration has ended start moving at full speed
                if(currTimeAccelerating >= AccelTime){
                    _currSpeed.y = -1 * MaxSpeed;
                    _accelStates[0] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue accelerating 
                else {
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.y -= MaxSpeed * thisFrameAccelPrcnt;
                }
            }
            // Otherwise start slowing down to zero
            else{
                _accelStates[0] = AccelerationState.DECREASING_TO_ZERO;
                _currAccelTime.y = Time.time;
            }
        }
        // If the player is slowing to zero
        else if(_accelStates[0] == AccelerationState.INCREASING_TO_ZERO){
            // If the s key is down and the player is on the ground switch to backward acceleration
            if(Input.GetKey(KeyCode.W) && playerGrounded){
                _accelStates[0] = AccelerationState.DECREASING_TO_NEG_MAX;
            }
            // Otherwise continue slowing down
            else{
                // If the player's acceleration has ended stop moving
                if(_currSpeed.y >= 0){
                    _currSpeed.y = 0;
                    _accelStates[0] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue slowing down
                else{
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.y += MaxSpeed * thisFrameAccelPrcnt;
                }

            }
        }

        // Side to side axis
        // If the player is accelerating to the right
        if(_accelStates[1] == AccelerationState.INCREASING_TO_POS_MAX){
            // If the d key is down and the player is on the ground continue moving right
            if(Input.GetKey(KeyCode.D) && playerGrounded){
                float currTimeAccelerating  = Time.time - _currAccelTime.x;
                // If the player's acceleration has ended start moving at full speed
                if(currTimeAccelerating >= AccelTime){
                    _currSpeed.x = MaxSpeed;
                    _accelStates[1] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue accelerating 
                else {
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.x += MaxSpeed * thisFrameAccelPrcnt;
                }
            }
            // Otherwise start slowing down to zero
            else{
                _accelStates[1] = AccelerationState.DECREASING_TO_ZERO;
                _currAccelTime.x = Time.time;
            }
        }
        // If the player is slowing to zero
        else if(_accelStates[1] == AccelerationState.DECREASING_TO_ZERO){
            // If the D key is down and the player is on the ground switch to rightward acceleration
            if(Input.GetKey(KeyCode.D) && playerGrounded){
                _accelStates[1] = AccelerationState.INCREASING_TO_POS_MAX;
            }
            // Otherwise continue slowing down
            else{
                // If the player's acceleration has ended stop moving
                if(_currSpeed.x <= 0){
                    _currSpeed.x = 0;
                    _accelStates[1] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue slowing down
                else{
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.x -= MaxSpeed * thisFrameAccelPrcnt;
                }

            }
        }
        // If the player isn't accelerating 
        else if(_accelStates[1] == AccelerationState.NO_ACCEL){
            // If the player isn't moving and is on the ground 
            if(_currSpeed.x == 0 && playerGrounded){
                // If the d key is pressed start accelerating rightward
                if(Input.GetKey(KeyCode.D)){
                    _accelStates[1] = AccelerationState.INCREASING_TO_POS_MAX;
                    _currAccelTime.x = Time.time;
                } 
                // If the s key is pressen start accelerating backward
                else if (Input.GetKey(KeyCode.A)){
                    _accelStates[1] = AccelerationState.DECREASING_TO_NEG_MAX;
                    _currAccelTime.x = Time.time;

                }
            }
            // if the player is moving forwards
            else if(_currSpeed.x > 0){
                // If the player isn't holding d or isn't grounded start slowing
                if(!Input.GetKey(KeyCode.D) || !playerGrounded){
                    _accelStates[1] = AccelerationState.DECREASING_TO_ZERO;
                }
            }
            // if the player is moving backwards
            else if(_currSpeed.x < 0){
                // If the player isn't holding a or isn't grounded start slowing
                if(!Input.GetKey(KeyCode.A) || !playerGrounded){
                    _accelStates[1] = AccelerationState.INCREASING_TO_ZERO;
                }
            }
        }
        // If the player is accelerating leftward
        else if(_accelStates[1] == AccelerationState.DECREASING_TO_NEG_MAX){
            // If the a key is down and the player is on the ground continue moving leftward
            if(Input.GetKey(KeyCode.A) && playerGrounded){
                float currTimeAccelerating  = Time.time - _currAccelTime.x;
                // If the player's acceleration has ended start moving at full speed
                if(currTimeAccelerating >= AccelTime){
                    _currSpeed.x = -1 * MaxSpeed;
                    _accelStates[1] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue accelerating 
                else {
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.x -= MaxSpeed * thisFrameAccelPrcnt;
                }
            }
            // Otherwise start slowing down to zero
            else{
                _accelStates[1] = AccelerationState.DECREASING_TO_ZERO;
                _currAccelTime.x = Time.time;
            }
        }
        // If the player is slowing to zero
        else if(_accelStates[1] == AccelerationState.INCREASING_TO_ZERO){
            // If the a key is down and the player is grounded switch to leftward acceleration
            if(Input.GetKey(KeyCode.S) && playerGrounded){
                _accelStates[1] = AccelerationState.DECREASING_TO_NEG_MAX;
            }
            // Otherwise continue slowing down
            else{
                // If the player's acceleration has ended stop moving
                if(_currSpeed.x >= 0){
                    _currSpeed.x = 0;
                    _accelStates[1] = AccelerationState.NO_ACCEL;
                }
                // Otherwise continue slowing down
                else{
                    float thisFrameAccelPrcnt = Time.deltaTime / AccelTime;
                    _currSpeed.x += MaxSpeed * thisFrameAccelPrcnt;
                }
            }
        }

     
        // Jumping logic
        float frameJump = 0;
        if(_jumping){
            // If at the top of the jump stop jumping
            if(_currJumpHeight >= JumpHeight){
                _jumping = false;
            }
            // Otherwise set increase for this frame
            else{
                frameJump = JumpHeight + (-1 * Physics.gravity.y);
                _currJumpHeight += (Time .deltaTime / _jumpTime) * JumpHeight;
            }
        }

        // If the jump key is pressed and the player is on the ground
        if(Input.GetKey(KeyCode.Space) && playerGrounded){
            _jumping = true;
            _currJumpHeight = 0;
        }


        // Move player by the current speed
        Vector3 moveVector = new Vector3(_currSpeed.x, frameJump, _currSpeed.y);
        Debug.Log(moveVector);
        moveVector = Matrix4x4.Rotate(transform.rotation) * moveVector;
        transform.position += Time.deltaTime * moveVector;

    }
}
