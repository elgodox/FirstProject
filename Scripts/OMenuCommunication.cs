using Godot;
using System;
using OMenuClient;
using OMenuClient.Structs;

public class OMenuCommunication : Node
{
    public override void _Ready()
    {
        VLTLocalClient oMenuClient = new VLTLocalClient(16);
        GD.Print(oMenuClient.GameProfileName);


        oMenuClient.StartService();
        OMenuClient.Structs.GameProfile profile = oMenuClient.GetGameProfile();
        GD.Print(profile.ToString());
        
    }

}
