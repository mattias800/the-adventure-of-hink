extends Node

var camera: GameCamera

func _process(delta):
	var room = get_room_containing_player()
	if room != null:
		CameraLimiter.apply_collision_shape_to_camera_limits(camera, room.collision_shape)
	else:
		print("no room")
		camera.clear_camera_limits()
	
func connect_to_tilemap(next_tilemap: TileMap) -> void:
	camera.connect_to_tilemap(next_tilemap)

func set_camera_target(target: Node2D):
	camera.set_camera_target(target)

func set_camera(c):
	camera = c
	
func get_room_containing_player():
	var rooms = get_tree().get_nodes_in_group("rooms")
	for room in rooms:
		if room.collision_shape == null:
			print("Missing collision shape on room node.")
			continue

		if room.enabled and GameManager.player.room_detection.overlaps_area(room):
			return room
