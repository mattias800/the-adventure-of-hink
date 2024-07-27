extends Node2D

@onready var blue_light = $BlueLight
@onready var sparkles = $Sparkles

var running := false
var stopping := false

var run_counter := 0.0

var counter := 1.0

func _ready():
	blue_light.visible = false
	sparkles.visible = false
	running = false
	start()

func start():
	if running:
		return

	running = true
	sparkles.play("default")
	sparkles.visible = true

	await get_tree().create_timer(0.2).timeout
	
	blue_light.visible = true
	blue_light.scale = Vector2(0, 0)
	blue_light.play("default")

	var tween = create_tween()
	tween.set_trans(Tween.TRANS_SINE)
	tween.tween_property(blue_light, "scale", Vector2(0.25, 0.25), 0.3)

func stop():
	if stopping:
		return
	stopping = true
	var tween = create_tween()
	tween.set_trans(Tween.TRANS_SINE)
	tween.tween_property(blue_light, "scale", Vector2(0, 0), 0.3)
	await tween.finished
	queue_free()
