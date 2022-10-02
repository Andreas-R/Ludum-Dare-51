public abstract class AbstractAbility {
    public int level1 = 0;
    public int level2 = 0;
    public int level3 = 0;
    
    public int level1Max = 0;
    public int level2Max = 0;
    public int level3Max = 0;

    public abstract void OnProcess(AbilityHandler abilityHandler, float delta);

    public int GetTotalLevel() {
        return level1 + level2 + level3;
    }
}
