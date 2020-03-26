using Godot;
using System;

public class UIButtonsManager : Control
{
	TextureButton playButton;
	TextureRect helpCanvas;
	[Signal] public delegate void restartGame();
	public override void _Ready()
	{
		playButton = GetNode("button_Play") as TextureButton;
		helpCanvas = GetNode("ui_Help") as TextureRect;
        var Mngr = Owner;

        if(Mngr != null)
        {
            GD.Print(Mngr.Name);
            Mngr.Connect("GameOver", this, "ActivateAgain");
        }
	}
	void PlayButtonUp()
	{
		EmitSignal(nameof(restartGame));
		playButton.Disabled = true;
	}
    void ActivateAgain()
    {
        playButton.Disabled = false;
		GD.Print("ActivateAgain");
    }

	void OnHelpButtonUp()
	{
		helpCanvas.Show();
	}

	void OnBackButtonUp()
	{
		helpCanvas.Hide();
	}
}
