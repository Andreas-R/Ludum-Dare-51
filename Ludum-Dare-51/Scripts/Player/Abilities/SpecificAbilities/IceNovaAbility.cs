using Godot;

public class IceNovaAbility : AbstractAbility {
    public IceNovaAbility() {
        level1Max = 4;
        level2Max = 4;
        level3Max = 3;
    }

    public override void OnProcess(Player player, float delta) {
        if (Metronome.instance.IsBeat(this.GetBeatFrequency(), this.GetSubBeatFrequency())) {
            
        }
    }

    private float GetDamageMultiplicator() {
        return this.level1 + 1;
    }

    private float GetScale() {
        return 1 + this.level1 * 0.25f;
    }

    private int GetNumberOfFireBalls() {
        return this.level2 + 3;
    }

    public override int[] GetBeatFrequency() {
        switch (this.level3) {
            case 0: {
                return new int[] {2};
            }
            case 1: {
                return new int[] {2, 6};
            }
            case 2: {
                return new int[] {0, 2, 4, 6};
            }
            case 3: {
                return new int[] {-1};
            }
            default: {
                return new int[] {6};
            }
        }
    }

    public override float[] GetSubBeatFrequency() {
        return new float[] {0.5f};
    }
}
