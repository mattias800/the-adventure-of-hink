extends Node2D

signal cutscene_started
signal cutscene_ended

@onready var m : AudioStreamPlayer2D = %Music

func _ready():
	Dialogic.timeline_ended.connect(_on_timeline_ended)
	cutscene_started.emit()
	m.stream = load_mp3("res://assets/audio/music/Early morning guitar.mp3")
	m.play()
	print(m)
	Dialogic.start("intro")

func _on_timeline_ended():
	Dialogic.timeline_ended.disconnect(_on_timeline_ended)
	m.stream = load_mp3("res://assets/audio/music/Soft ball.mp3")
	m.play()
	cutscene_ended.emit()

func load_mp3(path) -> AudioStreamMP3:
	var file  := FileAccess.open(path, FileAccess.READ)
	var sound := AudioStreamMP3.new()
	sound.data = file.get_buffer(file.get_length())
	return sound
