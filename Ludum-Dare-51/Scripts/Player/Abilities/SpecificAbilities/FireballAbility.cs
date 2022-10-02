using Godot;

public class FireballAbility : AbstractAbility {
    private static PackedScene fireballPrefab = ResourceLoader.Load("res://Prefabs/Player/Abilities/Fireball.tscn") as PackedScene;

    private float spreadAngle = 60f;
    private float spreadAngleRad;

    public FireballAbility() {
        level1Max = 4;
        level2Max = 4;
        level3Max = 3;

        level1 = level1Max;
        level2 = level2Max;
        level3 = level3Max;

        this.spreadAngleRad = Mathf.Deg2Rad(spreadAngle);
    }

    public override void OnProcess(Player player, float delta) {
        if (Metronome.instance.IsBeat(GetFrequency(), new float[] {0.5f})) {
            float numberOfFireballs = GetNumberOfFireBalls();
            
            for (int i = 0; i < numberOfFireballs; i += 1) {
                Fireball fireball = FireballAbility.fireballPrefab.Instance() as Fireball;
                fireball.direction = (player.GetGlobalMousePosition() - player.GetCenter()).Normalized();
                fireball.Rotation = player.GetCenter().AngleToPoint(player.GetGlobalMousePosition());
                if (numberOfFireballs > 1) {
                    float rotationOffset = (-spreadAngleRad * 0.5f) + i * (spreadAngleRad / (numberOfFireballs - 1));
                    fireball.direction = fireball.direction.Rotated(rotationOffset);
                    fireball.Rotation += rotationOffset;
                }
                fireball.GlobalPosition = player.GetCenter() + fireball.direction * 20f;
                GD.Print(fireball.Rotation);
                fireball.GetNode<DamageDealer>("DamageDealer").damage *= GetDamageMultiplicator();
                
                player.GetTree().Root.GetNode<Node>("Main").AddChild(fireball);
            }
        }
    }

    private float GetDamageMultiplicator() {
        return this.level1 + 1;
    }

    private int GetNumberOfFireBalls() {
        return this.level2 + 1;
    }

    private int[] GetFrequency() {
        switch (this.level3) {
            case 0: {
                return new int[] {1};
            }
            case 1: {
                return new int[] {1, 5};
            }
            case 2: {
                return new int[] {1, 3, 5, 7};
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
