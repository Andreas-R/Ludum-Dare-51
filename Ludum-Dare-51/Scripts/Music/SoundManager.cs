using Godot;
using System;
using System.Collections.Generic;

public class SoundManager : Node
{
    private AudioStreamPlayer penguinSfxPlayer;

    private AudioStreamPlayer blobSfxPlayer;
    private AudioStreamPlayer laserSfxPlayer;
    private AudioStreamPlayer robotSfxPlayer;

    public enum Sfx {
        penguin,
        blob,
        laser,
        robot
    }
    private Dictionary<Sfx, bool> sfxPlayingMap = new Dictionary<Sfx, bool>();

    public override void _Ready() {
        penguinSfxPlayer = GetNode<AudioStreamPlayer>("Penguin");
        blobSfxPlayer = GetNode<AudioStreamPlayer>("Blob");
        laserSfxPlayer = GetNode<AudioStreamPlayer>("Laser");
        robotSfxPlayer = GetNode<AudioStreamPlayer>("Robot");
        EmptySfxPlayingMap();
    }

    public void EmptySfxPlayingMap() {
        foreach (var key in Enum.GetValues(typeof(Sfx))) {
            sfxPlayingMap[(Sfx) key] = false;
        }
    }

    public void PlaySfx(Sfx sfx) {
        if (sfxPlayingMap[sfx]) return;

        sfxPlayingMap[sfx] = true;
        GetAudioPlayerForSfx(sfx).Play();
    }

    public AudioStreamPlayer GetAudioPlayerForSfx(Sfx sfx) {
        switch(sfx) {
            case Sfx.penguin:
                return penguinSfxPlayer;
            case Sfx.blob:
                return blobSfxPlayer;
            case Sfx.laser:
                return laserSfxPlayer;
            case Sfx.robot:
                return robotSfxPlayer;
            default:
                return null;
        }
    }

    public override void _Process(float delta) {
        if (Metronome.instance.IsBeatWithAudioDelay(new int[]{-1}, new float[]{0.25f, 0.5f, 0.75f, 1})) {
            EmptySfxPlayingMap();
        }
    } 
}
