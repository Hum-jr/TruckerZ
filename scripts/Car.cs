using Godot;

public partial class Car : VehicleBody3D
{
    [Export] 
    public float MaxSteer = 0.9f;
    [Export] 
    public float EnginePower = 300f;

    private float currentSpeed;
    private AudioStreamPlayer audioStreamPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        audioStreamPlayer = GetNode<AudioStreamPlayer>("Run");
        audioStreamPlayer.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Update current speed
        currentSpeed = LinearVelocity.Length();
        
        // Vehicle controls
        var steerInput = Input.GetAxis("steer_right", "steer_left") * MaxSteer;
        Steering = (float)Mathf.MoveToward(Steering, steerInput, (float)delta);
        
        var accelerateInput = Input.GetAxis("reverse", "accelerate") * EnginePower;
        EngineForce = accelerateInput;
    
        // Audio control
        if (currentSpeed > 0.1 && !audioStreamPlayer.Playing)
        {
            audioStreamPlayer.Play();
        }

        // Node3D visibility control - only visible when accelerating
        bool isAccelerating = accelerateInput > 0;
        //GetNode<Node3D>("Node3D/Node3D2").Visible = isAccelerating;
    }
}