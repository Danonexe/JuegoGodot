extends Button

# URL de la API
var api_url = "https://api-net-juego.onrender.com/api/Estadisticas"
var line_edit

func _ready():
	# Conectar la señal del botón
	pressed.connect(_on_button_pressed)
	
	# Obtener referencia al LineEdit (asumiendo que está al mismo nivel, como hijo del mismo padre)
	line_edit = get_parent().get_node("LineEdit")
	
	if line_edit == null:
		push_error("No se pudo encontrar el LineEdit. Asegúrate de que sea un nodo hermano llamado 'LineEdit'")

# Función que se llama cuando se presiona el botón
func _on_button_pressed():
	# Verificar que se encontró el LineEdit
	if line_edit == null:
		print_debug("No se encontró el LineEdit")
		return
		
	var nick = line_edit.text
	
	# Verificar que el nick no esté vacío
	if nick == null || nick == "":
		print_debug("Nick vacío, no se enviará a la API")
		return
	
	# Deshabilitar el botón mientras se procesa
	disabled = true
	
	# Obtener el score real del sistema de estadísticas
	var character_stats = get_node("/root/CharacterStats")
	var real_score = 0
	
	if character_stats != null:
		real_score = character_stats.score
		print_debug("Score real obtenido: " + str(real_score))
	else:
		print_debug("No se pudo acceder a CharacterStats, usando score 0")
	
	# Obtener el tiempo real del temporizador global
	var temporizador = get_node("/root/Temporizador")
	var tiempo_formateado = "00:00.00"  # Valor predeterminado
	
	if temporizador != null:
		# Obtener el tiempo final (usando get_time en lugar de get_current_time)
		var tiempo_final = temporizador.get_time()
		# Formatear el tiempo usando el método format_time del temporizador
		tiempo_formateado = temporizador.format_time(tiempo_final)
		print_debug("Tiempo formateado para enviar: " + tiempo_formateado)
	else:
		print_debug("No se pudo acceder al temporizador, usando valor predeterminado")
	
	# Obtener la fecha actual en formato DD/MM/YYYY HH:MM:SS
	var datetime = Time.get_datetime_dict_from_system()
	var current_date = "%02d/%02d/%04d %02d:%02d:%02d" % [
		datetime.day, 
		datetime.month, 
		datetime.year,
		datetime.hour,
		datetime.minute,
		datetime.second
	]
	
	var data = {
		"Id": "",  # Usamos "Id" con I mayúscula y cadena vacía
		"nick": nick,
		"score": real_score,  # Usamos el score real en lugar del falso
		"time": tiempo_formateado,  # Usamos el tiempo formateado
		"date": current_date  # Incluimos la fecha desde el cliente
	}
	
	# Convertir a JSON
	var json_data = JSON.stringify(data)
	
	# Debug: mostrar los datos que se van a enviar
	print_debug("Datos a enviar: " + json_data)
	
	# Crear una solicitud HTTP
	var http_request = HTTPRequest.new()
	add_child(http_request)
	http_request.request_completed.connect(_on_request_completed)
	
	# Configurar los encabezados
	var headers = ["Content-Type: application/json"]
	
	# Enviar la solicitud POST
	var error = http_request.request(api_url, headers, HTTPClient.METHOD_POST, json_data)
	
	# Verificar si hubo un error al iniciar la solicitud
	if error != OK:
		print_debug("Error al conectar con el servidor: " + str(error))
		disabled = false

# Función que se llama cuando se completa la solicitud HTTP
func _on_request_completed(result, response_code, headers, body):
	disabled = false
	
	# Verificar si la solicitud fue exitosa
	if result != HTTPRequest.RESULT_SUCCESS:
		print_debug("Error de conexión: " + str(result))
		return
	
	# Verificar el código de respuesta HTTP
	if response_code == 201 or response_code == 200:  # Creado exitosamente
		print_debug("¡Nick registrado con éxito!")
		# Mostrar la respuesta del servidor para depuración
		var response_text = body.get_string_from_utf8()
		print_debug("Respuesta del servidor: " + response_text)
		line_edit.text = ""  # Limpiar el campo
	else:
		# Mostrar error en la consola
		var response_text = body.get_string_from_utf8()
		print_debug("Error del servidor: " + str(response_code))
		print_debug("Respuesta: " + response_text)
