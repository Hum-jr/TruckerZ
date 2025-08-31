using Godot;

public partial class Car : VehicleBody3D
{
    [Export] 
    public float MaxSteer = 0.9f;
    [Export] 
    public float EnginePower = 300f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Any initialization code can go here
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        // Vehicle controls
        var steerInput = Input.GetAxis("steer_right", "steer_left") * MaxSteer;
        Steering = Mathf.MoveToward(Steering, steerInput, (float)delta);
        
        var accelerateInput = Input.GetAxis("reverse", "accelerate") * EnginePower;
        EngineForce = accelerateInput;
    }
}