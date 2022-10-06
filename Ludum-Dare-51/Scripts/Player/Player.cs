using Godot;
using System.Collections.Generic;

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
    public Sword sword;
    private TouchPad touchPad;
    private Sprite aimer;
    private int runningFrame = 0;
    private List<AbstractEnemy> enemies = new List<AbstractEnemy>();
    public AbstractEnemy nearestEnemy;

    private Color hitColor = new Color(1f, 0.5f, 0.5f);
    private Color defaultColor = new Color(1f, 1f, 1f);
    
    public override void _Ready() {
        instance = this;
        this.center = GetNode<Node2D>("Center");
        this.sword = GetNode<Sword>("SwordPivot");
        this.playerSprite = GetNode<AnimatedSprite>("PlayerSprite");
        this.swordSprite = GetNode<Sprite>("SwordPivot/Sword/Sprite");
        this.lifePointManager = GetNode<LifePointManager>("LifePointManager");
        this.invulnerableTimer = GetNode<Timer>("InvulnerableTimer");
        this.damageReceiverCollider = GetNode<CollisionShape2D>("DamageReceiver/Collider");
        this.touchPad = GetTree().Root.GetNode<TouchPad>("Main/HUD Parent/HUD/TouchPad");
        this.aimer = GetNode<Sprite>("Aimer");

        this.state = PlayerState.RUNNING;
    }

    public override void _Process(float delta) {
        if (!this.aimer.Visible) this.aimer.Visible = true;
        this.aimer.Rotation = -sword.attackDir.AngleTo(Vector2.Left);

        nearestEnemy = GetNearestEnemy();

        if (this.state == PlayerState.DEAD) return;

        if (Metronome.instance.IsBeat(-1, 0)) {
            StartMoveAnimation();
        }
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (this.state == PlayerState.DEAD) {
            bodyState.LinearVelocity = Vector2.Zero;
            return;
        }
        
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
        return touchPad.GetInput(false);
    }

    private void HandleMovement(Physics2DDirectBodyState bodyState, Vector2 movementInput) {
        Vector2 newVelocity = movementInput * runSpeed * moveSpeedMultiplier;
        bodyState.LinearVelocity = newVelocity;
    }

    private void HandleSpriteFlip() {
        Vector2 attackDir = sword.attackDir;
        if (attackDir.x > 0) {
            if (this.playerSprite.FlipH != true) {
                this.sword.Position = new Vector2( this.sword.Position.x * -1,  this.sword.Position.y);
            }
            this.playerSprite.FlipH = true;
            this.swordSprite.FlipV = true;
        }
        if (attackDir.x < 0) {
            if (this.playerSprite.FlipH != false) {
                this.sword.Position = new Vector2( this.sword.Position.x * -1,  this.sword.Position.y);
            }
            this.playerSprite.FlipH = false;
            this.swordSprite.FlipV = false;
        }
    }
    
    private void StartMoveAnimation() {
        playerSprite.Frame = 0;
        playerSprite.Playing = true;
        playerSprite.Play("running");
    }

    private void HitAnimation() {
        playerSprite.Frame = 0;
        playerSprite.Playing = true;
        playerSprite.Play("hit");
    }

    private void DieAnimation() {
        playerSprite.Frame = 0;
        playerSprite.Playing = true;
        playerSprite.Play("die");
    }

    private void DeadAnimation() {
        playerSprite.Frame = 0;
        playerSprite.Playing = true;
        playerSprite.Play("dead");
    }

    public Vector2 GetCenter() {
        return this.center.GlobalPosition;
    }

    private AbstractEnemy GetNearestEnemy() {
        enemies.Clear();

        Node2D main = GetTree().Root.GetNode<Node2D>("Main");
        foreach (Node node in main.GetChildren()) {
            AbstractEnemy enemy = node as AbstractEnemy;
            if (enemy != null) {
                enemies.Add(enemy);
            }
        }

        Vector2 pos = GetCenter();
        AbstractEnemy nearestEnemy = null;
        float minSquareDist = Mathf.Inf;

        foreach (AbstractEnemy enemy in enemies) {
            float squareDist = (pos - enemy.GlobalPosition).LengthSquared();
            if (squareDist < minSquareDist) {
                minSquareDist = squareDist;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
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
        if (state == PlayerState.DEAD) return;
        this.lifePointManager.isInvulnerable = true;
        this.damageReceiverCollider.SetDeferred("disabled", true);
        playerSprite.Modulate = hitColor;
        invulnerableTimer.Start();
        runningFrame = playerSprite.Frame;
        HitAnimation();
    }

    public void OnDeath() {
        if (state == PlayerState.DEAD) return;
        DieAnimation();
        ChangeState(PlayerState.DEAD);

        HUD hud = GetTree().Root.GetNode<HUD>("Main/HUD Parent/HUD");
        hud.OnGameOver();
    }

    public void OnAnimationFinished() {
        if (state == PlayerState.DEAD) {
            DeadAnimation();
        } else if (state == PlayerState.RUNNING) {
            playerSprite.Play("running");
            playerSprite.Frame = runningFrame;
        }
    }

    public bool IsDead() {
        return state == PlayerState.DEAD;
    }
}
