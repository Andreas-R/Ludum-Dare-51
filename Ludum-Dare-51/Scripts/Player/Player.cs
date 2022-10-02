using Godot;

public class Player : RigidBody2D {
    [Export]
    public float runSpeed = 100f;

    private PlayerState state;
    private AnimatedSprite playerSprite;
    private LifePointManager lifePointManager;
    private Timer invulnerableTimer;
    private CollisionShape2D damageReceiverCollider;

    private float movementDeadzone = 0.2f;
    public Vector2 lastNonZeroMoveDir = Vector2.Left;

    private Color hitColor = new Color(1f, 0.5f, 0.5f);
    private Color defaultColor = new Color(1f, 1f, 1f);
    
    public override void _Ready() {
        this.playerSprite = GetNode<AnimatedSprite>("PlayerSprite");
        this.lifePointManager = GetNode<LifePointManager>("LifePointManager");
        this.invulnerableTimer = GetNode<Timer>("InvulnerableTimer");
        this.damageReceiverCollider = GetNode<CollisionShape2D>("DamageReceiver/Collider");

        this.state = PlayerState.RUNNING;
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsFrame(-1, 0)) {
            StartMoveAnimation();
        }
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
    
    private void StartMoveAnimation() {
        playerSprite.Frame = 0;
        playerSprite.Playing = true;
        playerSprite.Play();
    }

    private bool IsInvulnerable() {
        return !invulnerableTimer.IsStopped();
    }

    private void OnVulnerableEnd() {
        this.lifePointManager.isInvulnerable = false;
        this.damageReceiverCollider.Disabled = false;
        playerSprite.Modulate = defaultColor;
    }

    public void OnHit(Vector2 direction, float knockbackForce) {
        this.lifePointManager.isInvulnerable = true;
        this.damageReceiverCollider.Disabled = true;
        playerSprite.Modulate = hitColor;
        invulnerableTimer.Start();
    }

    public void OnDeath() {
        ChangeState(PlayerState.DEAD);
        playerSprite.FlipV = true;
    }
}
