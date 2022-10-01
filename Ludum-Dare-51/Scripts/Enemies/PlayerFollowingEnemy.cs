using Godot;
using System;

public class PlayerFollowingEnemy : AbstractEnemy
{

    private bool _isMoving;
    private Timer _moveTimer;

    public override void OnReady()
    {
        base.OnReady();
        _moveTimer = GetNode<Timer>("MoveTimer"); 
    }

    protected override void Move(Physics2DDirectBodyState bodyState){
        if(!_isMoving){
            bodyState.LinearVelocity = Vector2.Zero;
            if(Metronome.instance.IsFrame(-1, 0f)){
                InitMove(bodyState);
            }
        }
    }

    private void InitMove(Physics2DDirectBodyState bodyState){
        _isMoving = true;
        StartMoveAnimation();
        _moveTimer.Start();
        Vector2 moveDir = (_playerNode.Position - Position).Normalized();
        moveDir = moveDir * moveSpeed;
        bodyState.LinearVelocity = moveDir;
        HandleSpriteFlip(moveDir);
    }

    private void StopMove(){
        _isMoving = false;
    }

    public override void CollisionEnter(Node body)
    {
        base.CollisionEnter(body);
        if(body.Equals(_playerNode)){
            GD.Print("Hit");
            //TODO: DO damage to player
        }
    }
}
