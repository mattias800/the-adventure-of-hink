extends Node2D

@export var tilemap: TileMap
@export var test_rect : ColorRect

func _ready():
	pass
	
func _enter_tree():
	add_nodes_for_each_tile()
	
func add_nodes_for_each_tile():
	var used_cells = tilemap.get_used_cells(0)
	var tile_size = tilemap.tile_set.tile_size
	print("creating death tiles: " + str(used_cells.size()))
	for c in used_cells:
		place_node_on_world_coordinates(get_world_coordinates(c, tile_size), tile_size)

func place_node_on_world_coordinates(coord: Vector2, tile_size: Vector2i):
	var area2d := Area2D.new()
	area2d.global_position = coord + Vector2(tile_size) / 2
	var collision_shape := CollisionShape2D.new()
	collision_shape.position = Vector2.ZERO
	var shape = RectangleShape2D.new()
	shape.size = tile_size
	collision_shape.shape = shape
	area2d.add_child(collision_shape)
	
	add_child(area2d)
	area2d.body_entered.connect(on_body_enter)

func on_body_enter(body):
	if body.is_in_group("player"):
		GameManager.respawn_player()
		
func get_world_coordinates(coord: Vector2i, tile_size: Vector2i) -> Vector2:
	var w = global_position
	w.x = w.x + coord.x * tile_size.x
	w.y = w.y + coord.y * tile_size.y
	return w
