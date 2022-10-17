using Godot;
using System;
using System.Collections.Generic;

public class RoomHandler : Node2D {

    private static PackedScene spawnPrefab = ResourceLoader.Load("res://Prefabs/Spawn.tscn") as PackedScene;
    public static RoomHandler instance;
    private static RandomNumberGenerator rng = new RandomNumberGenerator();

    [Export]
    public RoomData[] rooms;
    [Export]
    public RoomData treasureRoom;
    [Export]
    public Vector2 roomStart = new Vector2(-600, -400);
    [Export]
    public Vector2 roomEnd = new Vector2(600, 400);
    [Export]
    public float spawnWallMargin = 50f;
    [Export]
    public int chestSpawnFrequency = 4;
    [Export]
    public int lootGoblinSpawnFrequency = 4 * 4; // multiple of chestSpawnFrequency

    public bool TripleChestSpawnNextRoom{ get; set; }

    private Sprite roomSprite;
    private Chest chest1;
    private Chest chest2;
    private Chest chest3;
    private BgMusicHandler bgMusicHandler;

    public int roomCounter = 0;
    private int lastRoomIndex = -1;
    private int spriteIndex = 0;
    private int spriteCount = 0;
    public RoomData currentRoom;
    private EnemyManager enemyManager;
    private AbilityUpgradeHandler abilityUpgradeHandler;
    private static Vector2 scale = new Vector2(5f, 5f);
    private bool spawnBoss = false;

    private Dictionary<string, List<PackedScene>> roomEnemies = new Dictionary<string, List<PackedScene>>();
    private Dictionary<int, int> enemyIndexCounters = new Dictionary<int, int>();

