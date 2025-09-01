shader_type canvas_item;

// Basic water parameters
uniform float wave_speed : hint_range(0.1, 3.0) = 1.0;
uniform float wave_strength : hint_range(0.01, 0.1) = 0.02;
uniform vec4 water_color : source_color = vec4(0.2, 0.5, 0.8, 0.8);
uniform vec4 wave_color : source_color = vec4(0.6, 0.8, 1.0, 1.0);

// Simple noise function
float noise(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

// Simple wave function
float wave(vec2 uv, float time) {
    float wave1 = sin(uv.x * 4.0 + time * 2.0) * 0.5 + 0.5;
    float wave2 = sin(uv.y * 3.0 + time * 1.5) * 0.5 + 0.5;
    float wave3 = sin((uv.x + uv.y) * 2.0 + time * 2.5) * 0.5 + 0.5;
    
    return (wave1 + wave2 + wave3) / 3.0;
}

void fragment() {
    vec2 uv = UV;
    float time = TIME * wave_speed;
    
    // Create animated waves
    float waves = wave(uv, time);
    
    // Add some texture with noise
    float texture_noise = noise(uv * 20.0 + time * 0.1);
    waves = mix(waves, texture_noise, 0.3);
    
    // Mix colors based on wave height
    vec3 final_color = mix(water_color.rgb, wave_color.rgb, waves);
    
    // Add some brightness variation
    final_color *= 0.8 + waves * 0.4;
    
    COLOR = vec4(final_color, water_color.a);
}