using Godot;
using System;
using System.Collections.Generic;

public class Panner : Node
{
    [Export] float speed = 1;
    [Export] float time;
    float jumpDistance;
    int allLevels = 10;

    bool accelerating;
    Control backGround;
    Vector2 nextPos;
    Vector2 initPosition;
    float currentSpeed;
    float currenTime;
    float origin;
    float destiny;

    public override void _Ready()
    {
        currentSpeed = speed;
        currenTime = time;
        LoadScenes();
        nextPos = backGround.RectPosition;
        initPosition = backGround.RectPosition;
        jumpDistance =  (initPosition.y / 10) + 200;
    }

    public override void _Process(float delta)
    {
        CheckAcceleration(delta);
    }

    public void BackToOrigin(bool win, bool bonus)
    {
        Panning(10);
        destiny = initPosition.y;
    }
    public void Panning(int currentLevel)
    {
        float newDistance = jumpDistance * currentLevel;
        GD.Print(newDistance);
        if(!accelerating)
        {
            accelerating = true;
            currenTime = 0;
            origin = backGround.RectPosition.y;
            destiny = newDistance;
        }
    }
    void CheckAcceleration(float delta)
    {
        if(accelerating)
        {
            Accelerate(delta);
        }
        // else
        // {
        //     Descelerate(delta);
        // }
    }
    void Accelerate(float delta)
    {
        if(currenTime < 1)
        {
            currenTime += delta * currentSpeed;
        }
        else if(currenTime >= 1){ currenTime = 1;}
        nextPos.x = backGround.RectPosition.x;
        nextPos.y = Mathf.Lerp(origin, destiny, currenTime);
        backGround.RectPosition = nextPos;
        
        if(nextPos.y == destiny)
        {
            GD.Print("NextPos.Y " + nextPos.y + ", y destiny son iguales");
            currentSpeed = speed;
            accelerating = false;
        }
    }
    void Descelerate(float delta)
    {
        if(currentSpeed != speed)
        {
            currenTime += delta;
            currentSpeed = Mathf.Lerp(currentSpeed, speed, currenTime);
        }
    }

    void LoadScenes()
    {
        var scene = ResourceLoader.Load(Constants.PATH_BACKGROUND) as PackedScene;
		backGround = scene.Instance() as Control;
        AddChild(backGround);
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
