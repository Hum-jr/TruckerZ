// Attach this script to your minimap Camera3D node
using Godot;

public partial class CarFollow : Camera3D
{
    [Export] 
    public VehicleBody3D CarNode { get; set; }  // Drag your car node here in the inspector
    [Export] 
    public bool FollowCarRotation { get; set; } = true;

  
    // Whether minimap rotates with car

    public override void _Ready()
    {
        // If you didn't assign CarNode in inspector, try to find it
        if (CarNode == null)
        {
            CarNode = GetNode<VehicleBody3D>("..");  // Adjust path as needed
        }
    }

    public override void _Process(double delta)
    {
        if (CarNode != null)
        {
            // Follow the car's position but stay at fixed height
            GlobalPosition = CarNode.GlobalPosition + new Vector3(0, 40, 0);
            
            
            // Optional: Rotate minimap to match car's direction
            if (FollowCarRotation)
            {
                var rotation = Rotation;
                rotation.Y = CarNode.Rotation.Y;
                Rotation = rotation;
            }
        }
    }
}