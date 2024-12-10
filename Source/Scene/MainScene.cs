using Godot;
using System;
using static Godot.TabContainer;

public partial class MainScene : Node2D
{
	private Camera2D camera;
	private Player playerOne;
	private Player playerTwo;
	private Ball ball;

	public Vector2 ScreenSize { get; private set; }
	public Vector2 CameraCenter { get; private set; }
	private Vector2 topBorder;
	private Vector2 bottomBorder;

	private int awayFromScreen = 64;
	private float topPosition;
	private float bottomPosition;
	private float leftPosition;
	private float rightPosition;

	private int playerBorderFromCenter;

	private int playerOneScore = 0;
	private int playerTwoScore = 0;

	private Label playerOneScoreLabel;
	private Label playerTwoScoreLabel;

	private VBoxContainer titleScreen;
	private bool start;


	private PackedScene playerSwingUiScene;

    private PackedScene scoreBoardUiScene;

    private CanvasLayer canvasLayer;

	private PlayerSwing playerSwing;


    public override void _Ready()
	{
		camera = this.GetNode<Camera2D>("Camera2D");
		playerOne = this.GetNode<Player>("Player1");
		playerTwo = this.GetNode<Player>("Player2");
		ball = this.GetNode<Ball>("Ball");


        titleScreen = this.GetNode<VBoxContainer>("CanvasLayer/TitleScreen");


        CameraCenter = camera.GetScreenCenterPosition();

		ScreenSize = GetViewportRect().Size;


		topPosition = CameraCenter.Y - ScreenSize.Y / 2;
		bottomPosition = CameraCenter.Y + ScreenSize.Y / 2;


		leftPosition = CameraCenter.X - ScreenSize.X / 2 ;
		rightPosition = CameraCenter.X + ScreenSize.X / 2;

		topBorder = new Vector2(0, topPosition);
		bottomBorder = new Vector2(0, bottomPosition);



		playerBorderFromCenter = playerOne.PlayerBottomPosition / 2;

        ball.SetBorders(ScreenSize, CameraCenter);

        Reset();

		playerSwingUiScene = GD.Load<PackedScene>("res://Source/UI/player_swing.tscn");
        scoreBoardUiScene = GD.Load<PackedScene>("res://Source/ScoreBoard/score_board.tscn");
        start = false;

		canvasLayer = this.GetNode<CanvasLayer>("CanvasLayer");
    }


    private void StopObjects()
    {
        this.start = false;
        ball.Start = false;
        playerOne.Start = false;
        playerTwo.Start = false;
    }

    private void StartObjects()
    {
        this.start = true;
        ball.Start = true;
        playerOne.Start = true;
        playerTwo.Start = true;
    }

    public void StartGame()
    {
        titleScreen.QueueFree();



        ScoreBoard scoreBoard = scoreBoardUiScene.Instantiate() as ScoreBoard;

        canvasLayer.AddChild(scoreBoard);


        playerOneScoreLabel = this.GetNode<Label>("CanvasLayer/ScoreBoard/PlayerOneScore");
        playerTwoScoreLabel = this.GetNode<Label>("CanvasLayer/ScoreBoard/PlayerTwoScore");

        UpdateScoreLabels();
        StartObjects();
        Reset();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
		playerOne.GlobalPosition = playerOne.GlobalPosition.Clamp(new Vector2(playerOne.GlobalPosition.X, topBorder.Y + playerBorderFromCenter), new Vector2(playerOne.GlobalPosition.X, bottomBorder.Y - playerBorderFromCenter));

		playerTwo.GlobalPosition = playerTwo.GlobalPosition.Clamp(new Vector2(playerTwo.GlobalPosition.X, topBorder.Y + playerBorderFromCenter), new Vector2(playerTwo.GlobalPosition.X, bottomBorder.Y - playerBorderFromCenter));
	
		
		if(this.start == false)
		{
			if (Input.IsActionPressed("Swing"))
			{
				if(playerSwing != null)
				{
                    StartObjects();
					playerSwing.QueueFree();
                }
			}
		}
		else
		{
            if (playerTwo.IsAi)
            {
                playerTwo.GiveBallPosition(delta, ball.GlobalPosition, ball.MoveDirection);
            }
        }

	}

	private void Reset()
	{
        playerOne.GlobalPosition = new Vector2(leftPosition + awayFromScreen, CameraCenter.Y);
        playerTwo.GlobalPosition = new Vector2(rightPosition - awayFromScreen, CameraCenter.Y);
        ball.GlobalPosition = CameraCenter;
    }

    private void UpdateScoreLabels()
	{
        playerOneScoreLabel.Text = playerOneScore.ToString();
        playerTwoScoreLabel.Text = playerTwoScore.ToString();
    }


	private void LetPlayerSwing(bool moveLeft)
	{
        playerSwing = playerSwingUiScene.Instantiate() as PlayerSwing;

        canvasLayer.AddChild(playerSwing);


        StopObjects();

		if (moveLeft)
		{
            playerSwing.GetNode<Label>("PlayerSwingLabel").Text = "PlayerOne to Swing";
        }
		else
		{
            playerSwing.GetNode<Label>("PlayerSwingLabel").Text = "PlayerTwo to Swing";
			if (playerTwo.IsAi)
			{
                StartObjects();
                playerSwing.QueueFree();
            }
        }

        Reset();
	}

	
	public void _on_ball_score_increase(bool moveLeft){

		if (moveLeft)
		{
			playerTwoScore++;
		}
		else
		{
			playerOneScore++;
		}
		UpdateScoreLabels();

        LetPlayerSwing(moveLeft);
		ball.Reset(true, moveLeft);
    }


	public void StartSinglePlayer()
	{
        StartGame();
    }

	public void _on_single_player_button_pressed()
	{
		playerTwo.IsAi = true;
        StartSinglePlayer();

    }

	public void _on_two_player_button_pressed()
	{
        playerTwo.IsPlayerOne = false;
        StartGame();
    }
}
