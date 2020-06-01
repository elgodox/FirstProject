using Godot;
using System;
using Client = OMenuClient.OMenuClient;
using OMenuClient.Structs;


public class OMenuCommunication
{
    public Client oMenuClient;
    //public Profile profile;
    public SaveData saveData;
    public PlayInfo playInfo;


    public OMenuCommunication()
    {
        oMenuClient = new Client(Client.TypeConncetion.DataBase);
    }


    public void Start()
    {
        oMenuClient.StartService(16);
    }
    public Profile GetProfile()
    {
        return oMenuClient.GetProfile();
    }
    public SaveData GetSaveData()
    {
        return oMenuClient.GetSaveData();
    }
    public double GetMoney()
    {
        return GetSaveData().moneyAmount;
    }
    public double MaxBet()
    {
        return GetProfile().maxBet;
    }
    public double MinBet()
    {
        return GetProfile().minBet;
    }
    public double GetCurrentBet()
    {
        return GetSaveData().betAmount;
    }
    public double MaxPrize()
    {
        return GetProfile().maxPrize;
    }
    public double Percent()
    {
        return GetProfile().percent;
    }
    public double Denomination()
    {
        return GetProfile().denomination;
    }
    public string GetBetDescription()
    {
        return GetSaveData().description;
    }
    public bool IsPlaying()
    {
        return GetSaveData().isPlaying;
    }
    public void UpdateSaveData(SaveData saveData)
    {
        oMenuClient.UpdateSaveData(saveData);
    }

    // public void IsPlaying(Action OnSuccess, Action OnFail)
    // {
    //     oMenuClient.GetSaveData(delegate (OMenuClient.Structs.SaveData savedata) { if (savedata.isPlaying) OnSuccess?.Invoke(); }, OnFail);
    // }




}


/*

public struct SaveData
{
    public readonly bool isPlaying;
    public readonly double moneyAmount;
    public readonly double betAmount;
    public readonly DateTime date;
    public readonly string description;


    public SaveData(bool isPlaying, double moneyAmount, double betAmount, DateTime date, string description)
    {
        this.isPlaying = isPlaying;
        this.moneyAmount = moneyAmount;
        this.betAmount = betAmount;
        this.date = date;
        this.description = description;
    }


    public static implicit operator SaveData(OMenuClient.Structs.SaveData saveData)
    {
        return new SaveData(saveData.isPlaying, saveData.moneyAmount,saveData.betAmount, saveData.date, saveData.description);
    }
    
    public static implicit operator OMenuClient.Structs.SaveData(SaveData saveData)
    {
        return new SaveData(saveData.isPlaying, saveData.moneyAmount, saveData.betAmount, saveData.date, saveData.description);
    }

}
*/