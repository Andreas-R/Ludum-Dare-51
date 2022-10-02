using Godot;

public class PlayerFollowingEnemy : AbstractEnemy {
    [Export]
    public float moveSpeed;

    private bool isMoving;
    private Timer moveTimer;

    public override void _Ready() {
        base._Ready();
        this.moveTimer = GetNode<Timer>("MoveTimer"); 
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (IsHit()) return;

        if (!this.isMoving) {
            bodyState.LinearVelocity = Vector2.Zero;

            if (Metronome.instance.IsBeat(-1, 0f)) {
                this.InitMove(bodyState);
            }
        }
    }

    private void InitMove(Physics2DDirectBodyState bodyState) {
        this.isMoving = true;
        StartMoveAnimation();
        this.moveTimer.Start();
        Vector2 moveDir = (player.GlobalPosition - this.GlobalPosition).Normalized();
        bodyState.LinearVelocity = moveDir * this.moveSpeed;
        HandleSpriteFlip(moveDir);
    }

    public void StopMove() {
        this.isMoving = false;
    }
}
