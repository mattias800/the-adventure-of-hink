extends CharacterBody2D

signal player_turned

var platform_controller: PlatformController
var enabled := false

func _ready():
	platform_controller = PlatformController.new(self, $AnimatedSprite2D, $JumpSound)
	
func _physics_process(delta):
	platform_controller.physics_process(delta)

func enable():
	enabled = true
	
func disable():
	enabled = false
