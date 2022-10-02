using Godot;
using System;
using System.Collections.Generic;

public class RoomHandler : Node2D {
    [Export]
    public RoomData[] rooms;

    private Sprite roomSprite;

    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private int roomCounter = 0;
    private int lastRoomIndex = -1;
    private int spriteIndex = 0;
    private int spriteCount = 0;
    private RoomData currentRoom;
    private static Vector2 scale = new Vector2(5f, 5f);
    public override void _Ready() {
        roomSprite = GetNode<Sprite>("RoomSprite");
        
        rng.Randomize();
        ChangeToRandomRoom();
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsFrame(-1, 0)) {
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
        currentRoom = room;
        spriteCount = room.roomImage.Length;
        roomSprite.Texture = room.roomImage[0];
    }
}
