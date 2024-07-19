extends Node2D

@export var player_fly_area : Area2D

@export var fly_away_start_speed := 40.0
@export var fly_max_speed := 100

@onready var animated_sprite_2d = $AnimatedSprite2D

var current_speed := 40.0

enum BirdState {
	IDLE,
	FLYING_AWAY
}

@export var state : BirdState = BirdState.IDLE
@export var flying_direction := Vector2(1, -1)

var distance_flown := 0.0

func _ready():
	current_speed = fly_away_start_speed
	animated_sprite_2d.play("idle")
	if player_fly_area != null:
		player_fly_area.body_entered.connect(fly_away_if_player)

func _process(delta):
	match state:
		BirdState.IDLE:
			animated_sprite_2d.play("idle")
		BirdState.FLYING_AWAY:
			animated_sprite_2d.play("fly")


func _physics_process(delta):
	match state:
		BirdState.IDLE:
			return
		BirdState.FLYING_AWAY:
			current_speed += 1
			if current_speed > fly_max_speed:
				current_speed = fly_max_speed

			var d = flying_direction * current_speed * delta
			global_position += d
			distance_flown += d.length()

			var distance_to_camera = (CameraManager.camera.global_position - global_position).abs()
			if distance_to_camera.x > 400 or distance_to_camera.y > 200:
				queue_free()

func fly_away_if_player(body):
	print("Player approached bird")
	if CollisionUtil.is_player(body):
		fly_away()

func fly_away():
	state = BirdState.FLYING_AWAY
