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
	
	var data = {
		"Id": "",
		"nick": nick,
		"score": 0,
		"time": 0
	}
	
	# Convertir a JSON
	var json_data = JSON.stringify(data)
	
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
		print_debug("Error al conectar con el servidor")
		disabled = false

# Función que se llama cuando se completa la solicitud HTTP
func _on_request_completed(result, response_code, headers, body):
	disabled = false
	
	# Verificar si la solicitud fue exitosa
	if result != HTTPRequest.RESULT_SUCCESS:
		print_debug("Error de conexión")
		return
	
	# Verificar el código de respuesta HTTP
	if response_code == 201 or response_code == 200:  # Creado exitosamente
		print_debug("¡Nick registrado con éxito!")
		line_edit.text = ""  # Limpiar el campo
	else:
		# Mostrar error en la consola
		var response_text = body.get_string_from_utf8()
		print_debug("Error del servidor: ", response_code)
		print_debug("Respuesta: ", response_text)
