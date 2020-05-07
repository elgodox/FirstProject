using Godot;
using System;
using System.Collections.Generic;

public class Panner : Node
{
    private AnimationPlayer _animation;
    int allLevels = 10;
    

    public override void _Ready()
    {
        _animation = GetNode("Background/AnimationPlayer") as AnimationPlayer;
    }

    public void BackToOrigin(bool win, bool bonus)
    {
        Panning(10);
    }
    public void Panning(int currentLevel)
    {
        float currentAnimation = currentLevel;
        _animation.CurrentAnimation = currentAnimation.ToString();
        _animation.Play();
    }
   

    // public void Panning()
    // {
    //     size.x = myBackGrounds[0].RectSize.x * myBackGrounds[0].RectScale.x;
    //     viewPort = GetViewport().GetVisibleRect();
    //     float offSet =  -size.x + (size.x / viewPort.Size.x) + currentSpeed;
    //     foreach (var item in myBackGrounds)
    //     {
    //         item.RectPosition += Vector2.Left * currentSpeed;
    //         GD.Print("posX: "+ item.RectPosition.x);
    //         var currentSize = (item.RectSize.x * item.RectScale.x);
    //         if(item.RectPosition.x <= - (currentSize  + (currentSize % viewPort.Size.x)))
    //         {
    //             GD.Print(item.Name + " desapareció por completo de escena en pos: " + item.RectPosition.x);
    //             item.RectPosition = new Vector2(size.x, size.y);
    //             size.x += (myBackGrounds[0].RectPosition.x % myBackGrounds[1].RectPosition.x);

    //             item.RectPosition = new Vector2(size.x, size.y);
    //             GD.Print("Se posicionó en: " + item.RectPosition.x);
    //         }
    //         //if(currentSizes)
    //         // if(item.RectPosition.x >= offSet)
    //         // {
    //         //     item.RectPosition += Vector2.Left * currentSpeed;
    //         // }
    //         else
    //         {
                
    //             //GD.Print("Size X: " + size.x);
    //             //GD.Print("OffSet: " + size.x / viewPort.Size.x);
    //             //GD.Print("viewPort: " + viewPort);
    //             //GD.Print(item.Name + ", XPos before Jump: " + item.RectPosition.x);
    //             //item.RectPosition = new Vector2(size.x, size.y);
    //             //size.x += (myBackGrounds[0].RectPosition.x % myBackGrounds[1].RectPosition.x);
    //             //GD.Print("Distancia entre el final del 1 y el principio el 2: " + (myBackGrounds[0].RectPosition.x % myBackGrounds[1].RectPosition.x));
    //             //item.RectPosition = new Vector2(size.x, size.y);
    //             //GD.Print(item.Name + ", XPos AFTER Jump: " + item.RectPosition.x);
    //             //GD.Print("Nuevo OriginX: " + (origin.x + (myBackGrounds[0].RectPosition.x % myBackGrounds[1].RectPosition.x)));
    //         }
    //     }
        
    // }
}
