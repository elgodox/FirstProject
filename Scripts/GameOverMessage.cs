using Godot;
using System;

public class GameOverMessage : Control
{
    [Export] bool MyWin;

    AnimationControl myAnim;
    public override void _Ready()
    {
        myAnim = GetNode("AnimationPlayer") as AnimationControl;
    }
    public void ReceiveGameOverPopUp(bool win)
    {
        if(win == MyWin)
        {
            myAnim.StartEnterAnimation();
        }
    }

    public void ClearGameOverMessage()
    {
        myAnim.StartExitAnimation();
    }
}
