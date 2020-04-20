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
        
    #endregion

    public override void _Ready()
    {
        InitChilds();
        _backGroundMusic.Stream = idleMusic;
        _backGroundMusic.Play();
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

    public void GameOverSound(bool win)
    {
        PlayAudioTrack(idleMusic, _backGroundMusic);

        if(win)
        {
            PlayAudioTrack(gameOverWinMusic, _notificationSounds);
        }
        else
        {
           PlayAudioTrack(gameOverLoseMusic, _notificationSounds);
        }
    }

    public void PlayLevelMusic()
    {
        PlayAudioTrack(gamePlayMusic, _backGroundMusic);
    }


}
