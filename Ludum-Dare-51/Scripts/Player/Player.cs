using Godot;

public class Player : RigidBody2D {
    [Export]
    public float runSpeed = 100f;

    private PlayerState state;
    private AnimatedSprite playerSprite;
    private float movementDeadzone = 0.2f;
    public Vector2 lastNonZeroMoveDir = Vector2.Left;
    
    public override void _Ready() {
        this.playerSprite = GetNode<AnimatedSprite>("PlayerSprite");

        this.state = PlayerState.RUNNING;
    }

    public override void _Process(float delta) {
        
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (this.state == PlayerState.DEAD) return;
        
        Vector2 movementInput = this.GetMoveInputDirection();

        switch(this.state) {
            case PlayerState.RUNNING: {
                this.HandleMovement(bodyState, movementInput);
                this.HandleSpriteFlip(movementInput);
                break;
            }
            case PlayerState.ATTACKING: {
                break;
            }
        }
    }

    private void ChangeState(PlayerState newState) {
        this.OnStateLeave(this.state);
        this.state = newState;
        this.OnStateEnter(this.state);
    }

    private void OnStateEnter(PlayerState enteredState) {
        switch(this.state) {
            case PlayerState.RUNNING: {
                // TODO - start animations
                break;
            }
            case PlayerState.ATTACKING: {
                // TODO - start animations
                break;
            }
        }
    }

    private void OnStateLeave(PlayerState leftState) {
        switch(this.state) {
            case PlayerState.RUNNING: {
                // TODO - state cleanup
                break;
            }
            case PlayerState.ATTACKING: {
                // TODO - state cleanup
                break;
            }
        }
    }

    public Vector2 GetMoveInputDirection() {
        float left = Input.GetActionRawStrength("move_left");
        float right = Input.GetActionRawStrength("move_right");
        float up = Input.GetActionRawStrength("move_up");
        float down = Input.GetActionRawStrength("move_down");

        Vector2 moveDir = new Vector2(right - left, down - up);

        // dead zone for gamepad
        if (moveDir.LengthSquared() < movementDeadzone * movementDeadzone) {
            return Vector2.Zero;
        }

        // prevent diagonal speed gain
        if (moveDir.LengthSquared() > 1) {
            moveDir = moveDir.Normalized();
        }

        if (moveDir != Vector2.Zero) {
            this.lastNonZeroMoveDir = moveDir;
        }

        return moveDir;
    }

    private void HandleMovement(Physics2DDirectBodyState bodyState, Vector2 movementInput) {
        Vector2 newVelocity = movementInput * runSpeed;
        bodyState.LinearVelocity = newVelocity;
    }

    private void HandleSpriteFlip(Vector2 movementInput) {
        if (movementInput.x > movementDeadzone) {
            this.playerSprite.FlipH = true;
        }
        if (movementInput.x < -movementDeadzone) {
            this.playerSprite.FlipH = false;
        }
    }
}
