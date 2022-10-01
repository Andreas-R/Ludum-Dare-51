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

    public override void _Ready() {
        roomSprite = GetNode<Sprite>("RoomSprite");

        rng.Randomize();

        ChangeToRandomRoom();
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
        roomSprite.Texture = room.roomImage;
    }
}
