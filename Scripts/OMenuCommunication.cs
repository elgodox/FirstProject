using Godot;
using System;
using OMenuClient;
using OMenuClient.Structs;

public class OMenuCommunication
{
    public VLTLocalClient oMenuClient;
    public OMenuClient.Structs.GameProfile profile;

    public SaveData saveData;
    public void Start()
    {
        oMenuClient = new VLTLocalClient(16);
        oMenuClient.StartService();
        profile = oMenuClient.GetGameProfile();
        GD.Print(profile.ToString());
    }

    public double GetMoney()
    {
        return oMenuClient.GetSaveData().moneyAmount;
    }
    public double MaxBet()
    {
        return profile.maxBet;
    }
    public double MinBet()
    {
        return profile.minBet;
    }
    public double MaxPrize()
    {
        return profile.maxPrize;
    }
    public double Percent()
    {
        return profile.percent;
    }
    public double Denomination()
    {
        return profile.denomination;
    }
    public void UpdateSaveData(bool isPlaying,double moneyAmount,double betAmount,DateTime DateTime, String description)
    {
        oMenuClient.UpdateSaveData(isPlaying,moneyAmount,betAmount,DateTime,description);
    }
    




}