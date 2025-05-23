extends Node2D

@onready var _animated_sprite = $AnimatedSprite2D  # El sprite animado de la llave
@onready var _tecla_sprite = $Tecla  # El sprite de la tecla
@onready var _area = $Area2D  # El área de detección
@onready var _character_stats = get_node("/root/CharacterStats")

var _player_inside = false
var _is_collected = false
var _player_body = null  # Guardar referencia al jugador

func _ready():
	# Ocultar el sprite de la tecla inicialmente
	_tecla_sprite.visible = false
	
	# Conectar las señales del Area2D
	_area.body_entered.connect(_on_body_entered)
	_area.body_exited.connect(_on_body_exited)
	
	print("Key: Script iniciado correctamente")

func _process(delta):
	# Solo verificar la tecla E si el jugador está dentro y la llave no ha sido recolectada
	if _player_inside and not _is_collected:
		# Probar múltiples formas de detectar el input
		if Input.is_action_just_pressed("ui_accept") or Input.is_key_pressed(KEY_E):
			print("Key: Tecla presionada, intentando recoger llave")
			collect_key()

func _on_body_entered(body):
	print("Key: Cuerpo entró en el área: ", body.name)
	print("Key: Tipo de cuerpo: ", body.get_class())
	
	# Probar múltiples formas de identificar al personaje
	var is_player = false
	
	# Método 1: Verificar si tiene el método TakeDamage
	if body.has_method("TakeDamage"):
		is_player = true
		print("Key: Identificado como jugador por método TakeDamage")
	
	# Método 2: Verificar por nombre del nodo
	if body.name == "Character" or body.name.begins_with("Character"):
		is_player = true
		print("Key: Identificado como jugador por nombre")
	
	# Método 3: Verificar por grupo (si tienes al personaje en un grupo llamado "player")
	if body.is_in_group("player"):
		is_player = true
		print("Key: Identificado como jugador por grupo")
	
	if is_player:
		_player_inside = true
		_player_body = body
		_tecla_sprite.visible = true
		print("Key: Jugador dentro del área, mostrando tecla")

func _on_body_exited(body):
	print("Key: Cuerpo salió del área: ", body.name)
	
	if body == _player_body:
		_player_inside = false
		_player_body = null
		_tecla_sprite.visible = false
		print("Key: Jugador salió del área, ocultando tecla")

func collect_key():
	print("Key: Función collect_key() llamada")
	
	# Marcar como recolectada para evitar múltiples recolecciones
	_is_collected = true
	
	# Ocultar el sprite de la tecla inmediatamente
	_tecla_sprite.visible = false
	
	# Añadir llave a las estadísticas
	_character_stats.add_keys(1)
	print("Key: Llave añadida a las estadísticas. Total llaves: ", _character_stats.keys)
	
	# Crear efecto de desaparición
	var tween = create_tween()
	tween.parallel().tween_property(_animated_sprite, "scale", Vector2(1.5, 1.5), 0.3)
	tween.parallel().tween_property(_animated_sprite, "modulate:a", 0.0, 0.3)
	tween.parallel().tween_property(self, "position:y", position.y - 20, 0.3)
	
	print("Key: Iniciando animación de desaparición")
	
	# Eliminar el nodo cuando termine la animación
	tween.finished.connect(func(): 
		print("Key: Animación terminada, eliminando nodo")
		queue_free()
	)
