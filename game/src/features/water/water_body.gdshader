shader_type canvas_item;

uniform vec4 tint : source_color = vec4(1.0);
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D noise1 : repeat_enable;


vec2 pixelate(vec2 ab, float amount) {
	return round(ab * amount) / amount;
}

void fragment() {
	vec2 offset = texture(noise1, SCREEN_UV + TIME * 0.1).rg / 200.0;
	vec4 screen_color = texture(SCREEN_TEXTURE, SCREEN_UV + offset);

	// vec2 uv = pixelate(globalposition / 32.0, 28);



	vec4 color = screen_color;

	color = mix(color, tint, 0.3);
	color = mix(vec4(0.5), color, 1.4);

	COLOR = color;
}
