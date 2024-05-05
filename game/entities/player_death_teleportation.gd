extends Node2D

@onready var animated_sprite := $AnimatedSprite2D

enum {
	DISABLED,
	TELEPORTING,
	APPEARING
}

var state

func _ready():
	state = DISABLED
	
func _process(delta):
	match state:
		DISABLED:
			animated_sprite.visible = false
			return
		TELEPORTING:
			animated_sprite.play("teleporting")
		APPEARING:
			return
			
func play_teleporting():
	animated_sprite.visible = true
	animated_sprite.play("teleporting")
	state = TELEPORTING
	
func play_player_appearing():
	state = APPEARING
	animated_sprite.visible = true
	animated_sprite.play("player_appearing")
	animated_sprite.animation_finished.connect(func(): state = DISABLED)
