extends Area2D

@export var k := 0.015
@export var d := 0.03
@export var spread = 0.0002

var springs: Array[WaterSpring] = []
var passes := 8

# Spring instances
@export var distance_between_springs := 8
@onready var water_spring := preload("res://src/features/water/water_spring.tscn")

# Body of water
@onready var water_body := $WaterBody

# Surface
@export var border_thickness = 1
@onready var water_surface := $WaterSurface
@onready var collision_shape := $CollisionShape2D

# Layout, it is read from collision shape.
var water_position := Vector2i(0, 0)
var water_size := Vector2i(0, 0)


# Random waves
var noise: FastNoiseLite
var noise_offset := 0.0

func _ready():
	noise = FastNoiseLite.new()
	noise.frequency = 1

	var rect = collision_shape.shape.get_rect()
	water_position = collision_shape.global_position + rect.position
	water_size = rect.size
	water_surface.width = border_thickness

	var num_springs = round(rect.size.x / distance_between_springs) + 1

	for i in range(num_springs):
		var spring_global_x = water_position.x + i * distance_between_springs
		var w = water_spring.instantiate()

		add_child(w)
		springs.append(w)
		w.initialize(Vector2i(spring_global_x, water_position.y), i)
		w.set_collision_width(distance_between_springs)
		w.splash.connect(splash)

func _process(delta):
	pass

func apply_noise_waves(delta: float):
	noise_offset += delta
	for i in range(springs.size()):
		var n = noise.get_noise_1d(noise_offset + i)
		springs[i].velocity += n * 0.01

func _physics_process(delta):
	apply_noise_waves(delta)

	for i in springs:
		i.water_update(k, d)

	var left_deltas = []
	var right_deltas = []

	for i in range(springs.size()):
		left_deltas.append(0)
		right_deltas.append(0)

	for j in range(passes):
		for i in range(springs.size()):
			if i > 0:
				left_deltas[i] = spread * (springs[i].height - springs[i - 1].height)
				springs[i - 1].velocity += left_deltas[i]

			if i < springs.size() - 1:
				right_deltas[i] = spread * (springs[i].height - springs[i + 1].height)
				springs[i + 1].velocity += right_deltas[i]
	new_border()
	draw_water_body()

func splash(index: int, speed: float):
	if (index >= 0 and index < springs.size()):
		springs[index].velocity += speed

func draw_water_body():
	var bottom = water_position.y + water_size.y - water_surface.global_position.y

	var curve = water_surface.curve
	var points = Array(curve.get_baked_points())

	var water_polygon_points = points

	var first_index = 0
	var last_index = water_polygon_points.size() - 1

	water_polygon_points.append(Vector2(water_polygon_points[last_index].x, bottom))
	water_polygon_points.append(Vector2(water_polygon_points[first_index].x, bottom))

	water_polygon_points = PackedVector2Array(water_polygon_points)

	water_body.polygon = water_polygon_points

func new_border():
	var curve := Curve2D.new().duplicate()

	var surface_points = []
	for i in range(springs.size()):
		surface_points.append(springs[i].position)

	for i in range(surface_points.size()):
		curve.add_point(surface_points[i])

	water_surface.curve = curve
	water_surface.smooth(true)
	water_surface.queue_redraw()
