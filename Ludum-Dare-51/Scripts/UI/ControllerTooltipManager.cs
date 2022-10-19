using Godot;
using System;
using System.Collections.Generic;

class ControllerTooltip{
    public Sprite spriteElement;
    public string controllerKey;

    public ControllerTooltip(Sprite spriteElement, string controllerKey){
        this.spriteElement = spriteElement;
        this.controllerKey = controllerKey;
    }
}

public class ControllerTooltipManager : Node
{
    private List<ControllerTooltip> tooltips = new List<ControllerTooltip>();
    public Dictionary<string, Dictionary<string, Texture>> controllerButtonTextures;

    public string[] controllerTypes = {"xbox", "playstation"};
    public string[] controllerButtons = {"r_down", "r_left", "r_right", "r_up"};

    public string currentController = "";

    public static ControllerTooltipManager instance;
    
    public override void _Ready()
    {
        instance = this;
        Input.Singleton.Connect("joy_connection_changed", this, "_on_joy_connection_changed");
        controllerButtonTextures = new Dictionary<string, Dictionary<string, Texture>>();

        foreach (string controllerType in controllerTypes){
            controllerButtonTextures.Add(controllerType, new Dictionary<string, Texture>());
            foreach(string controllerButton in controllerButtons){
                controllerButtonTextures[controllerType].Add(controllerButton, ResourceLoader.Load($"res://Textures/controller_buttons/{controllerType}/{controllerButton}.png") as Texture);
            }
        }

        checkForConnectedControllers();
    }

    public void registerTooltipNode(Sprite sprite, string controllerKey){
        ControllerTooltip newTooltip = new ControllerTooltip(sprite, controllerKey);
        changeTooltipToController(newTooltip, currentController);
        tooltips.Add(newTooltip);
    }

    private void _on_joy_connection_changed(int device_id, bool connected){
        if(connected && currentController == ""){
            changeTooltipsToController(device_id);
        }
        else{
            checkForConnectedControllers();
        }
    }

    private static string getInternalControllerType(int device_id){
        string joyname = Input.GetJoyName(device_id);
        if(joyname.Contains("PS4") || joyname.Contains("PS5")){
            return "playstation";
        }
        else{
            return "xbox";
        }
    }

    private void checkForConnectedControllers(){
        if(Input.GetConnectedJoypads().Count > 0){
            changeTooltipsToController((int)Input.GetConnectedJoypads()[0]);
        }
        else{
            removeControllerTooltips();
        }
    }

    private void changeTooltipsToController(int device_id){
        string controllerType = getInternalControllerType(device_id);
        currentController = controllerType;
        foreach (ControllerTooltip tooltip in tooltips){
            changeTooltipToController(tooltip, controllerType);
        }
    }

    private void changeTooltipToController(ControllerTooltip tooltip, string controllerType){
        tooltip.spriteElement.Visible = true;
        tooltip.spriteElement.Texture = getButtonTexture(tooltip.controllerKey, controllerType);
    }

    private void removeControllerTooltips(){
        foreach (ControllerTooltip tooltip in tooltips){
            tooltip.spriteElement.Visible = false;
        }
        currentController = "";
    }

    private Texture getButtonTexture(string controllerKey, string controllerType){
        if(controllerButtonTextures.ContainsKey(controllerType) && controllerButtonTextures[controllerType].ContainsKey(controllerKey)){
            return controllerButtonTextures[controllerType][controllerKey];
        }
        else{
            return null;
        }
    }
}
