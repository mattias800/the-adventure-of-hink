extends DirectionalLight3D

func _process(delta):
	rotate(Vector3.RIGHT, deg_to_rad(0.1))
