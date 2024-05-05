extends CharacterBody2D
class_name Player

signal player_turned

@onready var player_death_teleportation := $PlayerDeathTeleportation
@onready var animated_sprite := $AnimatedSprite2D
@onready var player_jump_sound := $PlayerJumpSound
@onready var player_land_sound := $PlayerLandSound
@onready var player_grab_wall_sound := $PlayerGrabWallSound
@onready var player_jump_from_wall_sound := $PlayerJumpFromWallSound
@onready var player_dash_sound := $PlayerDashSound
@onready var death_boom_sound := $DeathBoomSound
@onready var death_appear_sound := $DeathAppearSound
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
	platform_controller = PlatformController.new(self, animated_sprite, player_jump_sound, player_land_sound, player_dash_sound, player_grab_wall_sound, player_jump_from_wall_sound, $PlayerJumpFromAirSound)
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
	death_boom_sound.play()
	disable()
	collision_shape.disabled = true
	animated_sprite.visible = false

	player_death_teleportation.play_teleporting()

	var duration = global_position.distance_to(spawn_world_pos) / 200

	var tween = create_tween()
	tween.set_trans(Tween.TRANS_SINE)
	tween.tween_property(self, "global_position", spawn_world_pos, duration)
	tween.finished.connect(on_death_teleportation_done)

func on_death_teleportation_done():
	print("death tele done")
	death_appear_sound.play()
	player_death_teleportation.play_player_appearing()
	collision_shape.disabled = false
	animated_sprite.visible = true
	velocity = Vector2.ZERO
	enable()

func enable():
	enabled = true
	match active_controller:
		CharacterControllerType.PLATFORM:
			platform_controller.enable()
		CharacterControllerType.OVERWORLD:
			overworld_controller.enable()

func disable():
	enabled = false
	platform_controller.disable()
	overworld_controller.disable()
