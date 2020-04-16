using Godot;
using System;

public class AudioManager : Node
{
    AudioStreamPlayer _backGroundMusic;
    AudioStreamPlayer _notificationSounds;

    #region AudioTracks
    
    [Export] AudioStream idleMusic;
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

    void PlayOneShot(AudioStream track, AudioStreamPlayer audioSource)
    {
        audioSource.Stream = track;
        audioSource.Play();
    }

    public void GameOverSound(bool win)
    {
        if(win)
        {
            PlayOneShot(gameOverWinMusic, _notificationSounds);
        }
        else
        {
           PlayOneShot(gameOverLoseMusic, _notificationSounds);
        }
    }
}
