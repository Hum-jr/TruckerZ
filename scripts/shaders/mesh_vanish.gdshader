// ===== GODOT 4.x VERSION =====
shader_type spatial;
render_mode depth_draw_opaque, depth_test_disabled, shadows_disabled;

// Basic dissolve parameters
uniform float dissolve_amount : hint_range(0.0, 1.0) = 0.0;
uniform sampler2D noise_texture : source_color;