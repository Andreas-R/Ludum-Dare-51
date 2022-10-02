public class ArrowDamageDealer : DamageDealer {
    public override void HandleDamage(DamageReceiver receiver, float damage) {
        base.HandleDamage(receiver, damage);
        GetParent().QueueFree();
    }
}
