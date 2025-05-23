extends Node2D

var _lifetime = 1.5  # Tiempo de vida en segundos
var _current_time = 0.0
var _velocity = Vector2(0, -50)  # Velocidad hacia arriba
var _label = null

func _ready():
	# Buscar el nodo Label
	_label = $Label
	if _label == null:
		push_error("No se pudo encontrar el nodo Label en FloatingText. Asegúrate de que existe y se llama 'Label'")
		return
	
	# Configurar animación de aparición
	scale = Vector2(0.1, 0.1)
	var tween = create_tween()
	tween.tween_property(self, "scale", Vector2(1.0, 1.0), 0.2)

func _process(delta):
	# Mover el texto hacia arriba
	position += _velocity * delta
	
	# Tiempo de vida
	_current_time += delta
	if _current_time >= _lifetime:
		# Desvanecer antes de eliminar
		var tween = create_tween()
		tween.tween_property(self, "modulate:a", 0.0, 0.3)
		tween.finished.connect(func(): queue_free())

func set_text(text):
	if _label != null:
		_label.text = text

func set_color(color):
	if _label != null:
		_label.modulate = color
