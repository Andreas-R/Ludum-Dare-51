public class FireballAbility : AbstractAbility {
    public float baseDamage = 50f;


    public override void OnProcess(float delta) {
    }

    private float GetDamages() {
        return baseDamage * (level1 + 1);
    }

    private int GetNumberOfFireBalls() {
        return level2 + 1;
    }
}