    public override void _Ready() {
        instance = this;
        roomSprite = GetNode<Sprite>("RoomSprite");
        chest1 = GetTree().Root.GetNode<Chest>("Main/Chest1");
        chest2 = GetTree().Root.GetNode<Chest>("Main/Chest2");
        chest3 = GetTree().Root.GetNode<Chest>("Main/Chest3");
        bgMusicHandler = GetTree().Root.GetNode<BgMusicHandler>("Main/BgMusicHandler");
        enemyManager = GetTree().Root.GetNode<EnemyManager>("Main/EnemyManager");
        abilityUpgradeHandler = GetTree().Root.GetNode<AbilityUpgradeHandler>("Main/AbilityUpgradeHandler");

        foreach (RoomData room in rooms) {
            roomEnemies[room.id] = new List<PackedScene>();
            foreach (string prefabPath in room.spawnableEnemyPrefabPaths) {
                roomEnemies[room.id].Add(ResourceLoader.Load(prefabPath) as PackedScene);
            }
        }

        roomEnemies[treasureRoom.id] = new List<PackedScene>();
        foreach (string prefabPath in treasureRoom.spawnableEnemyPrefabPaths) {
            roomEnemies[treasureRoom.id].Add(ResourceLoader.Load(prefabPath) as PackedScene);
        }
        
        rng.Randomize();
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(-1, 0)) {
            //roomSprite.Scale = scale * 1.001f;
            int nextIndex = spriteIndex + 1;
            if (nextIndex < spriteCount) {
                roomSprite.Texture = currentRoom.roomImage[nextIndex];
                spriteIndex = nextIndex;
            } else {
                spriteIndex = 0;
                roomSprite.Texture = currentRoom.roomImage[0];
            }
        } else {
            // roomSprite.Scale -= (scale * 0.001f) / (delta * 1000);
            // if (roomSprite.Scale.x < scale.x) {
            //      roomSprite.Scale = scale;
            // }
        }
    }

    public void ChangeToRandomRoom() {
        List<RoomData> roomsToChoose = new List<RoomData>();
        float maxWeight = 0f;

        int i = 0;
        foreach (RoomData room in rooms) {
            if (roomCounter >= room.minSpawnRoomCount && i != lastRoomIndex) {
                roomsToChoose.Add(room);
                maxWeight += room.spawnWeight;
            }
            i += 1;
        }

        float randomWeight = rng.RandfRange(0f, maxWeight);
        float currentWeight = 0f;
        int randomRoomIndex = -1;
        int j = 0;
        foreach (RoomData room in roomsToChoose) {
            currentWeight += room.spawnWeight;
            if (randomWeight <= currentWeight) {
                randomRoomIndex = j;
                break;
            }
            j += 1;
        }

        this.ChangeToRoom(roomsToChoose[randomRoomIndex]);

        roomCounter += 1;

        if (currentRoom != treasureRoom) {
            lastRoomIndex = Array.FindIndex(rooms, room => room == roomsToChoose[randomRoomIndex]);
        }
    }

    private void ChangeToRoom(RoomData room) {
        if (currentRoom != null && currentRoom.id == "TreasureRoom"){
            Goblin survivingGoblin = GetNodeOrNull<Goblin>("../Goblin");
            if (survivingGoblin != null){
                survivingGoblin.OnEscape();
            }
        }
        
        if (chest1.Visible) chest1.Despawn();
        if (chest2.Visible) chest2.Despawn();
        if (chest3.Visible) chest3.Despawn();

        // loot goblin room
        if (roomCounter % lootGoblinSpawnFrequency == lootGoblinSpawnFrequency - 2) {
            currentRoom = treasureRoom;
            spriteCount = currentRoom.roomImage.Length;
            roomSprite.Texture = currentRoom.roomImage[0];
            this.SpawnEnemies(currentRoom);
        }
        // treasure room
        else if (roomCounter % chestSpawnFrequency == chestSpawnFrequency - 1) {
            List<AbilityUpgradeHandler.AbilityUpgrade> possibleUpgrades = this.abilityUpgradeHandler.GetPossibleUpgrades();
            Utils.Shuffle<AbilityUpgradeHandler.AbilityUpgrade>(possibleUpgrades);

            currentRoom = treasureRoom;
            spriteCount = currentRoom.roomImage.Length;
            roomSprite.Texture = currentRoom.roomImage[0];
        
            if (TripleChestSpawnNextRoom) {
                if (possibleUpgrades.Count > 0) {
                    chest1.Spawn(possibleUpgrades.GetRange(0, Math.Min(2, possibleUpgrades.Count)));
                } else {
                    chest1.Spawn(new List<AbilityUpgradeHandler.AbilityUpgrade>());
                }

                if (possibleUpgrades.Count > 2) {
                    chest2.Spawn(possibleUpgrades.GetRange(2, Math.Min(4, possibleUpgrades.Count) - 2));
                } else {
                    chest2.Spawn(new List<AbilityUpgradeHandler.AbilityUpgrade>());
                }

                if (possibleUpgrades.Count > 4) {
                    chest3.Spawn(possibleUpgrades.GetRange(4, Math.Min(6, possibleUpgrades.Count) - 4));
                } else {
                    chest3.Spawn(new List<AbilityUpgradeHandler.AbilityUpgrade>());
                }

                TripleChestSpawnNextRoom = false;
            } else {
                if (possibleUpgrades.Count > 0) {
                    chest1.Spawn(possibleUpgrades.GetRange(0, Math.Min(2, possibleUpgrades.Count)));
                } else {
                    chest1.Spawn(new List<AbilityUpgradeHandler.AbilityUpgrade>());
                }
            }
        }
        // other rooms
        else {
            currentRoom = room;
            spriteCount = currentRoom.roomImage.Length;
            roomSprite.Texture = currentRoom.roomImage[0];
            this.SpawnEnemies(currentRoom);
        }

        bgMusicHandler.ChangeMainBackgroundMusic(currentRoom.bgMusicSamples, enemyManager.totalEnemyDifficulty);
    }

    private void SpawnEnemies(RoomData room) {
        if (!spawnBoss) {
            if (roomCounter >= 10) {
                spawnBoss = (roomCounter - 1) % 10 == 0;
            }
            if (roomCounter >= 20) {
                spawnBoss = (roomCounter - 1) % 5 == 0;
            }
            if (roomCounter >= 30) {
                spawnBoss = (roomCounter - 1) % 3 == 0;
            }
            if (roomCounter >= 50) {
                spawnBoss = true;
            }
        }

        int numberOfBosses = 1;

        if (spawnBoss) {
            numberOfBosses = 1 + Mathf.FloorToInt((Mathf.Max(0f, roomCounter - 50f) / 20f) * room.numberOfEnemiesSpawnFactor);
        }

        enemyIndexCounters.Clear();

        switch(room.id){
            case "TreasureRoom":
                SpawnEnemy(room, false);
                break;
            default:
                int numberOfEnemies = Mathf.CeilToInt(Mathf.FloorToInt(3 + roomCounter * 0.5f) * room.numberOfEnemiesSpawnFactor);
                List<int> bossIndices = new List<int>();

                if (room.canContainBosses && spawnBoss) {
                    List<int> possibleBossIndices = new List<int>();

                    for (int i = 0; i < numberOfEnemies; i++) {
                        possibleBossIndices.Add(i);
                    }

                    for (int i = 0; i < numberOfBosses; i++) {
                        int randIndex = rng.RandiRange(0, possibleBossIndices.Count - 1);
                        bossIndices.Add(possibleBossIndices[randIndex]);
                        possibleBossIndices.RemoveAt(randIndex);
                    }

                    spawnBoss = false;
                }

                for (int i = 0; i < numberOfEnemies; i += 1) {
                    SpawnEnemy(room, bossIndices.Contains(i));
                }
                break;
        }
    }

    private void SpawnEnemy(RoomData room, bool isBoss){
        int enemyTypeIndex = rng.RandiRange(0, roomEnemies[room.id].Count - 1);

        if (!enemyIndexCounters.ContainsKey(enemyTypeIndex)) {
            enemyIndexCounters[enemyTypeIndex] = 0;
        } else {
            enemyIndexCounters[enemyTypeIndex] += 1;
        }

        PackedScene enemyPrefab = roomEnemies[room.id][enemyTypeIndex];
        AbstractEnemy enemy = enemyPrefab.Instance() as AbstractEnemy;

        enemy.isBoss = isBoss;
        enemy.spawnIndex = enemyIndexCounters[enemyTypeIndex];

        float lifeMultiplier = 1f + (roomCounter * 0.05f) * ((250f + roomCounter) / 250f);

        if (isBoss) {
            enemy.SetScale(AbstractEnemy.bossSizeScale);
            lifeMultiplier *= AbstractEnemy.bossLifeScale;
        }

        enemy.GetNode<LifePointManager>("LifePointManager").maxHealth *= lifeMultiplier;
        enemy.GlobalPosition = this.GetRandomSpawnPosition();

        Spawn spawn = spawnPrefab.Instance() as Spawn;
        spawn.Position = enemy.Position;
        spawn.Scale *= isBoss ? 2f : 1f;
        spawn.SetEnemy(enemy);
        GetTree().Root.GetNode<Node>("Main").AddChild(spawn);
        enemyManager.OnEnemySpawn(enemy);
    }

    private Vector2 GetRandomSpawnPosition() {
        return GlobalPosition + new Vector2(
            rng.RandfRange(roomStart.x + spawnWallMargin, roomEnd.x - spawnWallMargin),
            rng.RandfRange(roomStart.y + spawnWallMargin, roomEnd.y - spawnWallMargin)
        );
    }
}
