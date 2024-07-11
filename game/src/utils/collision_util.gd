class_name CollisionUtil

static func bodies_contain_player(bodies: Array[Node2D]) -> bool:
	return bodies.any(is_player)

static func bodies_except_player(bodies: Array[Node2D]) -> Array[Node2D]:
	return bodies.filter(func (body: Node2D): return !is_player(body))

static func bodies_contain_only_player(bodies: Array[Node2D]) -> bool:
	return bodies.size() == 1 and is_player(bodies[0])

static func is_player(body: Node2D) -> bool:
	return body.is_in_group("player")
