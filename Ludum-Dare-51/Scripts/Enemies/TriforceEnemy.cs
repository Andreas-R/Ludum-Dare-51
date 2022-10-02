using Godot;

public class TriforceEnemy : AbstractEnemy {
    [Export]
    public static float triangleHeightFactor = 0.5f * Mathf.Sqrt(3);

    [Export]
    public float moveSpeed = 50;

    [Export]
    public float maxSpeed = 600;

    [Export]
    public float triangleSize = 100;

    [Export]
    public float phaseDelay = 0.0f;

    [Export]
    public float minDistance = 10.0f;

    [Export]
    public bool rotatesRight = false;

    [Export]
    public float velocityFilterConstant = 0.9f;


    private bool isMoving;
    private Timer moveTimer;
    private Vector2 basePosition;

    private Vector2 paintPos;

    public override void _Ready() {
        base._Ready();
        this.basePosition = this.GlobalPosition;
    }

    public override void _Draw() {
    //    this.DrawRect(new Rect2(this.basePosition.x - this.GlobalPosition.x - 3, this.basePosition.y - this.GlobalPosition.y - 3, 6, 6), new Color(1.0f, 0, 0));
    //    this.DrawRect(new Rect2(this.paintPos.x - this.GlobalPosition.x - 3, this.paintPos.y - this.GlobalPosition.y - 3, 6, 6), new Color(1.0f, 0, 1.0f));
    }

    public override void _PhysicsProcess(float delta) {
        this.Update();
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (IsHit()) return;
        float delta = bodyState.Step;
        Vector2 baseDiff = (player.GlobalPosition - this.basePosition);
        Vector2 moveDir = baseDiff.Normalized();
        float baseDiffLen = baseDiff.Length();
        if (baseDiffLen > minDistance) {
            this.basePosition += (moveDir * this.moveSpeed * delta);
        } else {
            this.basePosition -= (moveDir * this.moveSpeed * delta);
        }
        float beatTime = (Metronome.instance.currentBeat + phaseDelay) % 1.0f;

        float targetDiffPositionX;
        float targetDiffPositionY;
        if (beatTime < 0.375f) {
            targetDiffPositionX = 0.0f;
            targetDiffPositionY = 0.0f;
        } else if (beatTime < 0.75) {
            targetDiffPositionX = -triangleSize * triangleHeightFactor;
            targetDiffPositionY = triangleSize / 2.0f;
        } else {
            targetDiffPositionX = - triangleSize * triangleHeightFactor;
            targetDiffPositionY = -triangleSize / 2.0f;
        }


        if (this.rotatesRight) targetDiffPositionY *= -1.0f;

        Vector2 targetDiff = new Vector2(targetDiffPositionX, targetDiffPositionY);

        float angleToPlayer = Vector2.Right.AngleTo(moveDir);
        Vector2 rotatedTargetDiff = targetDiff.Rotated(angleToPlayer);
        Vector2 targetPos = this.basePosition + rotatedTargetDiff;

        this.paintPos = targetPos;

        Vector2 accel = (targetPos - this.GlobalPosition) / delta;

        Vector2 targetSpeed = accel.LimitLength(maxSpeed);
        Vector2 oldSpeed = bodyState.LinearVelocity;


        GD.Print(targetSpeed);
        Vector2 filteredSpeed = this.velocityFilterConstant * targetSpeed + (1.0f - this.velocityFilterConstant) * oldSpeed;

        GD.Print(filteredSpeed);

        bodyState.LinearVelocity = filteredSpeed;
    }
}
