using Godot;
using System;
using static Godot.TextServer;

public partial class Player : CharacterBody2D
{
	private bool isMoving;

	[Export]
    public float MoveSpeed { get; private set; } = 500f;


    public int PlayerBottomPosition { get; private set; }


    private Vector2 screenSize;

    public Vector2 InputVector { get; private set; }

    public bool IsPlayerOne { get; set; } = true;

    public bool Start = false;

    private bool isAi = false;
    public bool IsAi { 
        get { return isAi; } 
        set {
            isAi = value;
            if (isAi == true)
            {
                aiPlayer = GD.Load<PackedScene>("res://Source/AiPlayer/ai_player.tscn").Instantiate() as AiPlayer;
                this.AddChild(aiPlayer);
            }
        
        } } 

    private AiPlayer aiPlayer;

    public override void _Ready()
	{
        isMoving = false;

        PlayerBottomPosition = this.GetNode<Sprite2D>("Sprite2D").Texture.GetHeight();
        
        screenSize = GetViewportRect().Size;

    }


    public void GiveBallPosition(double delta, Vector2 ballPosition, Vector2 ballMoveDirection)
    {
        if (isAi)
        {
            aiPlayer.GiveBallPosition(delta, ballPosition, ballMoveDirection);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isAi)
        {
            if (Start)
            {
                Move(delta);
            }
        }
    }


    public void Move(double delta)
    {
        InputVector = Vector2.Zero;

        if (IsPlayerOne)
        {
            if (Input.IsActionPressed("PlayerOneMoveUp"))
            {
                InputVector = new Vector2(0, InputVector.Y - 1);
            }

            else if (Input.IsActionPressed("PlayerOneMoveDown"))
            {
                InputVector = new Vector2(0, InputVector.Y + 1);
            }
        }
        else
        {
            if (Input.IsActionPressed("PlayerTwoMoveUp"))
            {
                InputVector = new Vector2(0, InputVector.Y - 1);
            }

            else if (Input.IsActionPressed("PlayerTwoMoveDown"))
            {
                InputVector = new Vector2(0, InputVector.Y + 1);
            }

        }


        InputVector = InputVector.Normalized() * MoveSpeed;

        Velocity = InputVector * (float)delta;

        MoveAndCollide(Velocity);
    }



}
