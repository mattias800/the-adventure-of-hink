extends CharacterBody2D

signal player_turned

var platform_controller: PlatformController
var overworld_controller: OverworldController

var enabled := false

enum {
	PLATFORM,
	OVERWORLD
}

var active_controller := PLATFORM

func _ready():
	platform_controller = PlatformController.new(self, $AnimatedSprite2D, $JumpSound)
	overworld_controller = OverworldController.new(self, $AnimatedSprite2D)
	
func _physics_process(delta):
	if not enabled:
		return
	match active_controller:
		PLATFORM:
			platform_controller.physics_process(delta)
		OVERWORLD:
			overworld_controller.physics_process(delta)

func switch_to_platform():
	active_controller = PLATFORM

func switch_to_overworld():
	active_controller = OVERWORLD
	
func enable():
	enabled = true
	match active_controller:
		PLATFORM:
			platform_controller.enable()
		OVERWORLD:
			overworld_controller.enable()
	
func disable():
	enabled = false
	platform_controller.disable()
	overworld_controller.disable()
