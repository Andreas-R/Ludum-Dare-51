using Godot;

public class Fireball : RigidBody2D {
    [Export]
    public float speed;

    public Vector2 direction;

    public override void _Process(float delta) {
        this.GlobalPosition += this.direction * this.speed * delta;
    }

    public void OnBodyEntered(Node body) {
        this.OnDestroy();
    }

    public void OnDestroy() {
        QueueFree();
    }
}
