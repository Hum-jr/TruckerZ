using Godot;
using System;
using Truck.Choosing;



namespace Vehicle.Selector
{
    
    public partial class CarSelector : Node
    {

        private Button _next;
        private Button _previous;
        private Node _parent;
        private Button _select;
        
        [Export]
        private Node3D _carHolder;
        [Export]
        private PackedScene _truck1;
        [Export]
        private PackedScene _truck2;
        
        [Export]
        private PackedScene _playableTruck1;
        
        [Export]
        private PackedScene _playableTruck2;
        
        [Export]
        private PackedScene _gamescene;
        
        private Label _label;
       
        private Node _currentSceneInstance;
        private PackedScene _selectedTruck; // Track which truck is selected
       
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _next = GetNode<Button>("Control/Forward");
            _previous = GetNode<Button>("Control/Previous");
            _label = GetNode<Label>("Control/Label");
            _select = GetNode<Button>("Control/Select");
            
            // Connect button signals
            _next.Pressed += OnNextPressed;
            _previous.Pressed += OnPreviousPressed;
            _select.Pressed += OnSelectPressed;

            // Load initial scene and set as selected
            if (_truck1 != null)
            {
                _currentSceneInstance = _truck1.Instantiate();
                _carHolder.CallDeferred(Node.MethodName.AddChild, _currentSceneInstance);
                _selectedTruck = _playableTruck1; // Track the selected truck
                _label.Text = "BLUE TRUCK";

            }
        }

        private void OnNextPressed()
        {
            if (_truck2 != null)
            {
                SwitchToScene(_truck2);
                _selectedTruck = _playableTruck2; // Update selected truck
                _label.Text = "RED TRUCK";
            }
        }

        private void OnPreviousPressed()
        {
            if (_truck1 != null)
            {
                SwitchToScene(_truck1);
                _selectedTruck = _playableTruck1; // Update selected truck
                _label.Text = "BLUE TRUCK";
            }
        }

        private void OnSelectPressed()
        {
            if (_gamescene != null && _selectedTruck != null)
            {
                // Store the selected truck in a global singleton/autoload
                // You'll need to create this CarManager autoload first
                GlobalData.SelectedTruck = _selectedTruck;  

                // Change to the game scene
                GetTree().ChangeSceneToPacked(_gamescene);
            }
        }

        private void SwitchToScene(PackedScene newScene)
        {
            // Remove current scene
            if (_currentSceneInstance != null)
            {
                _currentSceneInstance.QueueFree();
            }

            // Add new scene to car holder
            _currentSceneInstance = newScene.Instantiate();
            _carHolder.CallDeferred(Node.MethodName.AddChild, _currentSceneInstance);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
    }
}