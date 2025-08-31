# Speedometer.gd
extends Control

@onready var speedometer_sprite = $SpeedometerSprite
@onready var fill_sprite = $FillSprite

@export var max_speed: float = 100.0
@export var current_speed: float = 0.0
@export var speed_change_rate: float = 50.0  # How fast the speedometer responds

var target_speed: float = 0.0

func _ready():
	# Make sure both sprites use the same texture initially
	if speedometer_sprite.texture:
		fill_sprite.texture = speedometer_sprite.texture
		
	# Set up the fill sprite material for masking
	setup_fill_material()

func _process(delta):
	# Smooth speed transitions
	current_speed = lerp(current_speed, target_speed, speed_change_rate * delta)
	update_speedometer()

func setup_fill_material():
	# Create a shader material for the fill effect
	var material = ShaderMaterial.new()
	var shader = Shader.new()
	
	# Shader code for radial fill
	shader.code = """
shader_type canvas_item;

uniform float fill_amount : hint_range(0.0, 1.0) = 0.0;
uniform vec4 fill_color : source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float start_angle : hint_range(0.0, 360.0) = 225.0; // Start angle in degrees
uniform float total_angle : hint_range(0.0, 360.0) = 270.0; // Total sweep angle

vec2 rotate_point(vec2 point, float angle) {
	float cos_a = cos(angle);
	float sin_a = sin(angle);
	return vec2(point.x * cos_a - point.y * sin_a, point.x * sin_a + point.y * cos_a);
}

void fragment() {
	vec2 center = vec2(0.5, 0.5);
	vec2 pos = UV - center;
	
	// Calculate angle from center
	float angle = atan(pos.y, pos.x);
	angle = angle + PI; // Convert to 0-2PI range
	angle = angle * 180.0 / PI; // Convert to degrees
	
	// Normalize angle to start from our start_angle
	float normalized_start = start_angle;
	if (normalized_start > 180.0) normalized_start -= 360.0;
	
	float relative_angle = angle - (normalized_start + 180.0);
	if (relative_angle < 0.0) relative_angle += 360.0;
	if (relative_angle > 360.0) relative_angle -= 360.0;
	
	// Check if this pixel should be filled
	float fill_angle = fill_amount * total_angle;
	
	if (relative_angle <= fill_angle) {
		COLOR = texture(TEXTURE, UV) * fill_color;
	} else {
		COLOR = vec4(0.0, 0.0, 0.0, 0.0); // Transparent
	}
	
	// Maintain original alpha
	COLOR.a *= texture(TEXTURE, UV).a;
}
"""
	
	material.shader = shader
	material.set_shader_parameter("fill_color", Color.RED)
	material.set_shader_parameter("start_angle", 225.0)  # Bottom-left start
	material.set_shader_parameter("total_angle", 270.0)  # 3/4 circle sweep
	
	fill_sprite.material = material

func update_speedometer():
	var fill_percentage = clamp(current_speed / max_speed, 0.0, 1.0)
	
	if fill_sprite.material is ShaderMaterial:
		fill_sprite.material.set_shader_parameter("fill_amount", fill_percentage)

# Call this function to update the target speed
func set_speed(new_speed: float):
	target_speed = clamp(new_speed, 0.0, max_speed)

# Example usage - connect this to your vehicle's speed
func _input(event):
	if event is InputEventKey and event.pressed:
		match event.keycode:
			KEY_UP:
				set_speed(current_speed + 10)
			KEY_DOWN:
				set_speed(current_speed - 10)
			KEY_R:
				set_speed(0)  # Reset

"""
SCENE SETUP INSTRUCTIONS:

1. Create a new Scene with Control as root node
2. Rename the Control node to "Speedometer"
3. Add the script above to the Speedometer node

4. Add child nodes:
   - Add Sprite2D as child, rename to "SpeedometerSprite"
   - Add another Sprite2D as child, rename to "FillSprite"

5. Set up the sprites:
   - Assign your curved speedometer texture to SpeedometerSprite
   - The FillSprite will automatically get the same texture via code
   - Position both sprites at the same location
   - Make sure FillSprite is above SpeedometerSprite in the scene tree

6. Adjust shader parameters if needed:
   - start_angle: Where the fill begins (225° = bottom-left)
   - total_angle: How much of the circle to fill (270° = 3/4 circle)
   - fill_color: The color of the fill (default: red)

7. Integration with your game:
   - Call speedometer.set_speed(your_vehicle_speed) from your vehicle script
   - The speedometer will smoothly animate to the new speed

ALTERNATIVE APPROACH (Simpler but less flexible):
If the shader approach is too complex, you can use TextureProgressBar:
1. Use TextureProgressBar instead of Control
2. Set your speedometer image as the "Under" texture
3. Create a red-tinted version as the "Progress" texture  
4. Set fill_mode to FILL_CLOCKWISE or FILL_COUNTER_CLOCKWISE
5. Update the 'value' property based on speed percentage
"""
