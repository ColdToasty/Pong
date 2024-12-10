using Godot;
using System;

public partial class AiPlayer : Node2D
{
	private Player owner; 

	public override void _Ready()
	{
		owner = this.GetParent<Player>();
	}

	private Vector2 ballPosition;
	private Vector2 ballMoveDirection;
	
	public void GiveBallPosition(double delta ,Vector2 ballPosition, Vector2 ballMoveDirection)
	{
		this.ballPosition = ballPosition;
		this.ballMoveDirection = ballMoveDirection;
		Move(delta);
    }


	private void Move(double delta)
	{
		Vector2 moveVector = Vector2.Zero;
		if(this.ballMoveDirection.X != 0)
		{
			if(this.ballMoveDirection.X == 1)
			{
				if(this.ballMoveDirection.Y < 0)
				{
					GD.Print("Gotta move up");
					moveVector = new Vector2(0, -1);

                }
				else if(this.ballMoveDirection.Y > 0)
				{
                    GD.Print("Gotta move Down");
                    moveVector = new Vector2(0, 1);
                }
				else
				{
                    GD.Print("Dont move");
                }



                moveVector = moveVector.Normalized() * (this.GetParent<Player>().MoveSpeed - this.GetParent<Player>().MoveSpeed / 3);

                this.GetParent<Player>().Velocity = moveVector * (float)delta;

                this.GetParent<Player>().MoveAndCollide(this.GetParent<Player>().Velocity);

            }	
		}
	}

}
