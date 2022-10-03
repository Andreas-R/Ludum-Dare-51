using Godot;

public class Player : RigidBody2D {

    public static Player instance;
    [Export]
    public float runSpeed = 100f;
    public float moveSpeedMultiplier = 1f;

    private PlayerState state;
    private Node2D center;
    private AnimatedSprite playerSprite;
    private Sprite swordSprite;
    private LifePointManager lifePointManager;
    private Timer invulnerableTimer;
    private CollisionShape2D damageReceiverCollider;
    private Node2D swordPivot;

    private float movementDeadzone = 0.2f;

    private Color hitColor = new Color(1f, 0.5f, 0.5f);
    private Color defaultColor = new Color(1f, 1f, 1f);
    
    public override void _Ready() {
        instance = this;
        this.center = GetNode<Node2D>("Center");
        this.swordPivot = GetNode<Node2D>("SwordPivot");
        this.playerSprite = GetNode<AnimatedSprite>("PlayerSprite");
        this.swordSprite = GetNode<Sprite>("SwordPivot/Sword/Sprite");
        this.lifePointManager = GetNode<LifePointManager>("LifePointManager");
        this.invulnerableTimer = GetNode<Timer>("InvulnerableTimer");
        this.damageReceiverCollider = GetNode<CollisionShape2D>("DamageReceiver/Collider");

        this.state = PlayerState.RUNNING;
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(-1, 0)) {
            StartMoveAnimation();
        }
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (this.state == PlayerState.DEAD) return;
        
        Vector2 movementInput = this.GetMoveInputDirection();

        switch(this.state) {
            case PlayerState.RUNNING: {
                this.HandleMovement(bodyState, movementInput);
                this.HandleSpriteFlip();
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

        return moveDir;
    }

    private void HandleMovement(Physics2DDirectBodyState bodyState, Vector2 movementInput) {
        Vector2 newVelocity = movementInput * runSpeed * moveSpeedMultiplier;
        bodyState.LinearVelocity = newVelocity;
    }

    private void HandleSpriteFlip() {
        Vector2 attackDir = (GetGlobalMousePosition() - GetCenter()).Normalized();
        if (attackDir.x > movementDeadzone) {
            if (this.playerSprite.FlipH != true) {
                this.swordPivot.Position = new Vector2( this.swordPivot.Position.x * -1,  this.swordPivot.Position.y);
            }
            this.playerSprite.FlipH = true;
            this.swordSprite.FlipV = true;
        }
        if (attackDir.x < -movementDeadzone) {
            if (this.playerSprite.FlipH != false) {
                this.swordPivot.Position = new Vector2( this.swordPivot.Position.x * -1,  this.swordPivot.Position.y);
            }
            this.playerSprite.FlipH = false;
            this.swordSprite.FlipV = false;
        }
    }
    
    private void StartMoveAnimation() {
        playerSprite.Frame = 0;
        playerSprite.Playing = true;
        playerSprite.Play();
    }

    public Vector2 GetCenter() {
        return this.center.GlobalPosition;
    }

    private bool IsInvulnerable() {
        return !invulnerableTimer.IsStopped();
    }

    private void OnVulnerableEnd() {
        this.lifePointManager.isInvulnerable = false;
        this.damageReceiverCollider.SetDeferred("disabled", false);
        playerSprite.Modulate = defaultColor;
    }

    public void OnHit(Vector2 direction, float knockbackForce) {
        this.lifePointManager.isInvulnerable = true;
        this.damageReceiverCollider.SetDeferred("disabled", true);
        playerSprite.Modulate = hitColor;
        invulnerableTimer.Start();
    }

    public void OnDeath() {
        ChangeState(PlayerState.DEAD);
        playerSprite.FlipV = true;
    }
}
