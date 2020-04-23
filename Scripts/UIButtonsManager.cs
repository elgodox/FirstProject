using Godot;
using System;
using System.Collections.Generic;

public class UIButtonsManager : Control
{
    TextureButton _playButton, _helpButton, _betButton, _maxBetButton, _collectButton;
    TextureRect _helpCanvas, _controlPanel, _timeButton;
    TextureButton _volumeButton;
    Dictionary<string, CurrencyLabel> _myCurrencyLabels = new Dictionary<string, CurrencyLabel>();
    [Export] AudioStream PlayFX;
    [Export] AudioStream ButtonFX;
    [Signal] public delegate void restartGame();
    [Signal] public delegate void bet();
    [Signal] public delegate void maxBet();
    [Signal] public delegate void collect();
    [Signal] public delegate void GameOverPopUp();
    [Signal] public delegate void TimerDone(bool win);
    [Signal] public delegate void ControlMasterVolume(float volume);

    AudioStreamPlayer _audio;

	AnimatedSprite volumeButton;

    Label _myTimeLabel;

    Timer _timer = new Timer();

    Control _gameOverMessage;

    float volume = -20;

    public override void _Ready()
    {
        InitChilds();
        ActivatePlayButton(false);
		volumeButton = GetNode("VolumeButton/AnimatedSprite") as AnimatedSprite;
        _controlPanel = GetNode("ControlPanel") as TextureRect;
        _timeButton = GetNode("Tiempo") as TextureRect;
        _volumeButton = GetNode("VolumeButton") as TextureButton;
		_myTimeLabel.Text = "--";
    }

    public override void _Process(float delta)
    {
        if (_timer.TimeLeft>0)
        {
            var timeInt = Convert.ToInt32(_timer.TimeLeft);
            _myTimeLabel.Text = timeInt.ToString();
        }
    }

    #region Funciones de Botones

    void ActivateAgain(bool win) //La llama GameManager, señal GameOver
    {
        _helpButton.Disabled = false;
        _maxBetButton.Disabled = false;
        _betButton.Disabled = false;
        _collectButton.Disabled = false;
    }

    void OnHelpButtonUp()
    {
        PlayAudio();
        _helpCanvas.Show();
        _controlPanel.Hide();
        _timeButton.Hide();
        _volumeButton.Hide();
    }
    void OnBackButtonUp()
    {
        PlayAudio();
        _helpCanvas.Hide();
        _controlPanel.Show();
        _timeButton.Show();
        _volumeButton.Show();
    }
    void OnBetButtonUp()
    {
        PlayAudio();
        EmitSignal(nameof(bet));
    }
    void OnMaxBetButtonUp()
    {
        PlayAudio();
        EmitSignal(nameof(maxBet));
    }
    void OnCollectButtonUp()
    {
        PlayAudio();
        EmitSignal(nameof(collect));
    }
    void OnPlayButtonUp()
    {
        _audio.Stream = PlayFX;
        _audio.Play();
        EmitSignal(nameof(restartGame));
        ActivatePlayButton(false);
        DeactivateButtons();
    }
    void DeactivateButtons()
    {
        _helpButton.Disabled = true;
        _maxBetButton.Disabled = true;
        _betButton.Disabled = true;
        _collectButton.Disabled = true;
    }
    void ActivatePlayButton(bool enable)
    {

        if (enable)
        {
            _playButton.Disabled = false;
        }
        else
            _playButton.Disabled = true;
    }
    void ActivateCollectButton()
    {
        _collectButton.Disabled = false;
    }

    void PlayAudio()
    {
        _audio.Stream = ButtonFX;
        _audio.Play();
    }

    #endregion
    void InitChilds()
    {
        LoadScene(Constants.PATH_UI_GAMEOVR_WIN);
        LoadScene(Constants.PATH_UI_GAMEOVR_LOSE);

        _playButton = GetNode("ControlPanel/button_Play") as TextureButton;
        _helpButton = GetNode("button_Help") as TextureButton;
        _betButton = GetNode("ControlPanel/button_Bet") as TextureButton;
        _maxBetButton = GetNode("ControlPanel/button_MaxBet") as TextureButton;
        _collectButton = GetNode("ControlPanel/button_Collect") as TextureButton;
        _helpCanvas = GetNode("ui_Help") as TextureRect;
        _myTimeLabel = GetNode("Tiempo/timer") as Label;
        _audio = GetNode("AudioStreamPlayer") as AudioStreamPlayer;
        SetUpTimer();
    }
    public void SubscribeCurrencyLabel(CurrencyLabel cl)
    {
        _myCurrencyLabels.Add(cl.myType, cl);
    }
    public void UnsuscribeCurrencyLabel(CurrencyLabel cl)
    {
        if (_myCurrencyLabels.ContainsValue(cl))
        {
            _myCurrencyLabels.Remove(cl.myType);
        }
    }
    void UpdateCurrencyUI(string currencyType, float currency)
    {
        _myCurrencyLabels[currencyType].UpdateLabel(Convert.ToInt32(currency).ToString());
    }

    void SetGameOverMessage(bool win) //La llama GameManager, señal GameOver
    {
        _timer.Stop();
		_myTimeLabel.Text = "--";
        EmitSignal(nameof(GameOverPopUp), win);
    }

    void LoadScene(string pathScene)
    {
        var scene = ResourceLoader.Load(pathScene) as PackedScene;
        var message = scene.Instance() as Control;
        GameOverMessage go = message as GameOverMessage;
        AddChildBelowNode(GetNode("GameOverMessageContainer") as Node, go);
        Connect(nameof(GameOverPopUp), go, nameof(go.ReceiveGameOverPopUp));
        Connect(nameof(restartGame), go, nameof(go.ClearGameOverMessage));
    }
#region Timer
    void StartTimer(float seconds)
    {
        _timer.WaitTime = seconds;
        _timer.OneShot = true;
        _timer.Start();
    }

    void TimerFinished()
    {
        EmitSignal(nameof(TimerDone), true);
    }

    void SetUpTimer()
    
    {
        AddChild(_timer);
        _timer.Connect("timeout", this, "TimerFinished");
    }
#endregion
    void _on_VolumeButton_pressed()
    {
        volume -= 20;
		
		if (volume == -20)
        {
			volumeButton.Animation = "Volume2";
        }
        if (volume == -40)
        {
			volumeButton.Animation = "Volume1";
        }
		if (volume == -60)
        {
			volumeButton.Animation = "Mute";
            volume=-80;
        }
		if (volume <= -81)
        {
			volume=0;
        }
		if (volume == 0)
        {
			volumeButton.Animation = "Volume3";
        }
        EmitSignal(nameof(ControlMasterVolume), volume);
    }

}
