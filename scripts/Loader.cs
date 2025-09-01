using Godot;
using System;

public partial class Loader : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("rotate");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
