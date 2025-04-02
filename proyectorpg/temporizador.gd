extends Node

# Variables del temporizador
var timer : Timer = null
var timer_running : bool = false
var current_time : float = 0.0

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
	
	print("Sistema global inicializado")

func start_timer():
	current_time = 0.0
	timer_running = true
	timer.start(0.1)
	print("Timer iniciado, tiempo actual: ", current_time)

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

func _on_timer_timeout():
	if timer_running:
		current_time += 0.1
