extends Node2D

@export var k := 0.015
@export var d := 0.03
@export var spread = 0.0002

var springs: Array[WaterSpring] = []
var passes := 8

# Spring instances
var distance_between_springs := 16
var number_of_springs := 32
var water_width := distance_between_springs * number_of_springs
@onready var water_spring := preload("res://src/features/water/water_spring.tscn")

# Body of water
var depth := 1000
var target_height = global_position.y
var bottom = target_height + depth
@onready var water_body := $WaterBody

# Surface
@export var border_thickness = 0.5
@onready var water_surface := $WaterSurface

func _ready():
	water_surface.width = border_thickness
	
	for i in range(number_of_springs):
		var x = i * distance_between_springs
		var w = water_spring.instantiate()
		
		add_child(w)
		springs.append(w)
		w.initialize(x)

	splash(2, 5)
	splash(5, 4)
	splash(12, 7)
	

func _process(delta):
	pass

func _physics_process(delta):
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
	var surface_points = []
	
	for i in range(springs.size()):
		surface_points.append(springs[i].position)
		
	var first_index = 0
	var last_index = surface_points.size() - 1
	
	var water_polygon_points = surface_points
	
	water_polygon_points.append(Vector2(surface_points[last_index].x, bottom))
	water_polygon_points.append(Vector2(surface_points[first_index].x, bottom))
	
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
