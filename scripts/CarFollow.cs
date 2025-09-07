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

        public override void _Ready()
        {
            // If you didn't assign CarNode in inspector, try to find it
            if (CarNode != null) return;
            
            var cars = GetTree().GetNodesInGroup("trucks"); 
            foreach (var node in cars)
            {
                if (node is not VehicleBody3D vehicle) continue;
                CarNode = vehicle;
                break;
            }
        }

        public override void _Process(double delta)
        {
            if (CarNode == null) return;
            
            GlobalPosition = CarNode.GlobalPosition + new Vector3(0, 40, 0);

            if (!FollowCarRotation) return;
            var rotation = Rotation;
            rotation.Y = CarNode.Rotation.Y;
            Rotation = rotation;
        }
    }
}