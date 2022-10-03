using Godot;
using System;
using System.Collections.Generic;

public class SoundManager : Node
{
    private AudioStreamPlayer penguinSfxPlayer;

    private AudioStreamPlayer blobSfxPlayer;
    private AudioStreamPlayer laserSfxPlayer;
    private AudioStreamPlayer robotSfxPlayer;

    private AudioStreamPlayer chainLightningSfxPlayer;

    private AudioStreamPlayer bowShotPlayer;

    public static SoundManager instance;

    public enum Sfx {
        penguin,
        blob,
        chainLightning,
        laser,
        robot,
        bowShot
    }
    private Dictionary<Sfx, bool> sfxPlayingMap = new Dictionary<Sfx, bool>();

    public override void _Ready() {
        instance = this;
        penguinSfxPlayer = GetNode<AudioStreamPlayer>("Penguin");
        blobSfxPlayer = GetNode<AudioStreamPlayer>("Blob");
        chainLightningSfxPlayer = GetNode<AudioStreamPlayer>("ChainLightning");
        laserSfxPlayer = GetNode<AudioStreamPlayer>("Laser");
        robotSfxPlayer = GetNode<AudioStreamPlayer>("Robot");
        bowShotPlayer = GetNode<AudioStreamPlayer>("BowShot");
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
            case Sfx.chainLightning:
                return chainLightningSfxPlayer;
            case Sfx.laser:
                return laserSfxPlayer;
            case Sfx.robot:
                return robotSfxPlayer;
            case Sfx.bowShot:
                return bowShotPlayer;
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
