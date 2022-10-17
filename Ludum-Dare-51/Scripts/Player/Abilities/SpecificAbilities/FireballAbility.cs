using Godot;

public class FireballAbility : AbstractAbility {
    private static PackedScene fireballPrefab = ResourceLoader.Load("res://Prefabs/Player/Abilities/Fireball.tscn") as PackedScene;

    private float spreadAngle = 12f;
    private float spreadAngleRad;

    public FireballAbility() {
        level1Max = 4;
        level2Max = 4;
        level3Max = 2;

        this.spreadAngleRad = Mathf.Deg2Rad(spreadAngle);
    }

    public override void OnProcess(Player player, float delta) {
        if (Metronome.instance.IsBeatWithAudioDelay(this.GetBeatFrequency(), this.GetSubBeatFrequency())) {
            SoundManager.instance.PlaySfx(SoundManager.Sfx.fireball);
        }
        if (Metronome.instance.IsBeat(this.GetBeatFrequency(), this.GetSubBeatFrequency())) {
            float numberOfFireballs = GetNumberOfFireBalls();
            
            for (int i = 0; i < numberOfFireballs; i += 1) {
                Fireball fireball = FireballAbility.fireballPrefab.Instance() as Fireball;
                float scale = GetScale();
                fireball.Scale = new Vector2(scale, scale);
                fireball.direction = (player.GetGlobalMousePosition() - player.GetCenter()).Normalized();
                fireball.Rotation = player.GetCenter().AngleToPoint(player.GetGlobalMousePosition());
                if (numberOfFireballs > 1) {
                    float rotationOffset = (-spreadAngleRad * (numberOfFireballs - 1)) * 0.5f + i * spreadAngleRad;
                    fireball.direction = fireball.direction.Rotated(rotationOffset);
                    fireball.Rotation += rotationOffset;
                }
                fireball.GlobalPosition = player.GetCenter() + fireball.direction * 20f;
                fireball.GetNode<DamageDealer>("DamageDealer").damage *= GetDamageMultiplicator();
                
                player.GetTree().Root.GetNode<Node>("Main").AddChild(fireball);
            }
        }
    }

    private float GetDamageMultiplicator() {
        return Mathf.Pow(1.5f, level1);
    }

    private float GetScale() {
        return 1 + this.level1 * 0.3f;
    }

    private int GetNumberOfFireBalls() {
        return 3 + this.level2;
    }

    public override int[] GetBeatFrequency() {
        switch (this.level3) {
            case 0: {
                return new int[] {3, 7};
            }
            case 1: {
                return new int[] {1, 3, 5, 7};
            }
            case 2: {
                return new int[] {-1};
            }
            default: {
                return new int[] {1, 5};
            }
        }
    }

    public override float[] GetSubBeatFrequency() {
        return new float[] {0f};
    }
}
