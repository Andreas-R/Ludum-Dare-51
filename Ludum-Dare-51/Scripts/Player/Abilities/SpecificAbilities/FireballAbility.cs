using Godot;

public class FireballAbility : AbstractAbility {
    private Player player;
    private float baseDamage = 50f;

    public FireballAbility() {
        level1Max = 4;
        level2Max = 4;
        level3Max = 3;
    }

    public override void OnProcess(AbilityHandler abilityHandler, float delta) {
        if (Metronome.instance.IsFrame(GetFrequency(), new float[] {0.5f})) {
            // spawn fireball
        }
    }

    private float GetDamages() {
        return baseDamage * (level1 + 1);
    }

    private int GetNumberOfFireBalls() {
        return level2 + 1;
    }

    private int[] GetFrequency() {
        switch (level3) {
            case 0: {
                return new int[] {1};
            }
            case 1: {
                return new int[] {1, 6};
            }
            case 2: {
                return new int[] {1, 3, 5, 7, 9};
            }
            case 3: {
                return new int[] {-1};
            }
            default: {
                return new int[] {1};
            }
        }
    }
}
