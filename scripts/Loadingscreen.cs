using Godot;
using System;

public partial class Loadingscreen : Control
{

	private TextureProgressBar _loadingProgressBar;
	private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_loadingProgressBar = GetNode<TextureProgressBar>("ColorRect/HBoxContainer/MarginContainer2/TextureProgressBar");
		_timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_timer.IsStopped())
		{
			double timeLeft = _timer.TimeLeft;        // Time remaining
			double waitTime = _timer.WaitTime;        // Total wait time
			double elapsed = waitTime - timeLeft;     // Time elapsed
			double progress = elapsed / waitTime;
			_loadingProgressBar.Value += progress;
		}
	}

	private void _on_timer_timeout()
	{
		GetTree().ChangeSceneToFile("res://scenes/CarSelector.tscn");

	}
}
