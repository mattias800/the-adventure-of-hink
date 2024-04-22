shader_type canvas_item;

uniform sampler2D noise1 : repeat_enable;
uniform sampler2D noise2 : repeat_enable;
uniform vec2 scroll1 = vec2(0.05, 0.05);
uniform vec2 scroll2 = vec2(-0.05, -0.05);
uniform vec4 tone_color : source_color;
uniform vec4 top_color : source_color;
uniform float light_start : hint_range(0.0, 1.0) = 0.275;
uniform float light_end : hint_range(0.0, 1.0) = 0.4;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float distortion_length : hint_range(-1, 1) = 0.2;


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float depth = texture(noise1, UV + scroll1 * TIME).r * texture(noise2, UV + scroll2 * TIME).r;
	
	vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV + distortion_length * vec2(depth));
	
	vec4 top_light = smoothstep(light_start, light_end, depth) * top_color;
	
	COLOR = screen_color * tone_color + top_light;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
