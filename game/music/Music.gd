extends AudioStreamPlayer2D

enum Song {
	EARLY_MORNING,
	HINK_THE_GAME
}

var songs = {
	"early_morning": Song.EARLY_MORNING,
	"hink_the_game": Song.HINK_THE_GAME
}
var currently_playing_node

func play_music(song: Song):
	if (currently_playing_node):
		currently_playing_node.stop()
	
	play_music_node(get_music_node(song))

func get_music_node(song: Song):
	match(song):
		Song.EARLY_MORNING:
			return $EarlyMorningMusic
		Song.HINK_THE_GAME:
			return $HinkTheGameMusic
	
func play_music_node(node):
	node.play()
	currently_playing_node = node
