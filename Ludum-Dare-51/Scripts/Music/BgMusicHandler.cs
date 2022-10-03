using Godot;
using System;

public class BgMusicHandler : Node
{
    private double audioServerDelay;
    private float fadeInDuration = 0.3f;

    private float bgOff = -80f;
    private float bgOn = 0f;

    private Tween musicFadeInTween;

    public AudioStreamPlayer currentMainBgPlayer;
    public AudioStreamPlayer nextMainBgPlayer;
    private AudioStreamPlayer recordScratchPlayer;
    private RoomHandler roomHandler;
    private bool isCurrentBgPlaying;
    [Export]
    private float difficultyThresholdMedium = 10;
    [Export]
    private float difficultyThresholdFast = 20;
    private enum MusicSpeed {
        SLOW,
        MEDIUM,
        FAST
    }

    private MusicSpeed currentMusicSpeed;

    public override void _Ready() {
        currentMusicSpeed = MusicSpeed.SLOW;
        audioServerDelay = AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency();
        musicFadeInTween = GetNode<Tween>("MusicFadeInTween");
        recordScratchPlayer = GetNode<AudioStreamPlayer>("RecordScratchPlayer");
        currentMainBgPlayer = GetNode<AudioStreamPlayer>("BgPlayer");
        nextMainBgPlayer = GetNode<AudioStreamPlayer>("NextBgPlayer");
        roomHandler = GetTree().Root.GetNode<RoomHandler>("Main/RoomHandler");
        isCurrentBgPlaying = true;
    }

    private MusicSpeed GetMusicSpeedFromDifficulty(float totalEnemyDifficulty) {
         if (totalEnemyDifficulty >= difficultyThresholdFast) {
            return  MusicSpeed.FAST;
        } else if (totalEnemyDifficulty >= difficultyThresholdMedium) {
            return MusicSpeed.MEDIUM;
        }

        return MusicSpeed.SLOW;
    }

    public void OnDifficultyChanged(float totalEnemyDifficulty) {
        var currentPlayer = isCurrentBgPlaying ? currentMainBgPlayer : nextMainBgPlayer;
        // dont queue other difficulty if already at end of song
        if (currentPlayer.GetPlaybackPosition() >= 9f) {
            return;
        }
        MusicSpeed newMusicSpeed = GetMusicSpeedFromDifficulty(totalEnemyDifficulty);
        if (newMusicSpeed != currentMusicSpeed) {
            var sample = GetCorrectDifficultySample(roomHandler.currentRoom.bgMusicSamples, totalEnemyDifficulty);
            ChangeMainBackgroundMusic(sample, newMusicSpeed, false);
        }
    }

    public AudioStreamOGGVorbis GetCorrectDifficultySample(AudioStreamOGGVorbis[] samples, float totalEnemyDifficulty) {
        MusicSpeed newMusicSpeed = GetMusicSpeedFromDifficulty(totalEnemyDifficulty);
        if (newMusicSpeed == MusicSpeed.FAST) {
            return samples[samples.Length - 1];
        } else if (newMusicSpeed == MusicSpeed.MEDIUM && samples.Length > 1) {
            return samples[samples.Length - 2];
        }

        return samples[0];
    }

    private void ChangeMainBackgroundMusic(AudioStreamOGGVorbis sample, MusicSpeed newMusicSpeed, bool playRecordScratch = true) {
        var fadeOutPlayer = isCurrentBgPlaying ? currentMainBgPlayer : nextMainBgPlayer;
        var fadeInPlayer = isCurrentBgPlaying ? nextMainBgPlayer : currentMainBgPlayer;

        musicFadeInTween.InterpolateProperty(fadeOutPlayer, "volume_db", bgOn, bgOff, fadeInDuration,
            Tween.TransitionType.Linear, Tween.EaseType.In);
        musicFadeInTween.Start();

        if (playRecordScratch) {
            recordScratchPlayer.Play();
        }
        fadeInPlayer.Stream = sample;
        PlayAudio(fadeInPlayer, Math.Max(0, (float)(Metronome.instance.elapsedTime + audioServerDelay)));
        isCurrentBgPlaying = !isCurrentBgPlaying;
        currentMusicSpeed = newMusicSpeed;
    }

    public void ChangeMainBackgroundMusic(AudioStreamOGGVorbis[] samples, float totalEnemyDifficulty) {
        MusicSpeed newMusicSpeed = GetMusicSpeedFromDifficulty(totalEnemyDifficulty);
        var sample = GetCorrectDifficultySample(samples, totalEnemyDifficulty);
        ChangeMainBackgroundMusic(sample, newMusicSpeed);
    }

    public void PlayAudio(AudioStreamPlayer player) {
        // sync millisecond, but not seconds
        var currentPlayer = isCurrentBgPlaying ? currentMainBgPlayer : nextMainBgPlayer;
        double nextSecond = Math.Ceiling(currentPlayer.GetPlaybackPosition());
        double diff = nextSecond - currentPlayer.GetPlaybackPosition();

        PlayAudio(player, (float) (1 - diff));
    }

    public void PlayAudio(AudioStreamPlayer player, float fromPosition) {
        player.VolumeDb = bgOff;
        musicFadeInTween.InterpolateProperty(player, "volume_db", bgOff, bgOn, fadeInDuration,
            Tween.TransitionType.Linear, Tween.EaseType.In);
        player.Play(fromPosition);
        musicFadeInTween.Start();
    }
}
