extends Node

# Variables del temporizador
var timer : Timer = null
var timer_running : bool = false
var current_time : float = 0.0

# Variable para controlar la visibilidad del temporizador
var show_timer : bool = true

# Referencias a nodos de UI
var canvas_layer : CanvasLayer = null
var panel : Panel = null
var time_label : Label = null

# Señal para cuando el timer se detiene
signal timer_stopped(final_time: float)

func _ready():
	# Crear el timer
	timer = Timer.new()
	timer.one_shot = false
	timer.autostart = false
	add_child(timer)
	
	# Conectar la señal de timeout
	timer.timeout.connect(_on_timer_timeout)
	
	# Crear la interfaz de usuario simplificada
	setup_simple_timer_ui()
	
	print("Sistema global inicializado")

func setup_simple_timer_ui():
	# Crear un CanvasLayer
	canvas_layer = CanvasLayer.new()
	canvas_layer.layer = 100
	add_child(canvas_layer)
	
	# Crear un Control para posicionamiento
	var control = Control.new()
	control.set_anchors_preset(Control.PRESET_FULL_RECT)  # Ocupar toda la pantalla
	control.mouse_filter = Control.MOUSE_FILTER_IGNORE
	canvas_layer.add_child(control)
	
	# Crear un panel simple
	panel = Panel.new()
	panel.set_anchors_preset(Control.PRESET_BOTTOM_RIGHT)  # Anclar a la esquina inferior derecha
	panel.mouse_filter = Control.MOUSE_FILTER_IGNORE
	panel.custom_minimum_size = Vector2(80, 28)
	
	# Hacer el panel semitransparente
	var style = StyleBoxFlat.new()
	style.bg_color = Color(0, 0, 0, 0.4)
	style.corner_radius_top_left = 4
	style.corner_radius_top_right = 4
	style.corner_radius_bottom_left = 4
	style.corner_radius_bottom_right = 4
	panel.add_theme_stylebox_override("panel", style)
	
	# Añadir márgenes para separar del borde de la pantalla
	panel.position = Vector2(-90, -38)  # Mover arriba y a la izquierda del ancla
	control.add_child(panel)
	
	# Crear la etiqueta de tiempo
	time_label = Label.new()
	time_label.text = "00:00.00"
	time_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	time_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	time_label.set_anchors_preset(Control.PRESET_FULL_RECT)  # Ocupar todo el panel
	time_label.add_theme_font_size_override("font_size", 18)
	time_label.add_theme_color_override("font_color", Color(1, 1, 1))
	panel.add_child(time_label)
	
	# Configurar la visibilidad inicial
	canvas_layer.visible = show_timer
	
	print("UI simple del temporizador configurada")

func start_timer():
	current_time = 0.0
	timer_running = true
	timer.start(0.1)
	print("Timer iniciado, tiempo actual: ", current_time)
	
	# Asegurarse de que la UI sea visible si show_timer es true
	if canvas_layer and show_timer:
		canvas_layer.visible = true

func stop_timer() -> float:
	if timer_running:
		timer.stop()
		timer_running = false
		timer_stopped.emit(current_time)
		print("Timer detenido, tiempo final: ", current_time)
		return current_time
	return current_time

func get_time() -> float:
	return current_time

func format_time(time_val: float) -> String:
	var minutes := int(time_val / 60)
	var seconds := int(time_val) % 60
	var milliseconds := int((time_val - int(time_val)) * 100)
	return "%02d:%02d.%02d" % [minutes, seconds, milliseconds]

func _process(_delta):
	# Actualizar el texto de la etiqueta si está visible
	if time_label and show_timer and timer_running:
		time_label.text = format_time(current_time)

func _on_timer_timeout():
	if timer_running:
		current_time += 0.1

# Método para cambiar la visibilidad del temporizador
func set_timer_visibility(visible: bool):
	show_timer = visible
	if canvas_layer:
		canvas_layer.visible = visible
