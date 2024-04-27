shader_type canvas_item;
render_mode unshaded;

uniform float circle_size : hint_range(0.0, 1.05);
uniform float screen_width;
uniform float screen_height;
uniform vec2 focus_pos;

void fragment() {
	float ratio = screen_width / screen_height;
	vec2 target = vec2(focus_pos.x / screen_width, focus_pos.y / screen_height);
	float dist = distance(target, vec2(mix(0.5, UV.x, ratio), UV.y));
	COLOR.a = step(circle_size, dist);
}