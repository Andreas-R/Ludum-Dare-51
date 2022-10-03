using Godot;

public class Penguin : AbstractEnemy {
    [Export]
    public float moveSpeed;

    private bool isMoving;
    private Timer moveTimer;
    private SoundManager soundManager;

    public override void _Ready() {
        base._Ready();
        this.moveTimer = GetNode<Timer>("MoveTimer");
        this.soundManager = GetTree().Root.GetNode<SoundManager>("Main/SoundManager");
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (IsHit()) return;

        if (!this.isMoving) {
            bodyState.LinearVelocity = Vector2.Zero;

            if (Metronome.instance.IsBeatWithAudioDelay(GetBeatFrequency(), GetSubBeatFrequency())) {
                this.soundManager.PlaySfx(SoundManager.Sfx.penguin);
            }

            if (Metronome.instance.IsBeat(GetBeatFrequency(), GetSubBeatFrequency())) {
                this.InitMove(bodyState);
            }
        }
    }

    private void InitMove(Physics2DDirectBodyState bodyState) {
        this.isMoving = true;
        StartMoveAnimation();
        this.moveTimer.Start();
        Vector2 moveDir = (player.GlobalPosition - this.GlobalPosition).Normalized();
        if (Mathf.Abs(moveDir.x) >= Mathf.Abs(moveDir.y)) {
            bodyState.LinearVelocity = new Vector2(moveDir.x, 0) * this.moveSpeed;
        } else {
            bodyState.LinearVelocity = new Vector2(0, moveDir.y) * this.moveSpeed;
        }
        HandleSpriteFlip(moveDir);
    }

    public void StopMove() {
        this.isMoving = false;
    }

    private int[] GetBeatFrequency() {
        if  (spawnIndex % 2 == 0) {
            return new int[] {0, 2, 4, 6};
        } else {
            return new int[] {1, 3, 5, 7};
        }
    }

    private float[] GetSubBeatFrequency() {
        return new float[] {0f};
    }
}
