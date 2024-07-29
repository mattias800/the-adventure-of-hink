extends Node2D

@onready var particles_right = $ParticlesRight
@onready var particles_left = $ParticlesLeft

func _ready():
	particles_left.one_shot = true
	particles_right.one_shot = true
	particles_left.emitting = true
	particles_right.emitting = true
	await particles_left.finished
	queue_free()
