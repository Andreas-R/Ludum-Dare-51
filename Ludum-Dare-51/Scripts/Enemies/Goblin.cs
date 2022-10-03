using Godot;
using System;

public class Goblin : AbstractEnemy
{
    [Export]
    public float moveSpeed;
    [Export]
    public float dashSpeed;

    private bool isDashing;
    private Timer dashTimer;
    private CollisionShape2D collider;

    public override void _Ready()
    {
        base._Ready();
        dashTimer = GetNode<Timer>("DashTimer");
        collider = GetNode<CollisionShape2D>("Collider");
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        if(!isDashing){
            /* if(Metronome.instance.IsBeat(5, 0f)){
                Teleport(bodyState);
            } */
            if(Metronome.instance.IsBeat(new[]{1,3,7,9}, new[]{0f})){
                InitDash(bodyState);
            }
            else{
                Vector2 moveDir = (this.GlobalPosition - this.player.GlobalPosition);
                moveDir = moveDir.Normalized() * this.moveSpeed;
                Physics2DDirectSpaceState spaceState = GetWorld2d().DirectSpaceState;
                CapsuleShape2D colliderShape = (CapsuleShape2D)(collider.Shape);
                Vector2 rayCastTarget = collider.GlobalPosition + moveDir.Normalized() * (colliderShape.Radius + 5f);
                if(spaceState.IntersectRay(collider.GlobalPosition, rayCastTarget, new Godot.Collections.Array { this }, 2).Count>0){
                    bool yBlocked = false;
                    bool xBlocked = false;
                    if(spaceState.IntersectRay(collider.GlobalPosition, new Vector2(0, rayCastTarget.y), new Godot.Collections.Array { this }, 2).Count>0){
                        yBlocked = true;
                    }
                    if(spaceState.IntersectRay(collider.GlobalPosition, new Vector2(rayCastTarget.x, 0), new Godot.Collections.Array { this }, 2).Count>0){
                        xBlocked = true;
                    }
                    if(xBlocked && yBlocked){
                        moveDir = Vector2.Zero;
                    }
                    else if(xBlocked){
                        moveDir = new Vector2(0, moveDir.y).Normalized() * this.moveSpeed;
                    }
                    else if(yBlocked){
                        moveDir = new Vector2(moveDir.x, 0).Normalized() * this.moveSpeed;
                    }
                }
                bodyState.LinearVelocity = moveDir;
                HandleSpriteFlip(moveDir);
            }
        }
    }

    private void InitDash(Physics2DDirectBodyState bodyState){
        isDashing = true;
        //TODO: Dash animation
        dashTimer.Start();
        RandomNumberGenerator randomGenerator = new RandomNumberGenerator();
        randomGenerator.Randomize();
        //TODO: Maybe exclude some directions, like dashing straight into the player
        float angle = randomGenerator.RandfRange((float)-Math.PI,(float)Math.PI);
        Vector2 moveDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        bodyState.LinearVelocity = moveDir * this.dashSpeed;
        HandleSpriteFlip(moveDir);
    }

    /* private void Teleport(Physics2DDirectBodyState bodyState){
        bodyState.Transform.origin = new Vector2();
    } */

    public void StopDash(){
        isDashing = false;
    }

    public override void OnDeath(){
        GrantBonus();
        base.OnDeath();
    }

    public void GrantBonus(){
        RoomHandler.instance.GuaranteedChestSpawnNextRoom = true;
    }

    public override void OnEscape()
    {
        RoomHandler.instance.GuaranteedNoChestSpawnNextRoom = true;
        base.OnEscape();
    }
}
