using Godot;
using System.Collections.Generic;

public class ChainLightningAbility : AbstractAbility {
    private static PackedScene chainLightningPrefab = ResourceLoader.Load("res://Prefabs/Player/Abilities/ChainLightning.tscn") as PackedScene;

    private List<AbstractEnemy> enemies = new List<AbstractEnemy>();
    private Dictionary<Node2D, bool> selectedTargets = new Dictionary<Node2D, bool>();
    
    private float maxDistance = 300f;
    private float maxDistanceSquared;


    public ChainLightningAbility() {
        level1Max = 4;
        level2Max = 4;
        level3Max = 2;

        maxDistanceSquared = maxDistance * maxDistance;
    }

    public override void OnProcess(Player player, float delta) {
        if (Metronome.instance.IsBeat(this.GetBeatFrequency(), this.GetSubBeatFrequency())) {
            // get all enemies
            enemies.Clear();
            selectedTargets.Clear();

            Node2D main = player.GetTree().Root.GetNode<Node2D>("Main");
            foreach (Node node in main.GetChildren()) {
                AbstractEnemy enemy = node as AbstractEnemy;
                if (enemy != null) {
                    enemies.Add(enemy);
                }
            }

            int[] numberOfChains = GetNumberOfChains();

            // first chain is between player and first enemy
            Chain(player, player, 0, numberOfChains);
        }
        if (Metronome.instance.IsBeatWithAudioDelay(this.GetBeatFrequency(), this.GetSubBeatFrequency())){
            enemies.Clear();
            selectedTargets.Clear();
            Node2D main = player.GetTree().Root.GetNode<Node2D>("Main");
            foreach (Node node in main.GetChildren()) {
                AbstractEnemy enemy = node as AbstractEnemy;
                if (enemy != null) {
                    enemies.Add(enemy);
                }
            }
            if(GetNearestEnemy(player.GlobalPosition)!=null){
                SoundManager.instance.PlaySfx(SoundManager.Sfx.chainLightning);
            }
        }
    }

    private void Chain(Player player, Node2D currentTarget, int depth, int[] numberOfChains) {
        if (depth >= numberOfChains.Length) return;

        List<Node2D> chainedEnemies = new List<Node2D>();

        for (int i = 0; i < numberOfChains[depth]; i += 1) {
            AbstractEnemy nearestEnemy = GetNearestEnemy(currentTarget.GlobalPosition);

            if (nearestEnemy != null) {
                SpawnChainLightning(player, currentTarget, nearestEnemy, depth);

                chainedEnemies.Add(nearestEnemy);
                selectedTargets[nearestEnemy] = true;
            }
        }

        foreach (AbstractEnemy enemy in chainedEnemies) {
            this.Chain(player, enemy, depth + 1, numberOfChains);
        }
    }

    private AbstractEnemy GetNearestEnemy(Vector2 pos) {
        AbstractEnemy nearestEnemy = null;
        float minSquareDist = Mathf.Inf;

        foreach (AbstractEnemy enemy in enemies) {
            float squareDist = (pos - enemy.GlobalPosition).LengthSquared();
            if (!selectedTargets.ContainsKey(enemy) && squareDist < minSquareDist && squareDist <= maxDistanceSquared) {
                minSquareDist = squareDist;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void SpawnChainLightning(Player player, Node2D target1, Node2D target2, int depth) {
        ChainLightning chainLightning = ChainLightningAbility.chainLightningPrefab.Instance() as ChainLightning;
        chainLightning.target1 = target1;
        chainLightning.target2 = target2;
        chainLightning.depth = depth;
        chainLightning.damage = chainLightning.baseDamage * GetDamageMultiplicator();
        chainLightning.UpdatePositions();
        player.GetTree().Root.GetNode<Node>("Main").AddChild(chainLightning);
    }

    private float GetDamageMultiplicator() {
        return Mathf.Pow(1.5f, level1);
    }

    private int[] GetNumberOfChains() {
        switch (this.level2) {
            case 0: {
                return new int[] {1, 3};   // 4 enemies
            }
            case 1: {
                return new int[] {1, 2, 2};    // 7 enemies
            }
            case 2: {
                return new int[] {1, 2, 4};    // 11 enemies
            }
            case 3: {
                return new int[] {1, 2, 2, 2}; // 15 enemies
            }
            case 4: {
                return new int[] {1, 2, 3, 2}; // 21 enemies
            }
            default: {
                return new int[] {1, 3};
            }
        }
    }

    public override int[] GetBeatFrequency() {
        switch (this.level3) {
            case 0: {
                return new int[] {0, 4};
            }
            case 1: {
                return new int[] {0, 4};
            }
            case 2: {
                return new int[] {0, 2, 4, 6};
            }
            default: {
                return new int[] {0, 4};
            }
        }
    }

    public override float[] GetSubBeatFrequency() {
        switch (this.level3) {
            case 0: {
                return new float[] {0.25f};
            }
            case 1: {
                return new float[] {0.25f, 0.75f};
            }
            case 2: {
                return new float[] {0.25f, 0.75f};
            }
            default: {
                return new float[] {0.25f};
            }
        }
    }
}
