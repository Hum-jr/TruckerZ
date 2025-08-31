extends VehicleBody3D

@export var MAX_STEER = 0.9
@export var ENGINE_POWER = 300

# Add reference to your speedometer - adjust the path to match your scene structure
@onready var speedometer = $"Speedometer" # If speedometer is child of car
# OR: @onready var speedometer = get_node("../UI/Speedometer")  # If speedometer is sibling
# OR: @onready var speedometer = $"../../UI/HUD/Speedometer"  # Adjust path as needed

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
	# Your existing car controls
	steering = move_toward(steering, Input.get_axis("steer_right","steer_left") * MAX_STEER, delta)
	engine_force = Input.get_axis("reverse","accelerate") * ENGINE_POWER
	
	# Update speedometer with current speed
	var current_speed = linear_velocity.length()
	
	# Optional: Convert to different units
	# For km/h: current_speed = current_speed * 3.6
	# For mph: current_speed = current_speed * 2.237
	
	if speedometer:
		speedometer.set_speed(current_speed)
