extends DirectionalLight3D

func _process(_delta):
	rotate(Vector3.RIGHT, deg_to_rad(0.1))
