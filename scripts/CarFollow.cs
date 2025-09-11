// Attach this script to your minimap Camera3D node
using Godot;

namespace car.follow
{
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
                var truck = GetTree().GetNodesInGroup("trucks");
                
                GD.Print($"Found {truck.Count} nodes in trucks");
                foreach (var node in truck)
                {
                    if (node is not VehicleBody3D vehicle) continue;
                    CarNode = vehicle;
                    break; // Found it, stop searching
                }
            }
            
        }

        public override void _Process(double delta)
        {
            if (CarNode == null) return;
            // Follow the car's position but stay at fixed height
            GlobalPosition = CarNode.GlobalPosition + new Vector3(0, 40, 0);
            
            
            // Optional: Rotate minimap to match car's direction
            if (!FollowCarRotation) return;
            var rotation = Rotation;
            rotation.Y = CarNode.Rotation.Y;
            Rotation = rotation;
        }
    }
}
