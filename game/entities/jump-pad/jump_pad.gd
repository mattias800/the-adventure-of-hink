extends Node2D

@export var power: int = 400

@onready var animated_sprite = $AnimatedSprite2D
@onready var area = $Area2D

func _ready():
	animated_sprite.play("idle")
	
func _physics_process(delta):
	if area.overlaps_body(GameManager.player):
		fire(GameManager.player)
		GameManager.player.trigger_force(Vector2(0, -power))
		
func _on_area_2d_body_entered(body):
	pass
	#if body is PhysicsBody2D and body != GameManager.player:
	#	fire(body)

func fire(body: CharacterBody2D):
	animated_sprite.stop()
	animated_sprite.play("blam")
	animated_sprite.animation_finished.connect(after_fire_done)
	
func after_fire_done():
	animated_sprite.play("idle")
	animated_sprite.animation_finished.disconnect(after_fire_done)
	
