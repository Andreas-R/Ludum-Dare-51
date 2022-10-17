using Godot;

public class IceNovaAbility : AbstractAbility {

    public IceNovaAbility() {
        level1Max = 4;
        level2Max = 4;
        level3Max = 2;
    }

    public override void OnProcess(Player player, float delta) {
        if (Metronome.instance.IsBeatWithAudioDelay(this.GetBeatFrequency(), this.GetSubBeatFrequency())) {
            SoundManager.instance.PlaySfx(SoundManager.Sfx.iceNova);
        }
        if (Metronome.instance.IsBeat(this.GetBeatFrequency(), this.GetSubBeatFrequency())) {
            IceNova iceNova = player.GetNode<IceNova>("IceNova");
            iceNova.damage = iceNova.baseDamage * this.GetDamageMultiplicator();
            float scale = this.GetScale();
            iceNova.Scale = new Vector2(scale, scale);
            iceNova.OnAttack();
        }
    }

    private float GetDamageMultiplicator() {
        return Mathf.Pow(1.3f, level1);
    }

    private float GetScale() {
        return 1.5f + this.level2 * 0.5f;
    }

    public override int[] GetBeatFrequency() {
        switch (this.level3) {
            case 0: {
                return new int[] {2, 6};
            }
            case 1: {
                return new int[] {0, 2, 4, 6};
            }
            case 2: {
                return new int[] {-1};
            }
            default: {
                return new int[] {2, 6};
            }
        }
    }

    public override float[] GetSubBeatFrequency() {
        return new float[] {0.5f};
    }
}
