using Godot;
using System;

public class AudioSlider : VSlider {
    public override void _Ready() {
        SetVolume((float) Value);
    }

    public void OnChange(float value) {
        SetVolume(value);
    }

    private void SetVolume(float linearValue) {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.Log(linearValue) * 10f);
    }
}
