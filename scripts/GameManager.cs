using Godot;
using Godot.Collections;


public static class GlobalData
{
    public static PackedScene SelectedTruck { get; set; }
}
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    private PackedScene SelectedTruck { get; set; }
    private TextureProgressBar _speedometer;
    private VehicleBody3D _truck;
    private Node2D _arrow;
    private Label _speedometerLabel;
    
    
    [Export]
    public Array<PackedScene> SceneArray = new Array<PackedScene>();
    [Export] 
    public float SpeedThreshold = 0.1f; // Minimum speed to register movement
    [Export] 
    public float MaxSpeed = 100f; // Maximum speed for speedometer scale
    [Export] 
    public float ArrowMinRotation = -111.7f; // Arrow position at 0 speed
    [Export] 
    public float ArrowMaxRotation = 50f; // Arrow position at max speed
    
    private Vector2 _radialCenterOffset;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get references to nodes - adjust paths as needed
        Instance = this;
        _speedometer = GetNode<TextureProgressBar>("ScreenElements/Speedometer2/TextureProgressBar");
        //truck = GetNode<VehicleBody3D>("truck/VehicleBody3D");
        _arrow = GetNode<Node2D>("ScreenElements/Speedometer2/Arrow2");
        _speedometerLabel = GetNode<Label>("ScreenElements/Speedometer2/speed");
        
        // Load the selected truck from GlobalData
        LoadSelectedTruck();
        
        // Set up speedometer properties
        if (_speedometer != null)
        {
            _speedometer.RadialCenterOffset = _radialCenterOffset;
        }
        
        // Initialize arrow position
        if (_arrow != null)
        {
            _arrow.RotationDegrees = ArrowMinRotation;
        }
    }

    private void LoadSelectedTruck()
    {
        if (GlobalData.SelectedTruck != null)
        {
            // Instantiate the selected truck
            var truckInstance = GlobalData.SelectedTruck.Instantiate();
            truckInstance.AddToGroup("trucks");
            
            // Find where to spawn the truck
            var truckSpawnPoint = GetNodeOrNull<Node3D>("TruckSpawn");
            if (truckSpawnPoint != null)
            {
                truckSpawnPoint.AddChild(truckInstance);
            }
            else
            {
                // If no specific spawn point, add to scene root
                AddChild(truckInstance);
            }
            
            // Update truck reference if the spawned truck is a VehicleBody3D
            if (truckInstance is VehicleBody3D vehicleBody)
            {
                _truck = vehicleBody;
                GD.Print("Truck is root node (VehicleBody3D)");
            }
            else
            {
                // Search for VehicleBody3D in the truck's children
                _truck = FindVehicleBodyInNode(truckInstance);
    
                if (_truck != null)
                {
                    GD.Print($"Found VehicleBody3D in truck children: {_truck.Name}");
                }
                else
                {
                    GD.PrintErr("No VehicleBody3D found in truck scene!");
                }
            }
            
            GD.Print($"Loaded selected truck: {truckInstance.Name}");
        }
        else
        {
            GD.Print("No truck selected in GlobalData");
        }
    }
    
    private VehicleBody3D FindVehicleBodyInNode(Node node)
    {
        GD.Print($"Searching in node: {node.Name} (Type: {node.GetType().Name})");
    
        // Check if current node is VehicleBody3D
        if (node is VehicleBody3D vehicleBody)
        {
            return vehicleBody;
        }
    
        // Recursively search all children
        foreach (Node child in node.GetChildren())
        {
            VehicleBody3D result = FindVehicleBodyInNode(child);
            if (result != null)
            {
                return result;
            }
        }
    
        return null; // Not found
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

    private void VehicleChooser(int index)
    {
        if (index >= 0 && index < SceneArray.Count)
        {
            var vehicle = SceneArray[index];
            GlobalData.SelectedTruck = vehicle;
            GD.Print($"Vehicle selected at index {index}");
        }
        else
        {
            GD.PrintErr($"Invalid vehicle index: {index}");
        }
    }
    
    
}