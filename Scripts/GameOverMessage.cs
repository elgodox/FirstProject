using Godot;
using System;

public class GameOverMessage : Control
{
    public Action ClearMe;
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
        if (RectScale.y > 0.1f || myAnim.CurrentAnimation == "Entrar")
        {
            myAnim.StartExitAnimation();
        }
    }
}
