using Godot;

public class moveSpeedAbility : AbstractAbility {

    private float moveBonusPerLevel = 0.2f;

    public moveSpeedAbility() {
        level1Max = 5;
        level2Max = 0;
        level3Max = 0;
    }

    public override void OnUpgrade(int upgradeType){
        Player.instance.moveSpeedMultiplier += moveBonusPerLevel;
    }

    public override void OnProcess(Player player, float delta){}

    public override int[] GetBeatFrequency(){
        return new int[0];
    }

    public override float[] GetSubBeatFrequency(){
        return new float[0];
    }
}
