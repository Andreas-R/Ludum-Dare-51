using Godot;

public class SwordAbility : AbstractAbility {
    private static PackedScene fireballPrefab = ResourceLoader.Load("res://Prefabs/Player/Abilities/Fireball.tscn") as PackedScene;

    private Sword sword;

    public SwordAbility(Sword sword) {
        level1Max = 4;
        level2Max = 4;
        level3Max = 2;

        this.sword = sword;
    }

    public override void OnProcess(Player player, float delta) {
    }

    public override void OnUpgrade(int upgradeType){
        switch(upgradeType){
            case 1: {
                sword.GetNode<DamageDealer>("Sword").damage *= GetDamageMultiplicator();
                break;
            }
            case 2: {
                sword.Scale *= GetScaleMultiplicator();
                break;
            }
            case 3: {
                sword.attackArchAngleRadians += Mathf.Pi/2;
                break;
            }
            default: {
                break;
            }
        }
    }

    private float GetDamageMultiplicator() {
        return Mathf.Pow(1.5f, level1);
    }

    private float GetScaleMultiplicator() {
        return 1 + 1.25f;
    }

    public override int[] GetBeatFrequency() {
        return new int[0];
    }

    public override float[] GetSubBeatFrequency() {
        return new float[0];
    }
}
