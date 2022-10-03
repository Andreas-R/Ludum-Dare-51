using Godot;
using System;

public class EnemyManager : Node
{
    private int enemyCounter;
    public float totalEnemyDifficulty;

    [Signal]
    public delegate void OnEnemySpawnSignal();

    [Signal]
    public delegate void OnEnemyDeathSignal();

    public override void _Ready() {
        enemyCounter = 0;
        totalEnemyDifficulty = 0f;
    }

    public void OnEnemySpawn(AbstractEnemy enemy) {
        enemyCounter++;
        totalEnemyDifficulty += enemy.difficultyFactor * (enemy.isBoss ? AbstractEnemy.bossLifeScale : 1f);
        EmitSignal(nameof(OnEnemySpawnSignal), totalEnemyDifficulty);
    }

    public void OnEnemyDeath(AbstractEnemy enemy) {
        enemyCounter--;
        totalEnemyDifficulty -= enemy.difficultyFactor * (enemy.isBoss ? AbstractEnemy.bossLifeScale : 1f);
        EmitSignal(nameof(OnEnemyDeathSignal), totalEnemyDifficulty);
    }
}
