using Godot;
using System;
using OMenuClient;
using OMenuClient.Structs;

public static class OMenuCommunication
{
        public static VLTLocalClient oMenuClient;
        public static OMenuClient.Structs.GameProfile profile;
    public static void Start()
    {
        oMenuClient = new VLTLocalClient(16);
        oMenuClient.StartService();
        profile = oMenuClient.GetGameProfile();
        GD.Print(profile.ToString());
    }

}