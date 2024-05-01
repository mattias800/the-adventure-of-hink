extends Node
class_name Tracks

enum Track {
	EARLY_MORNING,
	HINK_THE_GAME,
	SOFT_BALL,
	WHISPERING_SHADOWS
}

func get_track_node(song: Track):
	match(song):
		Track.EARLY_MORNING:
			return $EarlyMorningMusic
		Track.HINK_THE_GAME:
			return $HinkTheGameMusic
		Track.SOFT_BALL:
			return $SoftBall
		Track.WHISPERING_SHADOWS:
			return $WhisperingShadows
