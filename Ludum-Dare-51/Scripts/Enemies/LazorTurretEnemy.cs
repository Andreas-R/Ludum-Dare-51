using Godot;

public class LazorTurretEnemy : AbstractEnemy {
    private static PackedScene beamPrefab = ResourceLoader.Load("res://Prefabs/Enemies/LazorBeam.tscn") as PackedScene;
    private static RandomNumberGenerator rng = new RandomNumberGenerator();

    protected AnimatedSprite beamSprite;

    [Export]
    public float rotationDuration = 0.5f;

    [Export]
    public float velocityDamping = 0.5f;

    private float targetAngle;
    private float startAngle;

    private float currentRotation;

    private bool newRotationSet = false;

    private LazorBeam lazorBeam;

    private CollisionShape2D collisionShape;

    private bool skippedFirstBeat = false;
    private SoundManager soundManager;

    public override void _Ready() {
        base._Ready();
        rng.Randomize();
        Vector2 direction = Position - player.Position;
        float angle = direction.AngleTo(Vector2.Left);
        this.targetAngle = angle;
        this.currentRotation = this.targetAngle;
        this.Rotation = this.targetAngle;
        this.startAngle = this.targetAngle;
        this.sprite.Playing = false;
        this.LinearDamp = this.velocityDamping;
        this.lazorBeam = LazorTurretEnemy.beamPrefab.Instance() as LazorBeam;
        this.lazorBeam.Scale = new Vector2(1600.0f, 5.0f * (isBoss ? AbstractEnemy.bossSizeScale : 1f));
        this.lazorBeam.Position = new Vector2(800.0f + 80.0f, 0.0f);
        this.lazorBeam.ZIndex = 2;
        this.collisionShape = lazorBeam.GetNode<CollisionShape2D>("DamageDealer/Collider");
        this.collisionShape.SetDeferred("disabled", true);
        this.AddChild(lazorBeam);
        this.lazorBeam.beamSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        this.lazorBeam.beamEndSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        this.soundManager = GetTree().Root.GetNode<SoundManager>("Main/SoundManager");
    }

    public override void _Process(float delta) {
        base._Process(delta);
        
        if (spawnIndex % 2 == 0) {
            if (Metronome.instance.IsBeatWithAudioDelay(new int[]{1,3,5,7}, new float[]{0})) {
                soundManager.PlaySfx(SoundManager.Sfx.laser);
            }
        } else {
            if (Metronome.instance.IsBeatWithAudioDelay(new int[]{2,4,6}, new float[]{0})) {
                soundManager.PlaySfx(SoundManager.Sfx.laser);
            }
        }
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        this.LinearVelocity = Vector2.Zero; // pull the handbreak
        float delta = bodyState.Step;
        float beatTime = (Metronome.instance.currentBeat + (spawnIndex % 2f)) % 2f;
      
        if (skippedFirstBeat) {
          
            if (beatTime <= rotationDuration) {
                this.collisionShape.SetDeferred("disabled", true);
                this.lazorBeam.beamSprite.Frame = 0;
                this.lazorBeam.beamEndSprite.Frame = 0;
                this.lazorBeam.beamSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.2f);
                this.lazorBeam.beamEndSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.2f);
                this.sprite.Frame = 0;
                float totalRotation = targetAngle - startAngle;
                if (totalRotation > Mathf.Pi) {
                    totalRotation -= 2 * Mathf.Pi;
                }
                if (totalRotation < - Mathf.Pi) {
                    totalRotation += 2 * Mathf.Pi;
                }
                this.currentRotation = this.startAngle + totalRotation * (beatTime / rotationDuration);
                newRotationSet = false;
            } else if (beatTime < 0.9f) {

            } else if (beatTime < 0.94f) {
                this.sprite.Frame = 1;
            } else if (beatTime < 0.97f) {
                this.sprite.Frame = 2;
            } else if (beatTime < 1.0f) {
                this.lazorBeam.beamSprite.Frame = 1;
                this.lazorBeam.beamEndSprite.Frame = 1;
                this.lazorBeam.beamSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 1f);
                this.lazorBeam.beamEndSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 1f);
                this.sprite.Frame = 3;
            } else if (beatTime < 1.0625f) {
                this.collisionShape.SetDeferred("disabled", false);
                this.sprite.Frame = 4;
                this.lazorBeam.beamSprite.Frame = 2;
                this.lazorBeam.beamEndSprite.Frame = 2;
            } else if (beatTime < 1.125) {
                this.lazorBeam.beamSprite.Frame = 3;
                this.lazorBeam.beamEndSprite.Frame = 3;
            } else if (beatTime < 1.75f) {
                this.sprite.Frame = 4;
                if (!this.newRotationSet) {
                this.startAngle = this.currentRotation;
                float error = rng.RandfRange(0, Mathf.Pi / 8) - Mathf.Pi / 16;
                Vector2 direction = Position - player.GetCenter();
                float angle = -(direction.AngleTo(Vector2.Right) - Mathf.Pi) + error;
                this.targetAngle = angle;
                this.newRotationSet = true;
                }
            } else if (beatTime < 1.85f) {
                this.collisionShape.SetDeferred("disabled", true);
                this.lazorBeam.beamSprite.Frame = 0;
                this.lazorBeam.beamEndSprite.Frame = 0;
                this.sprite.Frame = 5;
            } else {
                this.sprite.Frame = 0;
            }
            this.Rotation = this.currentRotation;
        }

        Vector2 turretDirection = Vector2.Right.Rotated(this.Rotation);
        Vector2 from = this.GlobalPosition + turretDirection * 40.0f;
        Vector2 to = from + turretDirection * 1600.0f;
        Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
        var result = spaceState.IntersectRay(
            from,
            to,
            null,
            2,
            true,
            true
        );
        if (result.Contains("position")) {
            Vector2 wallPosition = (Vector2)result["position"];
            float lazorLength = (wallPosition - from).Length();
            this.lazorBeam.Scale = new Vector2(lazorLength, 5.0f * (isBoss ? AbstractEnemy.bossSizeScale : 1f));
            this.lazorBeam.beamEndSprite.Scale = new Vector2((5f * (isBoss ? AbstractEnemy.bossSizeScale : 1f)) / lazorLength, 1f);
            this.lazorBeam.Position = new Vector2(lazorLength / 2.0f + 40.0f, 0.0f);

        }
    }

    public void InitialTimerFinished() {
        skippedFirstBeat = true;
    }
}
