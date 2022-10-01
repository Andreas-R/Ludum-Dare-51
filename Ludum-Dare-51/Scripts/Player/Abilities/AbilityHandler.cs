using Godot;
using System.Collections.Generic;

public class AbilityHandler : Node {
    private static int ABILITY_MAX_LEVEL = 5;
    private static int MAX_EQUIPPED_ABILITIES = 3;

    private Dictionary<AbilityType, AbstractAbility> abilities = new Dictionary<AbilityType, AbstractAbility>();

    public override void _Process(float delta) {
        foreach (AbstractAbility ability in abilities.Values) {
            ability.OnProcess(delta);
        }
    }

    public bool AddAbility(AbilityType abilityType, int levelType) {
        if (abilities.ContainsKey(abilityType)) {
            if (abilities[abilityType].GetTotalLevel() >= ABILITY_MAX_LEVEL) {
                return false;
            }
            return IncreaseAbilityLevel(abilityType, levelType);
        } else if (abilities.Count < MAX_EQUIPPED_ABILITIES) {
            switch (abilityType) {
                case AbilityType.FIREBALL: {
                    abilities[abilityType] = new FireballAbility();
                    break;
                }
                default: {
                    return false;
                }
            }
            return IncreaseAbilityLevel(abilityType, levelType);
        } else {
            return false;
        }
    }

    private bool IncreaseAbilityLevel(AbilityType abilityType, int levelType) {
        switch (levelType) {
            case 1: {
                abilities[abilityType].level1 += 1;
                break;
            }
            case 2: {
                abilities[abilityType].level2 += 1;
                break;
            }
            case 3: {
                abilities[abilityType].level3 += 1;
                break;
            }
            default: {
                return false;
            }
        }
        return true;
    }

    public bool RemoveAbility(AbilityType abilityType) {
        if (abilities.ContainsKey(abilityType)) {
            abilities.Remove(abilityType);
            return true;
        } else {
            return false;
        }
    }
}
