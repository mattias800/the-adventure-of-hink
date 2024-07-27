extends Area2D
class_name Room

signal on_player_entered_room
signal on_player_exited_room

@onready var collision_shape = $CollisionShape2D

@export var enabled := true
var frames_left_initial_player_checked := 10

func _physics_process(delta):
	if frames_left_initial_player_checked > 0:
		frames_left_initial_player_checked -= 1
		if enabled and GameManager.player.room_detection.overlaps_area(self):
			on_player_entered_room.emit()
		
func _on_body_entered(body):
	if enabled and CollisionUtil.is_player(body):
		print("Player entered room: " + name)
		on_player_entered_room.emit()

func _on_body_exited(body):
	if enabled and CollisionUtil.is_player(body):
		on_player_exited_room.emit()

func _on_area_entered(area):
	# Checks against room trigger node on player.
	if enabled and CollisionUtil.is_player(area.get_parent()):
		print("Player entered room: " + name)
		on_player_entered_room.emit()

func _on_area_exited(area):
	# Checks against room trigger node on player.
	if enabled and CollisionUtil.is_player(area.get_parent()):
		on_player_exited_room.emit()
