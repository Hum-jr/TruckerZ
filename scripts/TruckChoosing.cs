using System;
using Godot;


namespace Truck.Choosing
{
	public partial class TruckChoosing : Control
	{
		private Node parent;
	
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			parent = (Node)GetParent();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			
		}
		
	}
}

