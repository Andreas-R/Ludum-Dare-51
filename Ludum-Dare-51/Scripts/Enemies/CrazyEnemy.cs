using Godot;

public class CrazyEnemy : AbstractEnemy {
    [Export]
    public float moveSpeed = 50;

    [Export]
    public float maxSpeed = 2000;

    [Export]
    public float circleDiameter = 50;

    [Export]
    public bool rotatesRight = true;

    [Export]
    public float velocityFilterConstant = 0.9f;

    [Export]
    public float minDistance = 10;

    [Export]
    public float phaseDelay = 0.75f;

    [Export]
    public float ellipseStretch = 3.0f;

    private bool isMoving;
    private Timer moveTimer;
    private Vector2 basePosition;

    private Vector2 paintPos;

    private float phase;

    public override void _Ready() {
        base._Ready();
        GD.Print("ready");
        this.moveTimer = GetNode<Timer>("MoveTimer"); 
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
        float circlePhase = ((beatTime * 2) % 1.0f) * 2 * Mathf.Pi;

        bool isEllipse = beatTime > 0.5f;
        //bool isEllipse = false;
        float targetDiffPositionX = circleDiameter / 2.0f - (circleDiameter / 2.0f) * Mathf.Cos(circlePhase);
        float targetDiffPositionY = (circleDiameter / 2.0f) * Mathf.Sin(circlePhase);



        if (this.rotatesRight) targetDiffPositionY *= -1.0f;
        if (isEllipse) targetDiffPositionX *= this.ellipseStretch;

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
