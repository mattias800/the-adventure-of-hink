static func find_level_tilemap_by_world_coordinates(tree: SceneTree, world_coordinates: Vector2):
	for tilemap in tree.get_nodes_in_group("rooms"):
		if tilemap is TileMap and is_world_coordinate_within_tilemap(tilemap, world_coordinates):
			return tilemap
	for tilemap in tree.get_nodes_in_group("overworlds"):
		if tilemap is TileMap and is_world_coordinate_within_tilemap(tilemap, world_coordinates):
			return tilemap

static func is_world_coordinate_within_tilemap(tilemap: TileMap, world_coordinate: Vector2) -> bool:
	return get_tilemap_global_rect(tilemap).has_point(world_coordinate)

static func get_tilemap_world_rect_old(tilemap: TileMap) -> Rect2:
	var tile_size    := tilemap.get_tileset().tile_size
	var tilemap_size := get_tilemap_size(tilemap)
	var level_size   := Vector2i(tile_size * tilemap_size)
	var level_offset_x: float =  tilemap.get_parent().position.x
	var level_offset_y: float = tilemap.get_parent().position.y

	return Rect2(level_offset_x, level_offset_y, level_size.x, level_size.y)

static func get_tilemap_size(tilemap: TileMap) -> Vector2i:
	var tilemap_rect := tilemap.get_used_rect()
	return Vector2i(
		tilemap_rect.end.x - tilemap_rect.position.x,
		tilemap_rect.end.y - tilemap_rect.position.y
	)

static func get_vector_from_direction(direction: String) -> Vector2:
	match direction:
		"Left":
			return Vector2(-24, 0)
		"Right":
			return Vector2(24, 0)
		"Up":
			return Vector2(0, -24)
		"Down":
			return Vector2(0, 24)
	print("Invalid direction: ", direction)
	return Vector2(0, 0)
			
static func get_tilemap_global_rect(tilemap: TileMap) -> Rect2:
	var rect := Rect2(tilemap.get_used_rect())
	var tile_size           := tilemap.get_tileset().tile_size
	rect.position *= Vector2(tile_size)
	rect.position += tilemap.global_position
	rect.size *= Vector2(tile_size)
	return rect
