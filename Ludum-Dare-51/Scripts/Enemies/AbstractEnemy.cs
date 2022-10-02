using Godot;

public class AbstractEnemy : RigidBody2D {
    protected AnimatedSprite sprite;
    protected Timer hitTimer;
    protected Player player;
    
    private Color hitColor = new Color(1f, 0.5f, 0.5f);
    private Color defaultColor = new Color(1f, 1f, 1f);

    public override void _Ready() {
        this.sprite = GetNode<AnimatedSprite>("Sprite");
        this.hitTimer = GetNode<Timer>("HitTimer");
        this.player = GetTree().Root.GetNode<Player>("Main/Player");
    }

    public override void _IntegrateForces(Physics2DDirectBodyState bodyState) {
        this.Move(bodyState);
    }

    public void StartMoveAnimation() {
        this.sprite.Frame = 0;
        this.sprite.Playing = true;
        this.sprite.Play();
    }

    protected void HandleSpriteFlip(Vector2 movementInput) {
        if (movementInput.x > 0) {
            this.sprite.FlipH = true;
        }
        if (movementInput.x < 0) {
            this.sprite.FlipH = false;
        }
    }

    protected virtual void Move(Physics2DDirectBodyState bodyState) {}

    public virtual void OnHit(Vector2 direction, float knockbackForce) {
        sprite.Modulate = hitColor;
        this.hitTimer.Start();
    }

    public void OnHitEnd() {
        sprite.Modulate = defaultColor;
    }

    public virtual void OnDeath() {
        this.QueueFree();
    }
}
