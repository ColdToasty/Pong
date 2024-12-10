using Godot;
using System;

public partial class Ball : CharacterBody2D
{
	[Export]
	private float moveSpeed = 600f;


	private bool moveLeft;

	public Vector2 MoveDirection;

	public int BallHeight;



	private float leftBorder;
	private float rightBorder;
	private float topBorder;
	private float bottomBorder;

	private Random random;
    [Signal]

	public delegate void ScoreIncreaseEventHandler(bool direction);

    public bool Start = false;

    public override void _Ready()
	{
		base._Ready();

		random = new Random();



		BallHeight = this.GetNode<Sprite2D>("Sprite2D").Texture.GetHeight();



		Reset();



    }

	public void SetBorders(Vector2 screenSize, Vector2 center)
	{
		 topBorder = center.Y - screenSize.Y / 2;

		bottomBorder = center.Y + screenSize.Y / 2;

		leftBorder = center.X - screenSize.X / 2;

		rightBorder = center.X + screenSize.X / 2;
	}

	public void Reset(bool specificDirection = false, bool moveLeft = false)
	{
		if (!specificDirection)
		{
            int moveLeftValue = random.Next(2);

            if (moveLeftValue == 0)
            {
                this.moveLeft = true;
            }
            else
            {
                this.moveLeft = false;
            }
        }
		else
		{
            this.moveLeft = moveLeft;
        }

        MoveDirection = Vector2.Zero;
    }

	public override void _PhysicsProcess(double delta)
	{
        if (Start)
        {
            if (moveLeft)
            {
                MoveDirection.X = -1;
            }

            else
            {
                MoveDirection.X = 1;
            }


            Velocity = MoveDirection * moveSpeed;

            var collision = MoveAndCollide(Velocity * (float)delta);
            if (collision != null)
            {
                Vector2 playerMoveDirection = ((Player)collision.GetCollider()).InputVector.Normalized();

                moveLeft = !moveLeft;

                if (playerMoveDirection.Y != 0)
                {
                    MoveDirection.Y = playerMoveDirection.Y * 0.5f;
                }

            }

            if (this.GlobalPosition.Y <= topBorder + BallHeight / 2)
            {
                this.GlobalPosition = new Vector2(this.GlobalPosition.X, topBorder + BallHeight / 2);
                MoveDirection.Y = -MoveDirection.Y;
            }

            if (this.GlobalPosition.Y >= bottomBorder - BallHeight / 2)
            {
                this.GlobalPosition = new Vector2(this.GlobalPosition.X, bottomBorder - BallHeight / 2);
                MoveDirection.Y = -MoveDirection.Y;
            }


            if (this.GlobalPosition.X <= leftBorder || this.GlobalPosition.X >= rightBorder)
            {
                EmitSignal(SignalName.ScoreIncrease, moveLeft);
            }
        }
      


	}
}
