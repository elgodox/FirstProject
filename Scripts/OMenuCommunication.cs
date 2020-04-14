using Godot;
using System;
using OMenuClient;
using OMenuClient.Structs;


public class OMenuCommunication
{
    public VLTLocalClient oMenuClient;
    public Profile profile;
    public SaveData saveData;
    public PlayInfo playInfo;


    public OMenuCommunication()
    {
        oMenuClient = new VLTLocalClient(VLTLocalClient.TypeConncetion.DataBase);
    }


    public void Start(Action OnSuccess, Action OnFail)
    {
        oMenuClient.StartService(16, delegate { GetProfile(OnSuccess, OnFail); }, OnFail);
    }

    public void GetProfile(Action OnSuccess, Action OnFail, Action OnCompletation = null)
    {
        oMenuClient.GetProfile(delegate (Profile profile) { this.profile = profile; OnSuccess?.Invoke(); }, OnFail, OnCompletation);
    }

    public void GetMoney(Action<double> OnSuccess, Action OnFail, Action OnCompletation = null)
    {
        oMenuClient.GetSaveData(delegate (OMenuClient.Structs.SaveData savedata) { OnSuccess?.Invoke(savedata.moneyAmount); }, OnFail, OnCompletation);
    }
    public double MaxBet()
    {
        return profile.maxBet;
    }
    public double MinBet()
    {
        return profile.minBet;
    }
    public void GetCurrentBet(Action<double> OnSuccess, Action OnFail, Action OnCompletation = null)
    {
        oMenuClient.GetSaveData(delegate (OMenuClient.Structs.SaveData saveData) { OnSuccess.Invoke(saveData.betAmount); }, OnFail, OnCompletation);
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
    public void UpdateSaveData(SaveData saveData, Action OnSuccess, Action OnFail)
    {
        oMenuClient.UpdateSaveData(saveData, OnSuccess, OnFail);
    }
    public void GetBetDescription(Action<string> OnSuccess, Action OnFail, Action OnCompletation = null)
    {
        oMenuClient.GetSaveData(delegate (OMenuClient.Structs.SaveData saveData) { OnSuccess?.Invoke(saveData.description); }, OnFail, OnCompletation);
    }


    public void IsPlaying(Action OnSuccess, Action OnFail)
    {
        oMenuClient.GetSaveData(delegate (OMenuClient.Structs.SaveData savedata) { if (savedata.isPlaying) OnSuccess?.Invoke(); }, OnFail);
    }

    public void GetSaveData(Action<SaveData> OnSuccess, Action OnFail, Action OnCompletation = null)
    {
        oMenuClient.GetSaveData(delegate (OMenuClient.Structs.SaveData saveData) { OnSuccess?.Invoke(saveData); }, OnFail, OnCompletation);
    }






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