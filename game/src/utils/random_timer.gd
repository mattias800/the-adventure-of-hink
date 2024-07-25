class_name RandomTimer

static func wait_random_time(tree: SceneTree, min_time: float, max_time: float) -> void:
	var random_time = randf_range(min_time, max_time)
	var timer = Timer.new()
	tree.root.add_child(timer)
	timer.wait_time = random_time
	timer.one_shot = true
	timer.start()
	await timer.timeout
	timer.queue_free()
