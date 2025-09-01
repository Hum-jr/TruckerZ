using Godot;

namespace Truckker.scripts;

public partial class GameManager : Node
{
    private TextureProgressBar _speedometer;
    private VehicleBody3D _truck;
    private Node2D _arrow;
    private Label _speedometerLabel;
    
    [Export] public float SpeedThreshold = 0.1f; // Minimum speed to register movement
    [Export] public float MaxSpeed = 100f; // Maximum speed for speedometer scale
    [Export] public float ArrowMinRotation = -111.7f; // Arrow position at 0 speed
    [Export] public float ArrowMaxRotation = 50f; // Arrow position at max speed
    
    private Vector2 _radialCenterOffset;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get references to nodes - adjust paths as needed
        _speedometer = GetNode<TextureProgressBar>("Overlays/Speedometer/TextureProgressBar");
        _truck = GetNode<VehicleBody3D>("truck/VehicleBody3D");
        _arrow = GetNode<Node2D>("Overlays/Arrow");
        _speedometerLabel = GetNode<Label>("Overlays/speed");
        
        // Set up speedometer properties
        if (_speedometer != null)
        {
            _radialCenterOffset = new Vector2(1523.0f, 1010.0f);
            _speedometer.RadialCenterOffset = _radialCenterOffset;
        }
        
        // Initialize arrow position
        if (_arrow != null)
        {
            _arrow.RotationDegrees = ArrowMinRotation;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_speedometer != null && _truck != null && _arrow != null)
        {
            // Get current speed from truck
            float currentSpeed = _truck.LinearVelocity.Length();
            
            // Apply threshold to filter out tiny movements from physics simulation
            if (currentSpeed < SpeedThreshold)
            {
                currentSpeed = 0f;
                
            }
            
            _speedometerLabel.Text = currentSpeed.ToString("0") + " KM/H";
            // Update speedometer progress bar
            _speedometer.Value = Mathf.Clamp(currentSpeed, _speedometer.MinValue, _speedometer.MaxValue);
            
            
            // Update arrow rotation based on speed
            UpdateArrowRotation(currentSpeed);
            
            // Debug output (remove these in final version)
            GD.Print($"Speed: {currentSpeed:F2}");
            GD.Print($"Speedometer Value: {_speedometer.Value:F2}");
            GD.Print($"Arrow Rotation: {_arrow.RotationDegrees:F2}");
        }
    }
    
    private void UpdateArrowRotation(float speed)
    {
        // Calculate arrow rotation based on speed percentage
        float speedPercentage = Mathf.Clamp(speed / MaxSpeed, 0f, 1f);
        
        // Interpolate between min and max rotation
        float targetRotation = Mathf.Lerp(ArrowMinRotation, ArrowMaxRotation, speedPercentage);
        
        // Smooth arrow movement (optional - remove if you want instant response)
        _arrow.RotationDegrees = Mathf.Lerp(_arrow.RotationDegrees, targetRotation, 0.1f);
        
        // OR for instant response, use:
        // arrow.RotationDegrees = targetRotation;
    }
}