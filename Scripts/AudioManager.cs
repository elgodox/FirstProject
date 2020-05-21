using Godot;
using System;
using System.Collections;

public class AudioManager : Node
{
    AudioStreamPlayer _backGroundMusic;
    AudioStreamPlayer _notificationSounds;

    #region AudioTracks
    
    [Export] AudioStream idleMusic;
    [Export] AudioStream gamePlayMusic;
    [Export] AudioStream gameOverWinMusic;
    [Export] AudioStream gameOverLoseMusic;
    [Export] AudioStream bonusMusic;
    [Export] AudioStream winBonusMusic;
        
    #endregion

    public override void _Ready()
    {
        InitChilds();
        _backGroundMusic.Stream = idleMusic;
    }

    public void BonusWinSound()
    {
        PlayAudioTrack(winBonusMusic,_backGroundMusic);
    }

    void StopCurrentMusic()
    {
        var volumeBackGround = _backGroundMusic.VolumeDb;
        var volumeNotifications = _notificationSounds.VolumeDb;
        _backGroundMusic.VolumeDb = -80;
        _notificationSounds.VolumeDb = -80;
        _backGroundMusic.Stop();
        _notificationSounds.Stream = null;
        _notificationSounds.Stop();
        _backGroundMusic.VolumeDb = volumeBackGround;
        _notificationSounds.VolumeDb = volumeNotifications;
    }

    void InitChilds()
    {
        _backGroundMusic = GetNode("BackGroundMusic") as AudioStreamPlayer;
        _notificationSounds = GetNode("NotificationSounds") as AudioStreamPlayer;
    }

    void PlayAudioTrack(AudioStream track, AudioStreamPlayer audioSource)
    {
        audioSource.Stream = track;
        audioSource.Play();
    }

    public void GameOverSound(bool win, bool bonus) //La llama GameManager, señal LevelsOver
    {
        _backGroundMusic.Stop();

        if(win)
        {
            PlayAudioTrack(gameOverWinMusic, _notificationSounds);
        }
        else
        {
           PlayAudioTrack(gameOverLoseMusic, _notificationSounds);
        }
        GD.Print("PlayGameOverSound");
    }

    public void PlayLevelMusic()
    {
        _notificationSounds.Stop();
        PlayAudioTrack(gamePlayMusic, _backGroundMusic);
    }
    
    public void PlayBonusMusic()
    {
        _notificationSounds.Stop();
        PlayAudioTrack(bonusMusic, _backGroundMusic);
    }

    public void _on_NotificationSounds_finished()
    {
        if (!_backGroundMusic.Playing)
            PlayAudioTrack(idleMusic, _backGroundMusic);
    }

    public void _on_BackGroundMusic_finished()
    {
        if (!_backGroundMusic.Playing)
            PlayAudioTrack(idleMusic, _backGroundMusic);
    }
    
    public void _on_UI_ControlMasterVolume(float volume)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"),volume);
    }


}
