using Godot;
using System;

public class ShootingEnemy : AbstractEnemy
{
    [Export]
    public float range;
    [Export]
    // How far will the enemy move over the range (to stay in range longer when player moves)
    // value in range 0-1
    public float rangeOverFollow;
    private static PackedScene _arrowPrefab;
    [Export]
    public String arrowPrefabPath;
    //private Timer _reloadTimer;

    public override void OnReady()
    {
        _arrowPrefab = ResourceLoader.Load<PackedScene>(arrowPrefabPath);
        //_reloadTimer = GetNode<Timer>("ReloadTimer");
    }

    protected override void Move(Physics2DDirectBodyState bodyState){
        Vector2 moveDir = (_playerNode.Position - Position);
        if(moveDir.Length() > range * (1-rangeOverFollow)){
            moveDir = moveDir.Normalized() * moveSpeed;
            bodyState.LinearVelocity = moveDir;
        }
        else{
            bodyState.LinearVelocity = Vector2.Zero;
        }
        HandleSpriteFlip(moveDir);
    }

    public override void _Process(float delta){
        if(Metronome.instance.IsFrame(-1, 0f) && (_playerNode.Position - Position).Length() <= range){
            //spawn arrow
            //TODO: Play animation
            Arrow arrow = _arrowPrefab.Instance() as Arrow;
            arrow.target = _playerNode.Position;
            arrow.Position = Position + (_playerNode.Position - Position).Normalized() * 40f;
            arrow.Rotation = Position.AngleToPoint(_playerNode.Position);
            GetTree().Root.GetNode<Node>("Main").AddChild(arrow);
            //_reloadTimer.Start();
        }
    }
}
