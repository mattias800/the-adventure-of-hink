extends Node2D

@onready var room = $Rooms/Room
@onready var room_2 = $Rooms/Room2
@onready var room_1_underground = $Rooms/Room1Underground
@onready var room_2_underground = $Rooms/Room2Underground
@onready var ground_over_well_hole = $GroundOverWellHole
@onready var attacking_state = $AttackingState

func _ready():
	MusicManager.PlayTrack(Tracks.Track.SOFT_BALL)
	if not GameState.state.levels.home_town.is_under_attack:
		attacking_state.queue_free()

func _on_room_switch_trigger_body_entered(body):
	if CollisionUtil.is_player(body):
		room.enabled = false
		room_2.enabled = false
		room_1_underground.enabled = true
		room_2_underground.enabled = true

func _on_overground_camera_trigger_body_entered(body):
	if CollisionUtil.is_player(body):
		room.enabled = true
		room_2.enabled = true
		room_1_underground.enabled = false
		room_2_underground.enabled = false


func _on_well_player_entered_well():
	ground_over_well_hole.visible = false
