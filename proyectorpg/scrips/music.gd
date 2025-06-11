extends Node

var music_player: AudioStreamPlayer

func _ready():
	# Crear el reproductor de música global
	music_player = AudioStreamPlayer.new()
	music_player.name = "GlobalMusicPlayer"
	add_child(music_player)
	print("MusicManager: Listo para reproducir música global")

func play_game_music():
	# Esta función se llama cuando presionas Start
	var game_music = load("res://assets/sfx/map.mp3")  # Cambia por tu ruta
	music_player.stream = game_music
	music_player.volume_db = -30.0
	music_player.play()
	print("MusicManager: Música del juego iniciada")

func stop_music():
	if music_player.is_playing():
		music_player.stop()
		print("MusicManager: Música detenida")

func reset_music():
	# Función que reinicia completamente el reproductor de música
	music_player.stop()  # Para sin importar el estado
	music_player.stream = null  # Limpia el stream
	music_player.volume_db = -30.0  # Resetea el volumen
	print("MusicManager: Música reseteada completamente")

func is_playing() -> bool:
	return music_player.is_playing()
