extends StaticBody2D

@onready var _door_sprite = $Sprite2D  # El sprite de la puerta que desaparecerá
@onready var _door_collision = $CollisionShape2D  # La colisión que desaparecerá
@onready var _candado_sprite = $Candado  # El sprite del candado
@onready var _tecla_sprite = $Tecla  # El sprite de la tecla E
@onready var _area = $Area2D  # El área de detección
@onready var _character_stats = get_node("/root/CharacterStats")

var _player_inside = false
var _player_body = null
var _door_opened = false  # Para evitar que se vuelva a mostrar la UI

func _ready():
	# Ocultar todos los sprites de UI inicialmente
	_candado_sprite.visible = false
	_tecla_sprite.visible = false
	
	# Conectar las señales del Area2D
	_area.body_entered.connect(_on_body_entered)
	_area.body_exited.connect(_on_body_exited)
	
	print("Door: Script iniciado correctamente")

func _process(delta):
	# Debug: mostrar estado actual
	if _player_inside and not _door_opened:
		var player_keys = _character_stats.keys
		# print("Door: Jugador dentro, llaves: ", player_keys, ", puerta abierta: ", _door_opened)
		
		if player_keys > 0:
			# Probar múltiples formas de detectar el input
			if Input.is_action_just_pressed("ui_accept"):
				print("Door: Tecla ui_accept detectada, abriendo puerta")
				open_door()
			elif Input.is_key_pressed(KEY_E):
				print("Door: Tecla E detectada, abriendo puerta")
				open_door()

# Alternativa: usar _input en lugar de _process
func _input(event):
	if _player_inside and not _door_opened and _character_stats.keys > 0:
		if event is InputEventKey and event.pressed:
			if event.keycode == KEY_E or event.keycode == KEY_ENTER:
				print("Door: Input event detectado, abriendo puerta")
				open_door()
				get_viewport().set_input_as_handled()

func _on_body_entered(body):
	print("Door: Cuerpo entró en el área: ", body.name)
	
	# Identificar si es el jugador
	var is_player = false
	
	if body.has_method("TakeDamage") or body is CharacterBody2D:
		is_player = true
	
	if is_player and not _door_opened:
		_player_inside = true
		_player_body = body
		update_ui()
		print("Door: Jugador dentro del área, llaves actuales: ", _character_stats.keys)

func _on_body_exited(body):
	print("Door: Cuerpo salió del área: ", body.name)
	
	if body == _player_body:
		_player_inside = false
		_player_body = null
		hide_ui()
		print("Door: Jugador salió del área")

func update_ui():
	# Solo mostrar UI si la puerta no está abierta
	if _door_opened:
		return
	
	var player_keys = _character_stats.keys
	print("Door: Actualizando UI, jugador tiene ", player_keys, " llaves")
	
	if player_keys > 0:
		# El jugador tiene llaves: mostrar candado y tecla E
		_candado_sprite.visible = true
		_tecla_sprite.visible = true
		print("Door: Mostrando candado y tecla E")
	else:
		# El jugador no tiene llaves: mostrar solo candado
		_candado_sprite.visible = true
		_tecla_sprite.visible = false
		print("Door: Mostrando solo candado")

func hide_ui():
	# Ocultar toda la UI
	_candado_sprite.visible = false
	_tecla_sprite.visible = false

func open_door():
	print("Door: ¡Función open_door() ejecutada!")
	
	# Marcar como abierta para evitar que se vuelva a activar
	_door_opened = true
	
	# Ocultar la UI inmediatamente
	hide_ui()
	
	# Consumir una llave
	_character_stats.add_keys(-1)
	print("Door: Llave consumida. Llaves restantes: ", _character_stats.keys)
	
	# Desactivar la colisión inmediatamente
	_door_collision.set_deferred("disabled", true)
	print("Door: Colisión desactivada")
	
	# Crear efecto de desaparición para la puerta
	var tween = create_tween()
	
	# Efecto de desvanecimiento y movimiento
	tween.parallel().tween_property(_door_sprite, "modulate:a", 0.0, 0.5)
	tween.parallel().tween_property(_door_sprite, "scale", Vector2(0.8, 0.8), 0.5)
	tween.parallel().tween_property(_door_sprite, "position:y", _door_sprite.position.y - 10, 0.5)
	
	print("Door: Iniciando animación de apertura")
	
	# Cuando termine la animación, ocultar completamente el sprite
	tween.finished.connect(func(): 
		_door_sprite.visible = false
		print("Door: Puerta completamente abierta y oculta")
	)
