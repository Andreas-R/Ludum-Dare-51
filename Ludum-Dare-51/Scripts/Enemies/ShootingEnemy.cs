using Godot;
using System;

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

    public override void _Process(float delta) {
        if (
            Metronome.instance.IsFrame(-1, 0f) &&
            (this.player.GetCenter() - this.Position).LengthSquared() <= Mathf.Pow(this.range, 2)
        ) {
            Arrow arrow = ShootingEnemy.arrowPrefab.Instance() as Arrow;
            arrow.direction = (this.player.GetCenter() - this.Position).Normalized();
            arrow.Position = this.Position + arrow.direction * 40f;
            arrow.Rotation = this.Position.AngleToPoint(this.player.GetCenter());
            GetTree().Root.GetNode<Node>("Main").AddChild(arrow);
        }
    }

    protected override void Move(Physics2DDirectBodyState bodyState) {
        Vector2 moveDir = (this.player.Position - this.Position);

        if (moveDir.LengthSquared() > Mathf.Pow(this.range * (1f - this.rangeOverFollow), 2)) {
            moveDir = moveDir.Normalized() * this.moveSpeed;
            bodyState.LinearVelocity = moveDir;
        } else {
            bodyState.LinearVelocity = Vector2.Zero;
        }

        HandleSpriteFlip(moveDir);
    }
}
