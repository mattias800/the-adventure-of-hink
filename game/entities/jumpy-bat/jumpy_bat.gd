extends Area2D
class_name JumpyBat

@onready var animated_sprite_2d = $AnimatedSprite2D

var active := true

func _ready():
	animated_sprite_2d.play("idle")

func _on_body_shape_entered(body_rid, body, body_shape_index, local_shape_index):
	if body.is_in_group("player") and active:
		GameManager.respawn_player()


func _on_area_shape_entered(area_rid, area, area_shape_index, local_shape_index):
	if area.is_in_group("player_bounce") and active:
		if area.global_position.y > global_position.y:
			GameManager.respawn_player()
		else:
			GameManager.player.on_hit_jump_source()
			active = false
			await get_tree().create_timer(0.1).timeout
			active = true
	
