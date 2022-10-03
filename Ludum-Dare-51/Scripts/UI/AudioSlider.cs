using Godot;

public class AudioSlider : VSlider {
    private static float AUDIO_VALUE = 1f;

    public override void _Ready() {
        Value = AUDIO_VALUE;
        SetVolume((float) Value);
    }

    public void OnChange(float value) {
        AUDIO_VALUE = value;
        SetVolume(value);
    }

    private void SetVolume(float linearValue) {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.Log(linearValue) * 10f);
    }
}
