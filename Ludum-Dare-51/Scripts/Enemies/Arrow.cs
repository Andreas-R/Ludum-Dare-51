using Godot;

public class Arrow : RigidBody2D {
    [Export]
    public float speed;

    public Vector2 direction;

    public override void _Process(float delta) {
        this.Position += this.direction * this.speed * delta;
    }

    public void OnBodyEntered(Node body) {
        this.OnDestroy();
    }

    public void OnDestroy() {
        QueueFree();
    }
}
