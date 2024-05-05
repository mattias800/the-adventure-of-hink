extends Area2D

@export_file("*.tscn") var next_scene_path
@export var target_portal_name: String
@export var spawn_point: Marker2D

signal player_entered_portal

func _on_body_entered(body):
	if body.is_in_group("player") and next_scene_path:
		print("Entered portal: " + name)
		GameManager.on_player_entered_portal(self)
		player_entered_portal.emit(self)
