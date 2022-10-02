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
  public Resource[] spawnableEnemies;

  [Export]
  public AudioStreamSample bgMusicSample;
}