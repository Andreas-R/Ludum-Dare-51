using Godot;
using System;
using System.Collections.Generic;

public class RoomHandler : Node2D {

    public static RoomHandler instance;
    private static RandomNumberGenerator rng = new RandomNumberGenerator();

    [Export]
    public RoomData[] rooms;
    [Export]
    public Vector2 roomStart = new Vector2(-600, -400);
    [Export]
    public Vector2 roomEnd = new Vector2(600, 400);
    [Export]
    public float spawnWallMargin = 50f;
    [Export]
    public int chestSpawnFrequency = 4;

    public bool GuaranteedChestSpawnNextRoom{ get; set; }
    public bool GuaranteedNoChestSpawnNextRoom{ get; set; }

    private Sprite roomSprite;
    private Chest chest;
    private BgMusicHandler bgMusicHandler;

    public int roomCounter = 0;
    private int lastRoomIndex = -1;
    private int spriteIndex = 0;
    private int spriteCount = 0;
    public RoomData currentRoom;
    private EnemyManager enemyManager;
    private static Vector2 scale = new Vector2(5f, 5f);
    private bool spawnBoss = false;

    private Dictionary<string, List<PackedScene>> roomEnemies = new Dictionary<string, List<PackedScene>>();

    public override void _Ready() {
        instance = this;
        roomSprite = GetNode<Sprite>("RoomSprite");
        chest = GetNode<Chest>("Chest");
        bgMusicHandler = GetTree().Root.GetNode<BgMusicHandler>("Main/BgMusicHandler");
        enemyManager = GetTree().Root.GetNode<EnemyManager>("Main/EnemyManager");

        foreach (RoomData room in rooms) {
            roomEnemies[room.id] = new List<PackedScene>();
            foreach (string prefabPath in room.spawnableEnemyPrefabPaths) {
                roomEnemies[room.id].Add(ResourceLoader.Load(prefabPath) as PackedScene);
            }
        }
        
        rng.Randomize();
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeat(-1, 0)) {
             roomSprite.Scale = scale * 1.001f;
            int nextIndex = spriteIndex + 1;
            if (nextIndex < spriteCount) {
                roomSprite.Texture = currentRoom.roomImage[nextIndex];
                spriteIndex = nextIndex;
            } else {
                spriteIndex = 0;
                roomSprite.Texture = currentRoom.roomImage[0];
            }
        } else {
            roomSprite.Scale -= (scale * 0.001f) / (delta * 1000);
            if (roomSprite.Scale.x < scale.x) {
                 roomSprite.Scale = scale;
            }
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
        lastRoomIndex = Array.FindIndex(rooms, room => room == roomsToChoose[randomRoomIndex]);
    }

    private void ChangeToRoom(RoomData room) {
        if(currentRoom != null && currentRoom.id == "TreasureRoom"){
            Goblin survivingGoblin = GetNodeOrNull<Goblin>("../Goblin");
            if(survivingGoblin != null){
                survivingGoblin.OnEscape();
            }
        }

        currentRoom = room;
        spriteCount = room.roomImage.Length;
        roomSprite.Texture = room.roomImage[0];
        
        if (chest.Visible) chest.Despawn();

        if (!GuaranteedNoChestSpawnNextRoom && (GuaranteedChestSpawnNextRoom || roomCounter % chestSpawnFrequency == chestSpawnFrequency - 1)) {
            GuaranteedChestSpawnNextRoom = false;
            chest.Spawn();
        } else {
            this.SpawnEnemies(room);
        }
        bgMusicHandler.ChangeMainBackgroundMusic(room.bgMusicSamples, enemyManager.totalEnemyDifficulty);
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

        switch(room.id){
            case "TreasureRoom":
                SpawnEnemy(room, false);
                break;
            default:
                int numberOfEnemies = Mathf.CeilToInt(Mathf.FloorToInt(3 + roomCounter * 0.5f) * room.numberOfEnemiesSpawnFactor);
                int bossIndex = -1;

                if (room.canContainBosses && spawnBoss) {
                    bossIndex = rng.RandiRange(0, numberOfEnemies - 1);
                    spawnBoss = false;
                }

                for (int i = 0; i < numberOfEnemies; i += 1) {
                    SpawnEnemy(room, i == bossIndex);
                }
                break;
        }
    }

    private void SpawnEnemy(RoomData room, bool isBoss){
        PackedScene enemyPrefab = roomEnemies[room.id][rng.RandiRange(0, roomEnemies[room.id].Count - 1)];
        AbstractEnemy enemy = enemyPrefab.Instance() as AbstractEnemy;

        enemy.isBoss = isBoss;
        float lifeMultiplier = 1f + (roomCounter * 0.05f);

        if (isBoss) {
            enemy.SetScale(AbstractEnemy.bossSizeScale);
            lifeMultiplier *= AbstractEnemy.bossLifeScale;
        }

        enemy.GetNode<LifePointManager>("LifePointManager").maxHealth *= lifeMultiplier;
        enemy.GlobalPosition = this.GetRandomSpawnPosition();

        GetTree().Root.GetNode<Node>("Main").AddChild(enemy);
        enemyManager.OnEnemySpawn(enemy);
    }

    private Vector2 GetRandomSpawnPosition() {
        return new Vector2(
            rng.RandfRange(roomStart.x + spawnWallMargin, roomEnd.x + spawnWallMargin),
            rng.RandfRange(roomStart.y - spawnWallMargin, roomEnd.y - spawnWallMargin)
        );
    }
}
