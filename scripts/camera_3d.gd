extends Camera3D

# Camera settings
@export var mouse_sensitivity: float = 0.002
@export var min_pitch: float = -80.0  # Minimum vertical angle (looking down)
@export var max_pitch: float = 80.0   # Maximum vertical angle (looking up)

# Camera pivot and vehicle references
var camera_pivot: Node3D
var vehicle: Node3D
var pitch: float = 0.0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Get reference to the existing camera pivot
	camera_pivot = get_parent()
	
	# Get reference to the vehicle (pivot's parent)
	vehicle = camera_pivot.get_parent()
	
	# Capture the mouse cursor
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	# The camera automatically follows its parent (pivot) transform
	# No explicit code needed for following - it's automatic in Godot
	
	# Handle mouse input for camera rotation
	if Input.is_action_just_pressed("ui_cancel"):  # ESC key to release mouse
		if Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		# Horizontal rotation (yaw) - unlimited 360 degrees
		camera_pivot.rotate_y(-event.relative.x * mouse_sensitivity)
		
		# Vertical rotation (pitch) - clamped between min and max
		pitch -= event.relative.y * mouse_sensitivity
		pitch = clamp(pitch, deg_to_rad(min_pitch), deg_to_rad(max_pitch))
		
		# Apply the clamped pitch rotation to the camera itself
		rotation.x = pitch
