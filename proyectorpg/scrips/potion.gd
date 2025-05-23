extends Node2D

@onready var _potion_sprite = $Sprite2D  # El sprite de la poción
@onready var _selected_sprite = $Selected  # El sprite de selección
@onready var _tecla_sprite = $Tecla  # El sprite de la tecla E
@onready var _area = $Area2D  # El área de detección
@onready var _character_stats = get_node("/root/CharacterStats")

var _player_inside = false
var _player_body = null
var _potion_consumed = false  # Para evitar que se vuelva a consumir

# Cantidad de vida que cura la poción
const HEAL_AMOUNT = 20

func _ready():
	# Ocultar todos los sprites de UI inicialmente
	_selected_sprite.visible = false
	_tecla_sprite.visible = false
	
	# Conectar las señales del Area2D
	_area.body_entered.connect(_on_body_entered)
	_area.body_exited.connect(_on_body_exited)
	
	print("Potion: Script iniciado correctamente")

func _process(delta):
	# Debug y detección de input
	if _player_inside and not _potion_consumed:
		# Debug
		# print("Potion: Jugador dentro, esperando input...")
		
		# Múltiples formas de detectar el input
		if Input.is_action_just_pressed("ui_accept"):
			print("Potion: ui_accept detectado, consumiendo poción")
			consume_potion()
		elif Input.is_key_pressed(KEY_E):
			print("Potion: Tecla E detectada, consumiendo poción")
			consume_potion()

# Método alternativo para detectar input
func _input(event):
	if _player_inside and not _potion_consumed:
		if event is InputEventKey and event.pressed:
			if event.keycode == KEY_E or event.keycode == KEY_ENTER:
				print("Potion: Input event detectado, consumiendo poción")
				consume_potion()
				get_viewport().set_input_as_handled()

func _on_body_entered(body):
	print("Potion: Cuerpo entró en el área: ", body.name)
	print("Potion: Tipo de cuerpo: ", body.get_class())
	
	# Identificar si es el jugador
	var is_player = false
	
	# Múltiples formas de identificar al jugador
	if body.has_method("TakeDamage"):
		is_player = true
		print("Potion: Identificado como jugador por método TakeDamage")
	elif body is CharacterBody2D:
		is_player = true
		print("Potion: Identificado como jugador por tipo CharacterBody2D")
	elif body.name == "Character" or body.name.begins_with("Character"):
		is_player = true
		print("Potion: Identificado como jugador por nombre")
	
	if is_player and not _potion_consumed:
		_player_inside = true
		_player_body = body
		show_ui()
		print("Potion: Jugador dentro del área, mostrando UI")

func _on_body_exited(body):
	print("Potion: Cuerpo salió del área: ", body.name)
	
	if body == _player_body:
		_player_inside = false
		_player_body = null
		hide_ui()
		print("Potion: Jugador salió del área, ocultando UI")

func show_ui():
	# Mostrar indicadores visuales
	_selected_sprite.visible = true
	_tecla_sprite.visible = true
	print("Potion: UI mostrada")

func hide_ui():
	# Ocultar indicadores visuales
	_selected_sprite.visible = false
	_tecla_sprite.visible = false

func consume_potion():
	print("Potion: ¡Función consume_potion() ejecutada!")
	
	# Marcar como consumida para evitar múltiples consumos
	_potion_consumed = true
	
	# Ocultar la UI inmediatamente
	hide_ui()
	
	# Curar al jugador
	var current_health = _character_stats.current_health
	var max_health = 100.0  # Ajusta según tu sistema
	var new_health = min(current_health + HEAL_AMOUNT, max_health)
	_character_stats.set_health(new_health)
	
	print("Potion: Vida curada. Salud anterior: ", current_health, " Nueva salud: ", new_health)
	
	# Crear efecto de desaparición
	var tween = create_tween()
	
	# Efecto de curación - brillar antes de desaparecer
	tween.parallel().tween_property(_potion_sprite, "modulate", Color(0.5, 1.5, 0.5, 1.0), 0.2)  # Verde brillante
	tween.parallel().tween_property(_potion_sprite, "scale", Vector2(1.2, 1.2), 0.2)
	
	# Luego desaparecer
	tween.tween_property(_potion_sprite, "modulate:a", 0.0, 0.3)
	tween.parallel().tween_property(_potion_sprite, "position:y", _potion_sprite.position.y - 15, 0.3)
	tween.parallel().tween_property(_potion_sprite, "scale", Vector2(0.5, 0.5), 0.3)
	
	print("Potion: Iniciando animación de consumo")
	
	# Eliminar el nodo cuando termine la animación
	tween.finished.connect(func(): 
		print("Potion: Poción completamente consumida y eliminada")
		queue_free()
	)
