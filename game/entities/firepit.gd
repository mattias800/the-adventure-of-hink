extends Node2D

@export var light_strength := 2.0
@onready var noise := FastNoiseLite.new()
@onready var light := $PointLight2D

var value := 0
const MAX_VALUE = 1000000

func _ready():
	randomize()
	value = randi() % MAX_VALUE
	
func _physics_process(delta):
	value += 1
	value = value % MAX_VALUE
	
	var energy = 2 + noise.get_noise_1d(value) * light_strength
	light.energy = energy
