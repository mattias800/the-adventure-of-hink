extends Node

@onready var fog := %Fog
@onready var overworld_clouds := %OverworldClouds
@onready var background := %Background
@onready var background3d := %Background3D

func _ready():
	fog.visible = false
	overworld_clouds.visible = false
	
func set_fog_visible(s: bool) -> void:
	fog.visible = s
	
func set_overworld_clouds_visible(s: bool) -> void:
	overworld_clouds.visible = s

func set_background_enabled(s: bool) -> void:
	background3d.visible = s
	background.visible = s
