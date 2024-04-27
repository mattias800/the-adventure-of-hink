extends Node
class_name Songs

func get_track_node(song: MusicManager.Track):
	match(song):
		MusicManager.Track.EARLY_MORNING:
			return $EarlyMorningMusic
		MusicManager.Track.HINK_THE_GAME:
			return $HinkTheGameMusic
