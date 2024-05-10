extends Area2D
class_name Talkable

signal on_talk

@onready var speech_bubble = $SpeechBubble

var player_is_inside := false
var active := true

func _process(_delta):
	if player_is_inside and active:
		speech_bubble.scale = lerp(speech_bubble.scale, Vector2.ONE, 0.1)
	else:
		speech_bubble.scale = lerp(speech_bubble.scale, Vector2.ZERO, 0.1)
		
	if player_is_inside and Input.is_action_just_pressed("interact") and active:
		talk()
		
func talk():
	active = false
	on_talk.emit()

func activate():
	active = true

func _on_body_entered(body):
	if body.is_in_group("player"):
		player_is_inside = true


func _on_body_exited(body):
	if body.is_in_group("player"):
		player_is_inside = false
