using Godot;
using System;

public class BgMusicHandler : Node2D
{
    private double audioServerDelay;
    private float fadeInDuration = 0.3f;

    private float bgOff = -80f;
    private float bgOn = 0f;

    private Tween musicFadeInTween;

    private AudioStreamPlayer bgPlayer;

    public override void _Ready() {
        audioServerDelay = AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency();
        musicFadeInTween = GetNode<Tween>("MusicFadeInTween");
        bgPlayer = GetNode<AudioStreamPlayer>("BgPlayer");
        playMainAudio();
    }

    public void playMainAudio() {
        playAudio(bgPlayer, Metronome.instance.elapsedTime + (float) audioServerDelay);
    }

    public void playAudio(AudioStreamPlayer player) {
        // sync millisecond, but not seconds
        double nextSecond = Math.Ceiling(bgPlayer.GetPlaybackPosition());
        double diff = nextSecond - bgPlayer.GetPlaybackPosition();

        playAudio(player, (float) (1 - diff));
    }

    public void playAudio(AudioStreamPlayer player, float fromPosition) {
        player.VolumeDb = bgOff;
        musicFadeInTween.InterpolateProperty(player, "volume_db", bgOff, bgOn, fadeInDuration,
            Tween.TransitionType.Linear, Tween.EaseType.In);     
        player.Play(fromPosition);
        musicFadeInTween.Start();
    }

    public override void _Process(float delta)
    {

    }    
}
