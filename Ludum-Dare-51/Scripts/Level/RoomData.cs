using Godot;

public class RoomData : Resource {
  [Export]
  public string id;

  [Export]
  public Texture[] roomImage;

  [Export]
  public float spawnWeight;

  [Export]
  public int minSpawnRoomCount;

  [Export]
  public string[] spawnableEnemyPrefabPaths;

  [Export]
  public float numberOfEnemiesSpawnFactor = 1f;

  [Export]
  public bool canContainBosses = true;

  [Export]
  public AudioStreamSample[] bgMusicSamples;
}