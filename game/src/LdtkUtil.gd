static func find_levels_parent(root: Window) -> Node2D:
	return root.get_node("Main").get_node("Hink");

static func find_level_node(root: Window, level_name: String) -> Node2D:
	return find_levels_parent(root).get_node(level_name);

static func find_all_level_nodes(root: Window) -> Array[Node]:
	return find_levels_parent(root).get_children();

static func find_entities_node_for_level(root: Window, level_name: String) -> Node2D:
	return find_level_node(root, level_name).get_node("Entities")

static func find_entities_node_for_level_node(level_node: Node2D) -> Node2D:
	return level_node.get_node("Entities")

static func find_entities_meta_for_level(root: Window, level_name: String) -> Array:
	return find_entities_node_for_level(root, level_name).get_meta("LDtk_entity_instances")

static func get_level_settings(level_node: Node2D):
	return level_node.get_meta("LDtk_level_fields")
	
static func find_tilemap_by_world_coordinates(root: Window, world_coordinates: Vector2) -> Array:
	for level in find_all_level_nodes(root):
		var tilemaps := level.get_children()
		for tilemap in tilemaps:
			if tilemap is TileMap and is_world_coordinate_within_tilemap(tilemap, world_coordinates):
				return [true, tilemap, level.name]

	return [false, null, null]

static func find_level_node_by_level_iid(root: Window, iid: String):
	print("find_level_node_by_level_iid")
	for n in find_all_level_nodes(root):
		var raw_data = n.get_meta("LDtk_raw_data")
		
		if raw_data["iid"] == iid:
			return n
		

static func find_entity_by_iid(root: Window, iid: String):
	print("find_entity_by_iid")
	for n in find_all_level_nodes(root):
		var entities = find_entities_node_for_level_node(n)
		
		var entity_instances = entities.get_meta("LDtk_entity_instances")
		for e in entity_instances:
			if e["iid"] == iid:
				return e
			

static func is_world_coordinate_within_tilemap(tilemap: TileMap, world_coordinate: Vector2) -> bool:
	return get_tilemap_world_rect(tilemap).has_point(world_coordinate)

static func get_tilemap_world_rect(tilemap: TileMap) -> Rect2:
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
			
