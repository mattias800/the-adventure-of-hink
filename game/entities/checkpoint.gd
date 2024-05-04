extends Area2D
class_name Checkpoint

func _on_body_entered(body):
	if body.is_in_group("player"):
		print("Reached checkpoint.")
		GameManager.current_checkpoint = self
