shader_type canvas_item;

// Water movement parameters
uniform float wave_speed : hint_range(0.1, 2.0) = 0.5;
uniform float wave_amplitude : hint_range(0.01, 0.2) = 0.05;
uniform float wave_frequency : hint_range(1.0, 10.0) = 4.0;

// Water colors
uniform vec4 deep_water_color : source_color = vec4(0.1, 0.3, 0.6, 1.0);
uniform vec4 shallow_water_color : source_color = vec4(0.3, 0.7, 0.9, 1.0);
uniform vec4 foam_color : source_color = vec4(0.9, 0.95, 1.0, 1.0);

// Pencil effect parameters
uniform float pencil_intensity : hint_range(0.0, 2.0) = 1.2;
uniform float pencil_scale : hint_range(0.5, 5.0) = 2.0;
uniform float edge_thickness : hint_range(0.001, 0.02) = 0.005;
uniform vec4 pencil_color : source_color = vec4(0.2, 0.2, 0.3, 1.0);

// Lighting
uniform float sun_intensity : hint_range(0.0, 2.0) = 1.0;
uniform vec2 sun_direction = vec2(0.5, -0.3);

// Hash function for noise
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// Smooth noise function
float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Fractal Brownian Motion for more complex noise
float fbm(vec2 p, int octaves) {
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;

    for (int i = 0; i < octaves; i++) {
        value += amplitude * noise(p * frequency);
        amplitude *= 0.5;
        frequency *= 2.0;
    }

    return value;
}

// Generate pencil texture
float pencil_texture(vec2 uv, float scale) {
    vec2 p = uv * scale;

    // Multiple noise layers for pencil grain
    float grain1 = fbm(p * 8.0, 3);
    float grain2 = fbm(p * 16.0 + vec2(100.0), 2) * 0.5;
    float grain3 = fbm(p * 32.0 + vec2(200.0), 2) * 0.25;

    // Combine grains with some directional bias for pencil strokes
    vec2 stroke_dir = vec2(0.7, 0.3);
    float stroke = sin(dot(p, stroke_dir) * 12.0) * 0.1;

    return (grain1 + grain2 + grain3 + stroke) * 0.5 + 0.5;
}

// Edge detection using sobel operator
float sobel_edge(sampler2D tex, vec2 uv, vec2 texel_size) {
    vec2 offset = texel_size;

    // Sample surrounding pixels
    float tl = texture(tex, uv + vec2(-offset.x, -offset.y)).r; // top left
    float tm = texture(tex, uv + vec2(0.0, -offset.y)).r;      // top middle
    float tr = texture(tex, uv + vec2(offset.x, -offset.y)).r; // top right
    float ml = texture(tex, uv + vec2(-offset.x, 0.0)).r;      // middle left
    float mr = texture(tex, uv + vec2(offset.x, 0.0)).r;       // middle right
    float bl = texture(tex, uv + vec2(-offset.x, offset.y)).r; // bottom left
    float bm = texture(tex, uv + vec2(0.0, offset.y)).r;       // bottom middle
    float br = texture(tex, uv + vec2(offset.x, offset.y)).r;  // bottom right

    // Sobel X
    float sobelX = (tr + 2.0 * mr + br) - (tl + 2.0 * ml + bl);

    // Sobel Y
    float sobelY = (bl + 2.0 * bm + br) - (tl + 2.0 * tm + tr);

    // Magnitude
    return sqrt(sobelX * sobelX + sobelY * sobelY);
}

void fragment() {
    vec2 uv = UV;
    float time = TIME * wave_speed;

    // Create water surface with multiple wave layers
    vec2 wave_uv1 = uv * wave_frequency + vec2(time * 0.8, time * 0.3);
    vec2 wave_uv2 = uv * wave_frequency * 1.7 + vec2(-time * 0.6, time * 0.7);
    vec2 wave_uv3 = uv * wave_frequency * 2.3 + vec2(time * 0.4, -time * 0.5);

    float wave1 = fbm(wave_uv1, 4) * 2.0 - 1.0;
    float wave2 = fbm(wave_uv2, 3) * 0.7;
    float wave3 = fbm(wave_uv3, 2) * 0.3;

    float wave_height = (wave1 + wave2 + wave3) * wave_amplitude;

    // Calculate water depth effect
    float depth = 1.0 - uv.y * 0.8; // Assuming deeper water towards bottom

    // Mix water colors based on depth and waves
    vec3 water_color = mix(deep_water_color.rgb, shallow_water_color.rgb,
                          depth + wave_height * 2.0);

    // Add foam at wave peaks
    float foam_mask = smoothstep(0.3, 0.7, wave_height + 0.5);
    water_color = mix(water_color, foam_color.rgb, foam_mask * 0.6);

    // Simple lighting calculation
    vec2 gradient = vec2(
        fbm(wave_uv1 + vec2(0.01, 0.0), 2) - fbm(wave_uv1 - vec2(0.01, 0.0), 2),
        fbm(wave_uv1 + vec2(0.0, 0.01), 2) - fbm(wave_uv1 - vec2(0.0, 0.01), 2)
    );

    float light = max(0.0, dot(normalize(gradient), normalize(sun_direction)));
    water_color += light * sun_intensity * 0.3;

    // Generate pencil texture
    float pencil = pencil_texture(uv + wave_height * 0.1, pencil_scale);

    // Apply pencil shading
    water_color *= mix(1.0, pencil, pencil_intensity * 0.5);

    // Create edge effect by detecting changes in wave height
    vec2 texel_size = vec2(edge_thickness);

    // Sample nearby wave heights for edge detection
    float h_center = wave_height;
    float h_right = fbm((uv + vec2(edge_thickness, 0.0)) * wave_frequency + vec2(time * 0.8, time * 0.3), 4) * wave_amplitude;
    float h_up = fbm((uv + vec2(0.0, edge_thickness)) * wave_frequency + vec2(time * 0.8, time * 0.3), 4) * wave_amplitude;

    // Calculate edge intensity
    vec2 edge_grad = vec2(h_right - h_center, h_up - h_center) / edge_thickness;
    float edge_intensity = length(edge_grad);

    // Add noise to edges for pencil effect
    float edge_noise = pencil_texture(uv * 20.0, 1.0);
    edge_intensity *= edge_noise;

    // Apply pencil edges
    float edge_mask = smoothstep(0.1, 0.8, edge_intensity);
    water_color = mix(water_color, pencil_color.rgb, edge_mask * pencil_intensity * 0.8);

    // Add some paper texture
    float paper_texture = fbm(uv * 100.0, 2) * 0.05;
    water_color += paper_texture;

    COLOR = vec4(water_color, 1.0);
}