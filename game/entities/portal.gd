extends Area2D

@export_file("*.tcsn") var next_scene_path

func _on_body_entered(body):
	print("Entered portal..")
