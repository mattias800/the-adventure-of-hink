extends Node

signal cutscene_started
signal cutscene_ended

func start_timeline(timeline_name: String):
	cutscene_started.emit()
	Dialogic.start(timeline_name)
	await Dialogic.timeline_ended
	await get_tree().create_timer(0.25).timeout # Prevent last input to be sent to player.
	cutscene_ended.emit()
