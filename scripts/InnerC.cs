using Godot;
using System;

public partial class InnerC : Sprite2D 
{
    private bool pressed = false;

    [Export]
    public int MaxLength = 100;
        
    [Export]
    public int DeadZone = 5;

    private Node2D parent;

    public override void _Ready()
    {
        parent = GetParent<Node2D>();
    }

    public override void _Process(double delta)
    {
        if (pressed)
        {
            Vector2 mousePos = GetGlobalMousePosition();
            Vector2 parentPos = parent.GlobalPosition;
            float distanceToParent = mousePos.DistanceTo(parentPos);
                                  
            if (distanceToParent <= MaxLength)
            {
                GlobalPosition = mousePos;
            }
            else
            {
                var angle = parentPos.AngleToPoint(GetGlobalMousePosition());
                var newPosition = new Vector2(
                    parentPos.X + Mathf.Cos(angle) * MaxLength,
                    parentPos.Y + Mathf.Sin(angle) * MaxLength
                );
                GlobalPosition = newPosition;
            }
        }
        else
        {
            // Return to center when not pressed
            Vector2 targetPos = parent.GlobalPosition;
            Vector2 currentPos = GlobalPosition;
            float distanceToCenter = currentPos.DistanceTo(targetPos);
                     
            if (distanceToCenter > 1.0f) // Only move if not already at center
            {
                GlobalPosition = currentPos.Lerp(targetPos, 0.1f);
            }
        }
    }

    // Input event handling for touch/mouse
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    _on_button_button_down();
                }
                else
                {
                    _on_button_button_up();
                }
            }
        }
    }

    private void _on_button_button_down()
    {
        pressed = true;
    }

    private void _on_button_button_up()
    {
        pressed = false;
    }
        
    // Add method to check joystick output for debugging
    private Vector2 GetJoystickOutput()
    {
        Vector2 offset = GlobalPosition - parent.GlobalPosition;
        Vector2 normalizedOutput = offset / MaxLength;
                
        return normalizedOutput;
    }

    // Complete the calculateVector method
    private Vector2 CalculateVector()
    {
        Vector2 offset = GlobalPosition - parent.GlobalPosition;
        
        // Apply dead zone
        if (Mathf.Abs(offset.X) < DeadZone && Mathf.Abs(offset.Y) < DeadZone)
        {
            return Vector2.Zero;
        }
        
        // Normalize the vector based on MaxLength
        Vector2 normalizedVector = offset / MaxLength;
        
        // Clamp to ensure values stay within -1 to 1 range
        normalizedVector.X = Mathf.Clamp(normalizedVector.X, -1.0f, 1.0f);
        normalizedVector.Y = Mathf.Clamp(normalizedVector.Y, -1.0f, 1.0f);
        
        return normalizedVector;
    }
    
    // Optional: Get raw input values (useful for debugging)
    private Vector2 GetRawInput()
    {
        return GlobalPosition - parent.GlobalPosition;
    }
    
    // Optional: Check if joystick is currently being used
    private bool IsPressed()
    {
        return pressed;
    }
}