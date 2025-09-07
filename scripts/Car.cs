using Godot;

public partial class Car : VehicleBody3D
{
    [Export] 
    public float MaxSteer = 0.9f;
    [Export] 
    public float EnginePower = 300f;

    private float _currentSpeed;
    
    private AudioStreamPlayer _audioStreamPlayer;

    // Called when the node enters the scene tree for th B e first time.
    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("Run");
        _currentSpeed = LinearVelocity.Length();
        _audioStreamPlayer.Play();
        
    }

    public override void _PhysicsProcess(double delta)
    {
        
        // Vehicle controls
        var steerInput = Input.GetAxis("steer_right", "steer_left") * MaxSteer;
        Steering = (float)Mathf.MoveToward(Steering, steerInput, (float)delta);
        
        var accelerateInput = Input.GetAxis("reverse", "accelerate") * EnginePower;
        EngineForce = accelerateInput;
        GD.Print("Steering: " + Steering);
        GD.Print("Accelerate: " + accelerateInput);
    
        
        if (_currentSpeed > 0.1 && _audioStreamPlayer.Playing == false)
        {
            _audioStreamPlayer.Play();
            // _audioStreamPlayer.Play();
        }
    }
}