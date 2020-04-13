using Godot;
using System;
using OMenuClient;
using OMenuClient.Structs;

public class OMenuCommunication
{
    public VLTLocalClient oMenuClient;
    public OMenuClient.Structs.Profile profile;
    public SaveData saveData;
    public PlayInfo playInfo;
    public bool Start()
    {
        oMenuClient = new VLTLocalClient(16);
        oMenuClient.StartService();
        if (oMenuClient.ServiceStarted)
        {
            profile = oMenuClient.GetProfile();
            return true;
        }
        return false;
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

    public double GetCurrentBet()
    {
        return oMenuClient.GetSaveData().betAmount;
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
    public void UpdateSaveData(bool isPlaying, double moneyAmount, double betAmount, DateTime DateTime, String description)
    {
        oMenuClient.UpdateSaveData(isPlaying, moneyAmount, betAmount, DateTime, description);
    }

    public String GetBetDescription()
    {
        return oMenuClient.GetSaveData().description;
    }

    public bool IsPlaying()
    {
        return oMenuClient.GetSaveData().isPlaying;
    }






}