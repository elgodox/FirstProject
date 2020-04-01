using Godot;
using System;
using System.Collections.Generic;

public class UIButtonsManager : Control
{
	TextureButton playButton, helpButton, betButton, maxBetButton, collectButton;
	TextureRect helpCanvas;
	Dictionary<string, CurrencyLabel> myCurrencyLabels = new Dictionary<string, CurrencyLabel>();
	[Signal] public delegate void restartGame();
	[Signal] public delegate void bet();
	[Signal] public delegate void maxBet();
	[Signal] public delegate void collect();
	[Signal] public delegate void GameOverPopUp();
	
	Control gameOverMessage;

	public override void _Ready()
	{
		InitChilds();
		ActivatePlayButton(false);
	}
    
	#region Funciones de Botones

	void ActivateAgain(bool win) //La llama GameManager, señal GameOver
    {
		ActivatePlayButton(win);
		helpButton.Disabled = !win;
		//maxBetButton.Disabled = win;
		//betButton.Disabled = win;
		//collectButton.Disabled = win;
    }
	
	void OnHelpButtonUp()
	{
		helpCanvas.Show();
	}
	void OnBackButtonUp()
	{
		helpCanvas.Hide();
	}
	void OnBetButtonUp()
	{
		EmitSignal(nameof(bet));
	}
	void OnMaxBetButtonUp()
	{
		EmitSignal(nameof(maxBet));
	}
	void OnCollectButtonUp()
	{
		EmitSignal(nameof(collect));
	}
	void OnPlayButtonUp() 
	{
		EmitSignal(nameof(restartGame));
		ActivatePlayButton(false);
		helpButton.Disabled = true;
		//maxBetButton.Disabled = true;
		//betButton.Disabled = true;
		//collectButton.Disabled = true;
	}
	void ActivatePlayButton(bool enable)
	{

		if(enable)
		{
			playButton.Disabled = false;
		}
		else
			playButton.Disabled = true;
	}
	
	#endregion
	void InitChilds()
	{
		LoadScene(Constants.ui_GmOvr_win_path);
		LoadScene(Constants.ui_GmOvr_lose_path);

		playButton = GetNode("button_Play") as TextureButton;
		helpButton = GetNode("button_Help") as TextureButton;
		betButton = GetNode("button_Bet") as TextureButton;
		maxBetButton = GetNode("button_MaxBet") as TextureButton;
		collectButton = GetNode("button_Collect") as TextureButton;
		helpCanvas = GetNode("ui_Help") as TextureRect;
	}
	public void SubscribeCurrencyLabel(CurrencyLabel cl)
	{
		myCurrencyLabels.Add(cl.myType, cl);
	}
	public void UnsuscribeCurrencyLabel(CurrencyLabel cl)
	{
		if(myCurrencyLabels.ContainsValue(cl))
		{
			myCurrencyLabels.Remove(cl.myType);
		}
	}
	void UpdateCurrencyUI(string currencyType, float currency)
	{
		myCurrencyLabels[currencyType].UpdateLabel(currency.ToString());
	}

	void SetGameOverMessage(bool win) //La llama GameManager, señal GameOver
	{
		EmitSignal(nameof(GameOverPopUp), win);
	}

	void LoadScene(string pathScene)
	{
		var scene = ResourceLoader.Load(pathScene) as PackedScene;
		var message = scene.Instance() as Control;
		GameOverMessage go = message as GameOverMessage;
		AddChild(go);
		Connect(nameof(GameOverPopUp), go, nameof(go.ReceiveGameOverPopUp));
		Connect(nameof(restartGame), go, nameof(go.ClearGameOverMessage));
	}
}
