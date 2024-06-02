shader_type canvas_item;

// Noise texture
uniform sampler2D noise_texture: repeat_enable, filter_nearest;
// Fog opacity
uniform float opacity: hint_range(0.0, 1.0) = 0.25;
// Fog density
uniform float density: hint_range(0.0, 1.0) = 0.25;
// Fog speed
uniform vec2 speed = vec2(0.001, 0.002);
uniform vec2 offset = vec2(0.0, 0.0);

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	// Make the fog move
	vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV);

	vec2 uv = UV + offset + speed * TIME;
	float noise = texture(noise_texture, uv * 0.5).r;
	// Convert range from 0.0 to 1.0, to -1.0 to 1.0
	float d = 1.0 - density;
	float fog = clamp(noise * (1.0 + d) - d, 0.0, 1.0);

	COLOR.a *= fog * opacity;
	// COLOR = screen_color - (fog * opacity);
}