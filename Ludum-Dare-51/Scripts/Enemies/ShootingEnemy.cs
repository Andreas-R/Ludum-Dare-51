using Godot;
using System;

public class ShootingEnemy : AbstractEnemy {
    private static PackedScene arrowPrefab = ResourceLoader.Load("res://Prefabs/Arrow.tscn") as PackedScene;

    [Export]
    public float moveSpeed;
    [Export]
    public float range;
    [Export]
    // How far will the enemy move over the range (to stay in range longer when player moves)
    // value in range 0-1
    public float rangeOverFollow;
    [Export]
    public String arrowPrefabPath;

    //private Timer _reloadTimer;

    public override void _Process(float delta) {
        if (
            Metronome.instance.IsFrame(-1, 0f) &&
            (this.player.Position - this.Position).LengthSquared() <= Mathf.Pow(this.range, 2)
        ) {
            //spawn arrow
            //TODO: Play animation
            Arrow arrow = ShootingEnemy.arrowPrefab.Instance() as Arrow;
            arrow.target = this.player.Position;
            arrow.playerNode = this.player;
            // arrow.damage = damage;
            arrow.Position = Position + (this.player.Position - Position).Normalized() * 40f;
            arrow.Rotation = Position.AngleToPoint(this.player.Position);
            GetTree().Root.GetNode<Node>("Main").AddChild(arrow);
            //_reloadTimer.Start();
        }
    }

    protected override void Move(Physics2DDirectBodyState bodyState) {
        Vector2 moveDir = (this.player.Position - Position);

        if (moveDir.LengthSquared() > Mathf.Pow(range * (1f - rangeOverFollow), 2)) {
            moveDir = moveDir.Normalized() * moveSpeed;
            bodyState.LinearVelocity = moveDir;
        } else {
            bodyState.LinearVelocity = Vector2.Zero;
        }

        HandleSpriteFlip(moveDir);
    }
}
