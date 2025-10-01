using Godot;
using System;

public partial class Track : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = new Vector3(Position.X, 0, Position.Z);
	}
}
