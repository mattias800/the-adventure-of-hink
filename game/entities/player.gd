extends CharacterBody2D
class_name Player

signal player_turned

@onready var player_death_teleportation := $PlayerDeathTeleportation
@onready var animated_sprite := $AnimatedSprite2D
@onready var jump_sound := $JumpSound
@onready var collision_shape := $CollisionShape2D

var platform_controller: PlatformController
var overworld_controller: OverworldController

var enabled := false

enum CharacterControllerType {
	PLATFORM,
	OVERWORLD
}

enum PlayerState {
	ACTIVE,
	DEATH_TELEPORTATION
}

var active_controller := CharacterControllerType.PLATFORM
var state := PlayerState.ACTIVE

func _ready():
	player_death_teleportation.visible = false
	platform_controller = PlatformController.new(self, animated_sprite, jump_sound)
	platform_controller.player_turned.connect(func(direction): player_turned.emit(direction))
	overworld_controller = OverworldController.new(self, animated_sprite)

func _physics_process(delta):
	if not enabled:
		return
	match state:
		PlayerState.ACTIVE:
			match active_controller:
				CharacterControllerType.PLATFORM:
					platform_controller.physics_process(delta)
				CharacterControllerType.OVERWORLD:
					overworld_controller.physics_process(delta)
		PlayerState.DEATH_TELEPORTATION:
			pass

func switch_to_platform():
	active_controller = CharacterControllerType.PLATFORM

func switch_to_overworld():
	active_controller = CharacterControllerType.OVERWORLD

func death_teleport(spawn_world_pos: Vector2):
	player_death_teleportation.play_teleporting()
	disable()
	collision_shape.disabled = true
	# animated_sprite.visible = false
	var duration = global_position.distance_to(spawn_world_pos) / 20
	
	var tween = create_tween()
	tween.set_process_mode(Tween.TWEEN_PROCESS_PHYSICS)
	tween.set_parallel(false)
	tween.set_trans(Tween.TRANS_SINE)
	tween.set_loops()
	tween.tween_property(self, "global_position", spawn_world_pos, duration)
	tween.finished.connect(on_death_teleportation_done)

func on_death_teleportation_done():
	player_death_teleportation.play_player_appearing()
	collision_shape.disabled = false
	animated_sprite.visible = true
	enable()
	
func enable():
	enabled = true
	collision_shape.disabled = true
	match active_controller:
		CharacterControllerType.PLATFORM:
			platform_controller.enable()
		CharacterControllerType.OVERWORLD:
			overworld_controller.enable()

func disable():
	enabled = false
	collision_shape.disabled = false
	platform_controller.disable()
	overworld_controller.disable()
