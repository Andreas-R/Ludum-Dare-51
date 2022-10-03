using Godot;

public class AbstractEnemy : RigidBody2D {
    protected AnimatedSprite sprite;
    protected AnimatedSprite weaponSprite = null;
    protected Timer hitTimer;
    protected Player player;
    protected EnemyManager enemyManager;
    
    protected Color hitColor = new Color(1f, 0.5f, 0.5f);
    protected Color defaultColor = new Color(1f, 1f, 1f);
    [Export]
    public float difficultyFactor = 1f;

    public override void _Ready() {
        this.sprite = GetNode<AnimatedSprite>("Sprite");
        if (HasNode("Weapon")) {
            this.weaponSprite = GetNode<AnimatedSprite>("Weapon");
        }
        this.hitTimer = GetNode<Timer>("HitTimer");
        this.player = GetTree().Root.GetNode<Player>("Main/Player");
        this.enemyManager = GetTree().Root.GetNode<EnemyManager>("Main/EnemyManager");
    }

    public void StartMoveAnimation() {
        this.sprite.Frame = 0;
        this.sprite.Playing = true;
        this.sprite.Play();
    }

    public void StartAttackAnimation() {
        this.weaponSprite.Frame = 0;
        this.weaponSprite.Playing = true;
        this.weaponSprite.Play();
    }

    public void Aim(Vector2 targetPosition) {
        Vector2 direction = Position - targetPosition;
        float angle = Vector2.Left.AngleTo(direction);
        if (this.weaponSprite.FlipH == false) angle += Mathf.Pi;
        this.weaponSprite.Rotation = angle; 
    }

    protected void HandleSpriteFlip(Vector2 movementInput) {
        if (movementInput.x > 0) {
            this.sprite.FlipH = true;
            if (this.weaponSprite != null && this.weaponSprite.FlipH != true) {
                this.weaponSprite.FlipH = true;
                this.weaponSprite.Position = new Vector2( this.weaponSprite.Position.x * -1,  this.weaponSprite.Position.y);
            }
        }
        if (movementInput.x < 0) {
            this.sprite.FlipH = false;
            if (this.weaponSprite != null && this.weaponSprite.FlipH != false) {
                this.weaponSprite.FlipH = false;
                this.weaponSprite.Position = new Vector2( this.weaponSprite.Position.x * -1,  this.weaponSprite.Position.y);
            }
        }
    }


    public bool IsFlipped() {
        return this.sprite.FlipH;
    }

    public bool IsHit() {
        return !this.hitTimer.IsStopped();
    }

    public virtual void OnHit(Vector2 direction, float knockbackForce) {
        LinearVelocity = direction * knockbackForce;
        sprite.Modulate = hitColor;
        this.hitTimer.Start();
    }

    public void OnHitEnd() {
        LinearVelocity = Vector2.Zero;
        sprite.Modulate = defaultColor;
    }

    public virtual void OnDeath() {
        enemyManager.OnEnemyDeath(this);
        this.QueueFree();
    }

    public virtual void OnEscape(){
        QueueFree();
    }
}
