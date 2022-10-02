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

    protected override void Move(Physics2DDirectBodyState bodyState){
        if (!this.isMoving){
            bodyState.LinearVelocity = Vector2.Zero;

            if (Metronome.instance.IsFrame(-1, 0f)) {
                this.InitMove(bodyState);
            }
        }
    }

    private void InitMove(Physics2DDirectBodyState bodyState) {
        this.isMoving = true;
        StartMoveAnimation();
        this.moveTimer.Start();
        Vector2 moveDir = (player.Position - this.Position).Normalized();
        moveDir = moveDir * this.moveSpeed;
        bodyState.LinearVelocity = moveDir;
        HandleSpriteFlip(moveDir);
    }

    public void StopMove() {
        this.isMoving = false;
    }
}
