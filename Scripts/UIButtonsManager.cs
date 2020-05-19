using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class UIButtonsManager : Control
{
    Dictionary<string, CurrencyLabel> _myCurrencyLabels = new Dictionary<string, CurrencyLabel>();
    [Export] AudioStream PlayFX;
    [Export] AudioStream ButtonFX;
    [Signal] public delegate void BonusAccepted();
    [Signal] public delegate void restartGame();
    [Signal] public delegate void ClearGameOver();
    [Signal] public delegate void bet();
    [Signal] public delegate void StopGameMusic();
    [Signal] public delegate void maxBet();
    [Signal] public delegate void collect();
    [Signal] public delegate void end_collect();
    [Signal] public delegate void GameOverPopUp();
    [Signal] public delegate void UIIdle();
    [Signal] public delegate void demoMode();
    [Signal] public delegate void TimerDone(bool win);
    [Signal] public delegate void ControlMasterVolume(float volume);
    TextureButton _playButton, _helpButton, _betButton, _maxBetButton, _collectButton, _endAndCollectButton, _okFinishBonusButton, _volumeButton, _buttonStartBonus;
    TextureRect _helpCanvas, _controlPanel, _timerRect, _incomingBonus, _finishedBonus, _nextLevel;
    Control _bonusFeedback;

    AudioStreamPlayer _audio;

    AnimatedSprite _spriteVolumeButton;
    
    AnimationPlayer _timerAnim, _playButtonAnim;

    Label _myTimeLabel;

    Timer _timer = new Timer();
    Timer _timerToSetIdle = new Timer();

    float volume = -20;
    float timeToSetIdle = 10;

    public override void _Ready()
    {
        InitChilds();
        ActivatePlayButton(false);
    }

    public override void _Process(float delta)
    {
        if (_timer.TimeLeft > 0)
        {
            var timeInt = Convert.ToInt32(_timer.TimeLeft);
            _myTimeLabel.Text = timeInt.ToString();
            if (timeInt <= 3)
            {
                _timerAnim.Play("Timing Out");
            }
        }
    }

    #region Funciones de Botones

    void ActivateAgain() //La llama GameManager, señal GameOver!
    {
        _helpButton.Disabled = false;
        _maxBetButton.Disabled = false;
        _betButton.Disabled = false;
        _collectButton.Disabled = false;
        ActivateEndAndCollectButton(false);
    }

    void OnHelpButtonUp()
    {
        PlayAudio();
        _helpCanvas.Show();
        _controlPanel.Hide();
        _timerRect.Hide();
        _volumeButton.Hide();
    }
    void OnBackButtonUp()
    {
        PlayAudio();
        _helpCanvas.Hide();
        _controlPanel.Show();
        _timerRect.Show();
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

    void OnEndAndCollectButtonUp()
    {
        PlayAudio();
        EmitSignal(nameof(end_collect));
    }
    void OnPlayButtonUp()
    {
        _timerToSetIdle.Stop();
        _audio.Stream = PlayFX;
        _audio.Play();
        EmitSignal(nameof(restartGame));
        ActivatePlayButton(false);
        DeactivateButtons();
    }
    void OnOkButtonUp()
    {
        _bonusFeedback.Hide();
        EmitSignal(nameof(BonusAccepted));
        _incomingBonus.Hide();
        _buttonStartBonus.Hide();
    }
    void DeactivateButtons()
    {
        _helpButton.Disabled = true;
        _maxBetButton.Disabled = true;
        _betButton.Disabled = true;
        _collectButton.Disabled = true;
    }
    void ActivateEndAndCollectButton(bool enable)
    {
        if (enable)
        {
            _endAndCollectButton.Show();
        }
        else
        {
            _endAndCollectButton.Hide();
        }
    }
    void ActivatePlayButton(bool enable)
    {

        if (enable)
        {
            _playButton.Disabled = false;
            _playButtonAnim.Play("Jumping");
        }
        else
        {
            _playButton.Disabled = true;
            _playButtonAnim.Play("Idle");
        }
    }
    void ActivateCollectButton()
    {
        ActivateEndAndCollectButton(true);
    }

    void PlayAudio()
    {
        _audio.Stream = ButtonFX;
        _audio.Play();
    }

    #endregion
    
    void InitChilds()
    {
        #region GetNodes

        _incomingBonus = GetNode("ui_IncomingBonus") as TextureRect;
        _finishedBonus = GetNode("ui_FinishedBonus") as TextureRect;
        _controlPanel = GetNode("ControlPanel") as TextureRect;
        _helpCanvas = GetNode("ui_Help") as TextureRect;
        _timerRect = GetNode("Tiempo") as TextureRect;
        _nextLevel = GetNode("ControlPanel/ui_Collect") as TextureRect;
        _volumeButton = GetNode("VolumeButton") as TextureButton;
        _endAndCollectButton = GetNode("ControlPanel/button_end_collect") as TextureButton;
        _playButton = GetNode("ControlPanel/button_Play") as TextureButton;
        _helpButton = GetNode("button_Help") as TextureButton;
        _betButton = GetNode("ControlPanel/button_Bet") as TextureButton;
        _maxBetButton = GetNode("ControlPanel/button_MaxBet") as TextureButton;
        _collectButton = GetNode("ControlPanel/button_Collect") as TextureButton;
        _okFinishBonusButton = GetNode("ControlPanel/button_OkFinishBonus") as TextureButton;
        _myTimeLabel = GetNode("Tiempo/timer") as Label;
        _audio = GetNode("AudioStreamPlayer") as AudioStreamPlayer;
        _spriteVolumeButton = GetNode("VolumeButton/AnimatedSprite") as AnimatedSprite;
        _bonusFeedback = GetNode("BonusFeedback") as Control;
        _buttonStartBonus = GetNode("ui_IncomingBonus/buttonStartBonus") as TextureButton;
        _timerAnim = GetNode("Tiempo/timer/AnimationPlayer") as AnimationPlayer;
        _playButtonAnim = GetNode("ControlPanel/button_Play/AnimationPlayer") as AnimationPlayer;

        #endregion
        
        _myTimeLabel.Text = "--";
        LoadScene(Constants.PATH_UI_GAMEOVR_WIN);
        LoadScene(Constants.PATH_UI_GAMEOVR_LOSE);
        SetUpTimers();
    }
    
    void CreateTimer(float secs, string method)
    {
        var timer = new Timer();
        timer.WaitTime = secs;
        timer.OneShot = true;
        AddChild(timer);
        timer.Start();
        timer.Connect("timeout", this, method);
        timer.Connect("timeout", timer, "queue_free");
    }
    public void SubscribeCurrencyLabel(CurrencyLabel cl)
    {
        //GD.Print("Suscribing " + cl.myType);
        if (!_myCurrencyLabels.ContainsValue(cl))
        {
            
            _myCurrencyLabels.Add(cl.myType, cl);
        }
    }
    public void UnsuscribeCurrencyLabel(CurrencyLabel cl)
    {
        if (_myCurrencyLabels.ContainsValue(cl))
        {
            //GD.Print("Desuscribing " + cl.myType);
            _myCurrencyLabels.Remove(cl.myType);
        }
    }
    void UpdateCurrencyUI(string currencyType, float currency)
    {
        _myCurrencyLabels[currencyType].UpdateLabel(currency);
    }

    void SetGameOverMessage(bool win, bool bonus) //La llama GameManager, señal GameOver
    {
        _nextLevel.Hide();
        ActivateEndAndCollectButton(false);
        StopTimer();
        EmitSignal(nameof(GameOverPopUp), win);
        if(bonus)
        {
            _incomingBonus.Show();
        }
        else
        {
            _timerToSetIdle.WaitTime = 10;
            _timerToSetIdle.Start();
        }
    }

    
    void SetIdle()
    {
        _timerToSetIdle.Stop();
        GD.Print("Setting Idle");
        EmitSignal(nameof(ClearGameOver));
        EmitSignal(nameof(UIIdle));
        EmitSignal(nameof(demoMode));
    }

    void BonusStarted() //La llama GameManager, señal StartBonus
    {
        EmitSignal(nameof(ClearGameOver));
    }

    void BonusPicked() //La llama GameManager, señal bonusOver
    {
        CreateTimer(2.5f,"ShowFinishedBonus");
        DeactivateButtons();
    }

    void ShowFinishedBonus()
    {
        _finishedBonus.Show();
        _okFinishBonusButton.Show();
    }
    void DeactivateFinishButton() //Lo llama GameManager, señal NodePicked
    {
        _endAndCollectButton.Disabled = true;
    }
    void ActivateFinishButton() //Lo llama GameManager, señal LevelCreated
    {
        _endAndCollectButton.Disabled = false;
    }

    public void BonusFeedback()
    {
        _bonusFeedback.Show();
    }

    void ActivateNextLevelRect() // La llama restartGame
    {
        _nextLevel.Show();
    }

    void FinishBonus() //La llama el botón OkFinishBonus
    {
        _finishedBonus.Hide();
        _okFinishBonusButton.Hide();
        ActivateAgain();
        EmitSignal(nameof(StopGameMusic));
    }

    void LoadScene(string pathScene)
    {
        var scene = ResourceLoader.Load(pathScene) as PackedScene;
        var message = scene.Instance() as Control;
        GameOverMessage go = message as GameOverMessage;
        AddChildBelowNode(GetNode("GameOverMessageContainer") as Node, go);
        Connect(nameof(GameOverPopUp), go, nameof(go.ReceiveGameOverPopUp));
        Connect(nameof(restartGame), go, nameof(go.ClearGameOverMessage));
        Connect(nameof(ClearGameOver), go, nameof(go.ClearGameOverMessage));
    }
    #region Timer

    public void StopTimer()
    {
        _timer.Stop();
        _myTimeLabel.Text = "--";
        _timerAnim.Play("Idle");
    }
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

    void SetUpTimers()
    {
        AddChild(_timer);
        AddChild(_timerToSetIdle);
        _timer.Connect("timeout", this, "TimerFinished");
        _timerToSetIdle.Connect("timeout", this, "SetIdle");
    }
    #endregion

    
    void _on_VolumeButton_pressed()
    {
        volume -= 20;

        if (volume == -20)
        {
            _spriteVolumeButton.Animation = "Volume2";
        }
        if (volume == -40)
        {
            _spriteVolumeButton.Animation = "Volume1";
        }
        if (volume == -60)
        {
            _spriteVolumeButton.Animation = "Mute";
            volume = -80;
        }
        if (volume <= -81)
        {
            volume = 0;
        }
        if (volume == 0)
        {
            _spriteVolumeButton.Animation = "Volume3";
        }
        EmitSignal(nameof(ControlMasterVolume), volume);
    }

}
