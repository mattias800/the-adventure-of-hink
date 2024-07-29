shader_type canvas_item;

// Noise texture
uniform sampler2D noise_texture: repeat_enable, filter_nearest;
// Fog opacity
uniform float opacity: hint_range(0.0, 1.0) = 0.25;
// Fog density
uniform float density: hint_range(0.0, 1.0) = 0.25;
// Fog speed
uniform vec2 speed = vec2(0.02, 0.01);

void fragment() {
	// Make the fog move
	vec2 uv = UV + speed * TIME;
	float noise = texture(noise_texture, uv).r;
	// Convert range from 0.0 to 1.0, to -1.0 to 1.0
	float d = 1.0 - density;
	float fog = clamp(noise * (1.0 + d) - d, 0.0, 1.0);
	COLOR.a *= fog * opacity;
}