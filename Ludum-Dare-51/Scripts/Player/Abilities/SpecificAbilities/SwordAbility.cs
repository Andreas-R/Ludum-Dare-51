using Godot;

public class SwordAbility : AbstractAbility {
    private static Texture[] sprites = {
        ResourceLoader.Load("res://Textures/sword1.png") as Texture,
        ResourceLoader.Load("res://Textures/sword2.png") as Texture,
        ResourceLoader.Load("res://Textures/sword3.png") as Texture,
        ResourceLoader.Load("res://Textures/sword4.png") as Texture,
        ResourceLoader.Load("res://Textures/sword5.png") as Texture
    };
    private float swordLengthIncreasePerLevel = 5f;
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
                //sword.Scale *= GetScaleMultiplicator();
                GD.Print(level2);
                sword.setSprite(sprites[level2]);
                CapsuleShape2D swordColliderShape = (CapsuleShape2D) sword.swordCollider.Shape;
                swordColliderShape.Height += swordLengthIncreasePerLevel;
                sword.swordCollider.Position -= new Vector2(0, swordLengthIncreasePerLevel / 2);
                if(level2 == 2){
                    swordColliderShape.Radius += 1;
                }
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
