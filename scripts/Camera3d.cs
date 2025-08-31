using Godot;
using System;

public partial class Camera3d : Camera3D
{
    [Export] public float MouseSensitivity = 0.001f;
    [Export] public float JoystickSensitivity = 2.0f; // Joystick sensitivity
    [Export] public float MinVerticalAngle = -80.0f; // Degrees
    [Export] public float MaxVerticalAngle = 80.0f;  // Degrees
    [Export] public bool ClampHorizontal = false; // Set to true to limit horizontal rotation
    [Export] public float MaxHorizontalAngle = 180.0f; // Only used if ClampHorizontal is true
    [Export] public float OrbitDistance = 10.0f; // Distance from car
    [Export] public Vector3 CarOffset = Vector3.Zero; // Offset from car center
    
    private float _yaw = 0.0f;   // Horizontal rotation around car
    private float _pitch = 0.0f; // Vertical rotation
    private float _initialYaw = 0.0f; // Starting horizontal position
    private Node3D _carNode; // Reference to the car
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Capture mouse for look around
        Input.MouseMode = Input.MouseModeEnum.Captured;
        
        // Find the car node (VehicleBody3D parent or sibling)
        _carNode = GetParent() as Node3D;
        if (_carNode == null)
        {
            // Try to find VehicleBody3D in parent's children
            Node parent = GetParent();
            foreach (Node child in parent.GetChildren())
            {
                if (child is VehicleBody3D)
                {
                    _carNode = child as Node3D;
                    break;
                }
            }
        }
        
        if (_carNode == null)
        {
            GD.PrintErr("Camera3d: Could not find VehicleBody3D to orbit around!");
            return;
        }
        
        // Calculate initial angles based on current position relative to car
        Vector3 offset = GlobalPosition - (_carNode.GlobalPosition + CarOffset);
        _initialYaw = Mathf.Atan2(offset.X, offset.Z);
        _yaw = _initialYaw;
        _pitch = Mathf.Asin(offset.Y / offset.Length());
        
        // Set initial distance
        OrbitDistance = offset.Length();
        
        UpdateCameraPosition();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Toggle mouse capture with ESC
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Visible;
            else
                Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        
        // Handle joystick camera rotation
        HandleJoystickInput(delta);
        
