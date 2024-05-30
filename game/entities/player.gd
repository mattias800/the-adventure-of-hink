extends CharacterBody2D
class_name Player

signal player_disabled
signal player_enabled
signal player_turned
signal player_dash_started(direction: Vector2)
signal player_dash_stopped
signal player_started_moving_on_ground
signal player_stopped_moving_on_ground

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
@onready var bounce_shape = $BounceShape/CollisionShape2D
@onready var squish_cast_right = $SquishCastRight
@onready var squish_cast_left = $SquishCastLeft
@onready var squish_cast_up = $SquishCastUp
@onready var squish_cast_down = $SquishCastDown
@onready var wall_ray_cast_left = $WallRayCastLeft
@onready var wall_ray_cast_right = $WallRayCastRight

var platform_controller: PlatformController
var overworld_controller: OverworldController

var enabled := false
var is_respawn_teleporting := false

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
	platform_controller.player_dash_started.connect(on_player_dash_started)
	platform_controller.player_dash_started.connect(func(direction): player_dash_started.emit(direction))
	platform_controller.player_dash_stopped.connect(func(): player_dash_stopped.emit())
	platform_controller.player_started_moving_on_ground.connect(func(): player_started_moving_on_ground.emit())
	platform_controller.player_stopped_moving_on_ground.connect(func(): player_stopped_moving_on_ground.emit())
	overworld_controller = OverworldController.new(self, animated_sprite)

func _physics_process(delta):
	if not enabled:
		return
		
	match state:
		PlayerState.ACTIVE:
			check_squish()
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

func on_player_dash_started(_direction: Vector2):
	$DashAnimation.play()

func death_teleport(spawn_world_pos: Vector2):
	if is_respawn_teleporting:
		return

	print("Player died, teleporting!")
	is_respawn_teleporting = true
	death_boom_sound.play()
	disable()
	turn_off_collisions()
	
	animated_sprite.visible = false

	player_death_teleportation.play_teleporting()

	var duration = global_position.distance_to(spawn_world_pos) / 200
	duration = clamp(duration, 1.0, 2.0)

	var fx := AudioServer.get_bus_effect(AudioServer.get_bus_index("Music"), 0) as AudioEffectLowPassFilter

	var tween = create_tween()
	tween.set_trans(Tween.TRANS_SINE)
	tween.set_parallel(true)
	tween.tween_property(self, "global_position", spawn_world_pos, duration)
	tween.tween_method(fx.set_cutoff, 0, 500, duration).set_trans(Tween.TRANS_CIRC)
	tween.tween_method(fx.set_resonance, 2.0, 0.5, duration).set_trans(Tween.TRANS_CIRC)
	tween.finished.connect(on_death_teleportation_done)



func on_death_teleportation_done():
	print("death tele done")
	is_respawn_teleporting = false
	var fx := AudioServer.get_bus_effect(AudioServer.get_bus_index("Music"), 0) as AudioEffectLowPassFilter
	fx.set_cutoff(20500)
	fx.set_resonance(0.5)
	death_appear_sound.play()
	player_death_teleportation.play_player_appearing()
	turn_on_collisions()
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
			
	player_enabled.emit()

func turn_off_collisions():
	collision_shape.disabled = true
	bounce_shape.disabled = true

func turn_on_collisions():
	collision_shape.disabled = false
	bounce_shape.disabled = false

func disable():
	enabled = false
	platform_controller.disable()
	overworld_controller.disable()
	player_disabled.emit()

func trigger_force(force: Vector2):
	match active_controller:
		CharacterControllerType.PLATFORM:
			platform_controller.trigger_force(force)
	
func on_hit_jump_source():
	match active_controller:
		CharacterControllerType.PLATFORM:
			platform_controller.on_hit_jump_source()

func check_squish():
	var squish_sides = squish_cast_left.is_colliding() and squish_cast_right.is_colliding()
	var squish_updown = squish_cast_up.is_colliding() and squish_cast_down.is_colliding()
	if squish_sides or squish_updown:
		await get_tree().create_timer(0.05).timeout
		GameManager.respawn_player()
	
