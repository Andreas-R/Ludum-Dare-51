using Godot;
using System;

public class BgMusicHandler : Node
{
    private double audioServerDelay;
    private float fadeInDuration = 0.5f;

    private float bgOff = -80f;
    private float bgOn = 0f;

    private Tween musicFadeInTween;

    public AudioStreamPlayer currentMainBgPlayer;
    public AudioStreamPlayer nextMainBgPlayer;
    private AudioStreamPlayer recordScratchPlayer;
    private bool isCurrentBgPlaying;

    public override void _Ready() {
        audioServerDelay = AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency();
        musicFadeInTween = GetNode<Tween>("MusicFadeInTween");
        recordScratchPlayer = GetNode<AudioStreamPlayer>("RecordScratchPlayer");
        currentMainBgPlayer = GetNode<AudioStreamPlayer>("BgPlayer");
        nextMainBgPlayer = GetNode<AudioStreamPlayer>("NextBgPlayer");
        isCurrentBgPlaying = true;
    }

    public void ChangeMainBackgroundMusic(AudioStreamSample sample) {
        var fadeOutPlayer = isCurrentBgPlaying ? currentMainBgPlayer : nextMainBgPlayer;
        var fadeInPlayer = isCurrentBgPlaying ? nextMainBgPlayer : currentMainBgPlayer;

        recordScratchPlayer.Play();
        fadeInPlayer.Stream = sample;
        PlayAudio(fadeInPlayer, Math.Max(0, (float)(Metronome.instance.elapsedTime + audioServerDelay)));
        isCurrentBgPlaying = !isCurrentBgPlaying;
    }

    public void PlayAudio(AudioStreamPlayer player) {
        // sync millisecond, but not seconds
        var currentPlayer = isCurrentBgPlaying ? currentMainBgPlayer : nextMainBgPlayer;
        double nextSecond = Math.Ceiling(currentPlayer.GetPlaybackPosition());
        double diff = nextSecond - currentPlayer.GetPlaybackPosition();

        PlayAudio(player, (float) (1 - diff));
    }

    public void PlayAudio(AudioStreamPlayer player, float fromPosition) {
        player.VolumeDb = bgOn;
        musicFadeInTween.InterpolateProperty(player, "volume_db", bgOff, bgOn, fadeInDuration,
            Tween.TransitionType.Linear, Tween.EaseType.In);
        player.Play(fromPosition);
        musicFadeInTween.Start();
    }

    public override void _Process(float delta)
    {

    }    
}
