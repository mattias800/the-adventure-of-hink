extends Node2D

@export var is_open := false

@onready var collision_shape_2d :CollisionShape2D = $OpeningBody/CollisionShape2D

signal player_entered_well

# Called when the node enters the scene tree for the first time.
func _ready():
	if is_open:
		open()
	else:
		close()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func open():
	is_open = true
	collision_shape_2d.disabled = true

func close():
	is_open = false
	collision_shape_2d.disabled = false


func _on_area_2d_body_entered(body):
	if CollisionUtil.is_player(body):
		player_entered_well.emit()
		
