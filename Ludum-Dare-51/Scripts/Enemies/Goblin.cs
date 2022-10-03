using Godot;
using System;

public class Goblin : AbstractEnemy
{
    [Export]
    public float moveSpeed;
    [Export]
    public float dashSpeed;
    [Export]
    public float maxPlayerDistance;

    private bool isDashing;
    private Timer dashTimer;
    private CollisionShape2D collider;
    private AnimatedSprite animatedSprite;
    static RandomNumberGenerator randomGenerator = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();
        dashTimer = GetNode<Timer>("DashTimer");
        collider = GetNode<CollisionShape2D>("Collider");
        animatedSprite = GetNode<AnimatedSprite>("Sprite");
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (!isDashing){
            if (Metronome.instance.IsBeat(-1, 0)) {
                InitDash(bodyState);
            }
            else {
                Vector2 playerDirection = (this.GlobalPosition - this.player.GlobalPosition);
                if (playerDirection.Length() < maxPlayerDistance) {
                    Vector2 moveDir = GetDirection();
                    bodyState.LinearVelocity = moveDir * this.moveSpeed;
                    HandleTurn(moveDir);
                }
                else {
                    bodyState.LinearVelocity = Vector2.Zero;
                }
            }
        }
    }

    private void InitDash(Physics2DDirectBodyState bodyState) {
        isDashing = true;
        animatedSprite.Frame = 0;
        animatedSprite.Play("dash");
        dashTimer.Start();
        Vector2 moveDir = GetDirection();
        bodyState.LinearVelocity = moveDir * this.dashSpeed;
        HandleTurn(moveDir);
    }

    private void HandleTurn(Vector2 moveDir) {
        if (moveDir.x > 0) {
            if (animatedSprite.FlipH != true) {
                animatedSprite.Position = new Vector2(animatedSprite.Position.x * -1,  animatedSprite.Position.y);
            }
        }
        if (moveDir.x < 0) {
            if (animatedSprite.FlipH != false) {
                animatedSprite.Position = new Vector2(animatedSprite.Position.x * -1,  animatedSprite.Position.y);
            }
        }
        HandleSpriteFlip(moveDir);
    }

    private Vector2 GetDirection() {
        Vector2 playerDirection = (this.GlobalPosition - this.player.GlobalPosition).Normalized();
        Vector2 awayFromPlayerDirection = playerDirection * 400f; // scaling in order to counter the vector to center
        Vector2 resultingDirection = (awayFromPlayerDirection - this.GlobalPosition).Normalized();
        // add some Randomness
        randomGenerator.Randomize();
        float angle = randomGenerator.RandfRange((float)-Math.PI / 4, (float) Math.PI / 4);
        resultingDirection.Rotated(angle);
        return resultingDirection;
    }

    public void StopDash() {
        isDashing = false;
    }

    public override void OnDeath() {
        GrantBonus();
        base.OnDeath();
    }

    public void GrantBonus() {
        RoomHandler.instance.TripleChestSpawnNextRoom = true;
    }
    
    public void OnAnimationFinished() {
        if (animatedSprite.Animation == "escape") {
            QueueFree();
        } else {
            animatedSprite.Play("idle");
        }
    }
}
