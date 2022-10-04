using Godot;

public class ShootingEnemy : AbstractEnemy {
    private static PackedScene arrowPrefab = ResourceLoader.Load("res://Prefabs/Enemies/Arrow.tscn") as PackedScene;

    [Export]
    public float moveSpeed;
    [Export]
    public float range;
    [Export]
    // How far will the enemy move over the range (to stay in range longer when player moves)
    // value in range 0-1
    public float rangeOverFollow;

    public override void _Ready() {
        base._Ready();
        if (isBoss) {
            this.range *= AbstractEnemy.bossSizeScale;
        }
    }

    public override void _Process(float delta) {
        if (IsHit()) return;

        if (Metronome.instance.IsBeat(GetBeatFrequency(), GetSubBeatFrequency())) {
            Vector2 playerPosition = this.player.GetCenter();
            Aim(playerPosition);
            StartAttackAnimation();
            Arrow arrow = ShootingEnemy.arrowPrefab.Instance() as Arrow;
            arrow.direction = (playerPosition - this.weaponSprite.GlobalPosition).Normalized();
            arrow.Rotation = this.weaponSprite.GlobalPosition.AngleToPoint(playerPosition);
            if (isBoss) {
                arrow.Scale = new Vector2(arrow.Scale.x * AbstractEnemy.bossSizeScale, arrow.Scale.y * AbstractEnemy.bossSizeScale);
                arrow.GlobalPosition = this.weaponSprite.GlobalPosition + arrow.direction * 30f * AbstractEnemy.bossSizeScale;
            } else {
                arrow.GlobalPosition = this.weaponSprite.GlobalPosition + arrow.direction * 30f;
            }
            GetTree().Root.GetNode<Node>("Main").AddChild(arrow);
        }

        if(Metronome.instance.IsBeatWithAudioDelay(GetBeatFrequency(), GetSubBeatFrequency())){
            SoundManager.instance.PlaySfx(SoundManager.Sfx.bowShot);
        }

        if (Metronome.instance.IsBeat(-1, 0f)) {
            StartMoveAnimation();
        }
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if (IsHit()) return;

        Vector2 moveDir = (this.player.GlobalPosition - this.GlobalPosition);

        if (moveDir.LengthSquared() > Mathf.Pow(this.range * (1f - this.rangeOverFollow), 2)) {
            moveDir = moveDir.Normalized() * this.moveSpeed;
            bodyState.LinearVelocity = moveDir;
        } else {
            bodyState.LinearVelocity = Vector2.Zero;
        }

        HandleSpriteFlip(moveDir);
    }

    public void OnShootFinished() {
        Aim(this.GlobalPosition + (IsFlipped() ? Vector2.Right : Vector2.Left));
    }


    private int[] GetBeatFrequency() {
        switch(spawnIndex % 2) {
            case 0: {
                return new int[] {0, 2, 4, 6};
            }
            case 1: {
                return new int[] {1, 3, 5, 7};
            }
        }
        return new int[] {};
    }

    private float[] GetSubBeatFrequency() {
        return new float[] {0f};
    }
}