        // Update camera position to follow car
        if (_carNode != null)
            UpdateCameraPosition();
    }
    
    private void HandleJoystickInput(double delta)
    {
        if (_carNode == null) return;
        
        // Get joystick input (right stick)
        Vector2 joystickInput = Vector2.Zero;
        
        // Try to use input actions first, fallback to direct joystick axis
        try
        {
            // Use these input actions (add them to Input Map):
            // look_left, look_right, look_up, look_down
            joystickInput.X = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
            joystickInput.Y = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");
        }
        catch
        {
            // Fallback: Use raw joystick axis if input actions don't exist
            joystickInput.X = Input.GetJoyAxis(0, JoyAxis.RightX);
            joystickInput.Y = Input.GetJoyAxis(0, JoyAxis.RightY);
        }
        
        if (joystickInput.Length() > 0.0f)
        {
            // Apply joystick movement to rotation
            _yaw -= joystickInput.X * JoystickSensitivity * (float)delta;
            _pitch -= joystickInput.Y * JoystickSensitivity * (float)delta;
            
            // Clamp vertical rotation (pitch) - always constrained
            _pitch = Mathf.Clamp(_pitch, 
                Mathf.DegToRad(MinVerticalAngle), 
                Mathf.DegToRad(MaxVerticalAngle));
            
            // Horizontal rotation (yaw) - allow 360° rotation by default
            if (ClampHorizontal)
            {
                // Optional: Clamp horizontal rotation relative to initial position
                float maxYawRad = Mathf.DegToRad(MaxHorizontalAngle / 2.0f);
                _yaw = Mathf.Clamp(_yaw, 
                    _initialYaw - maxYawRad, 
                    _initialYaw + maxYawRad);
            }
            // If not clamping horizontal, yaw can rotate freely 360°
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        // Only process mouse movement when captured
        if (Input.MouseMode != Input.MouseModeEnum.Captured || _carNode == null)
            return;
            
        if (@event is InputEventMouseMotion mouseEvent)
        {
            // Apply mouse movement to rotation
            _yaw -= mouseEvent.Relative.X * MouseSensitivity;
            _pitch -= mouseEvent.Relative.Y * MouseSensitivity;
            
            // Clamp vertical rotation (pitch) - always constrained
            _pitch = Mathf.Clamp(_pitch, 
                Mathf.DegToRad(MinVerticalAngle), 
                Mathf.DegToRad(MaxVerticalAngle));
            
            // Horizontal rotation (yaw) - allow 360° rotation by default
            if (ClampHorizontal)
            {
                // Optional: Clamp horizontal rotation relative to initial position
                float maxYawRad = Mathf.DegToRad(MaxHorizontalAngle / 2.0f);
                _yaw = Mathf.Clamp(_yaw, 
                    _initialYaw - maxYawRad, 
                    _initialYaw + maxYawRad);
            }
            // If not clamping horizontal, yaw can rotate freely 360°
            
            UpdateCameraPosition();
        }
        
        // Zoom in/out with mouse wheel
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                OrbitDistance = Mathf.Max(2.0f, OrbitDistance - 1.0f);
                UpdateCameraPosition();
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                OrbitDistance = Mathf.Min(50.0f, OrbitDistance + 1.0f);
                UpdateCameraPosition();
            }
        }
    }
    
    private void UpdateCameraPosition()
    {
        if (_carNode == null) return;
        
        // Calculate camera position in spherical coordinates around the car
        Vector3 carCenter = _carNode.GlobalPosition + CarOffset;
        
        Vector3 offset = new Vector3(
            Mathf.Sin(_yaw) * Mathf.Cos(_pitch),
            Mathf.Sin(_pitch),
            Mathf.Cos(_yaw) * Mathf.Cos(_pitch)
        ) * OrbitDistance;
        
        // Set camera position
        GlobalPosition = carCenter + offset;
        
        // Make camera look at the car
        LookAt(carCenter, Vector3.Up);
    }
    
    // Reset camera to initial position around car
    public void ResetCameraPosition()
    {
        _yaw = _initialYaw;
        _pitch = 0.0f;
        UpdateCameraPosition();
    }
    
    // Set specific orbit angle around car
    public void SetOrbitAngle(float horizontalDegrees, float verticalDegrees)
    {
        _yaw = Mathf.DegToRad(horizontalDegrees);
        _pitch = Mathf.DegToRad(verticalDegrees);
        
        // Always clamp vertical (pitch)
        _pitch = Mathf.Clamp(_pitch,
            Mathf.DegToRad(MinVerticalAngle),
            Mathf.DegToRad(MaxVerticalAngle));
            
        // Only clamp horizontal if enabled
        if (ClampHorizontal)
        {
            float maxYawRad = Mathf.DegToRad(MaxHorizontalAngle / 2.0f);
            _yaw = Mathf.Clamp(_yaw,
                _initialYaw - maxYawRad,
                _initialYaw + maxYawRad);
        }
        
        UpdateCameraPosition();
    }
    
    // Smoothly transition to a new orbit position
    public async void TransitionToAngle(float horizontalDegrees, float verticalDegrees, float duration = 1.0f)
    {
        float targetYaw = Mathf.DegToRad(horizontalDegrees);
        float targetPitch = Mathf.DegToRad(verticalDegrees);
        
        // Apply constraints to target - always clamp vertical
        targetPitch = Mathf.Clamp(targetPitch,
            Mathf.DegToRad(MinVerticalAngle),
            Mathf.DegToRad(MaxVerticalAngle));
            
        // Only clamp horizontal if enabled
        if (ClampHorizontal)
        {
            float maxYawRad = Mathf.DegToRad(MaxHorizontalAngle / 2.0f);
            targetYaw = Mathf.Clamp(targetYaw,
                _initialYaw - maxYawRad,
                _initialYaw + maxYawRad);
        }
        
        float startYaw = _yaw;
        float startPitch = _pitch;
        
        Tween tween = CreateTween();
        tween.TweenMethod(Callable.From<float>((t) => {
            _yaw = Mathf.Lerp(startYaw, targetYaw, t);
            _pitch = Mathf.Lerp(startPitch, targetPitch, t);
            UpdateCameraPosition();
        }), 0.0f, 1.0f, duration);
    }
}