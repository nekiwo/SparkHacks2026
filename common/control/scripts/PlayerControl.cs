using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class PlayerControl : CharacterBody3D
{
	public bool Locked { get; set; } = false;

	[Export]
	public float Speed { get; set; } = 5;
	[Export]
	public float FallAcceleration { get; set; } = 10;
	[Export]
	public float JumpSpeed { get; set; } = 5;

	private Vector3 _targetVelocity = Vector3.Zero;

	private AnimatedSprite3D _sprite = null;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
		_sprite.Play("default");
	}

	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		

		// var direction = Vector3.Zero;

		// if (!Locked)
		// {
		// 	if (Input.IsActionPressed("move_right"))
		// 	{
		// 		direction.X += 1.0f;
		// 	}
		// 	if (Input.IsActionPressed("move_left"))
		// 	{
		// 		direction.X -= 1.0f;
		// 	}
		// 	if (Input.IsActionPressed("move_down"))
		// 	{
		// 		direction.Z += 1.0f;
		// 	}
		// 	if (Input.IsActionPressed("move_up"))
		// 	{
		// 		direction.Z -= 1.0f;
		// 	}
		// }

		// if (direction != Vector3.Zero)
		// {
		// 	direction = direction.Normalized();
		// }

		// _targetVelocity.X = direction.X * Speed;
		// _targetVelocity.Z = direction.Z * Speed;

		// bool onFloor = IsOnFloor();
		// if (onFloor)
		// {
		// 	if (Input.IsActionPressed("jump"))
		// 	{
		// 		_targetVelocity.Y = JumpSpeed;
		// 	}
		// }
		// else
		// {
		// 	_targetVelocity.Y -= FallAcceleration * (float)delta;
		// }

		// _targetVelocity = _targetVelocity.Rotated(Vector3.Up, Rotation.Y);
		// Velocity = _targetVelocity;

		// if (Math.Abs(Velocity.X) >= 0.1)
		// {
		// 	if (Velocity.X >= 0.1)
		// 	{
		// 		_sprite.Play("right");
		// 	} else if (Velocity.X <= -0.1)
		// 	{
		// 		_sprite.Play("left");
		// 	}
		// } else
		// {
		// 	_sprite.Play("default");
		// }
			
		// MoveAndSlide();
	}
}
