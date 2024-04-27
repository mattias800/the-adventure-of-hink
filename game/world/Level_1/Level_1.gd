extends Node

var has_entered_map := false

@onready var level_manager := %LevelManager
@onready var cutscene_manager := %CutsceneManager
@onready var music_manager := %MusicManager

func on_player_enter_map():
	if not has_entered_map:
		cutscene_manager.start_timeline("intro")
		await Dialogic.timeline_ended
		music_manager.play_track(MusicManager.Track.HINK_THE_GAME)
		
	has_entered_map = true
