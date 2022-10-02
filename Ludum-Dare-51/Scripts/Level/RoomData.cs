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
  public AudioStreamSample bgMusicSample;
}