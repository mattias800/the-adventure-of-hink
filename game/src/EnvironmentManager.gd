extends Node

@onready var fog := %Fog
@onready var overworld_clouds := %OverworldClouds
@onready var overworld_water := %OverworldWater

func _ready():
	fog.visible = false
	overworld_clouds.visible = false
	
func set_fog(s: bool) -> void:
	fog.visible = s
	
func set_overworld_clouds(s: bool) -> void:
	overworld_clouds.visible = s

func enable_overworld():
	set_fog(false)
	set_overworld_clouds(true)
	
func enable_platformer():
	set_fog(true)
	set_overworld_clouds(false)
