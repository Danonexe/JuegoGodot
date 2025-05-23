extends Node

# Estadísticas dinámicas del jugador
var current_health: float = 100
var current_stamina: float = 85
var attack_damage: int = 20
var defense: int = 0
# Variables de nivel y experiencia
var current_level: int = 1
var current_experience: int = 0
var experience_required: int = 20  # Experiencia inicial para nivel 2
# Variable de llaves
var keys: int = 0
# Variable de puntuación
var score: int = 0

# Señales para notificar cambios
signal health_changed(new_value)
signal stamina_changed(new_value)
signal damage_changed(new_value)
signal defense_changed(new_value)
signal level_up(new_level, position)  # Señal para cuando sube de nivel
signal experience_changed(new_value, required_value)
signal keys_changed(new_value)  # Nueva señal para llaves
signal score_changed(new_value)  # Nueva señal para puntuación

# Funciones para modificar los valores
func set_health(value: float) -> void:
	current_health = value
	emit_signal("health_changed", current_health)

func set_stamina(value: float) -> void:
	current_stamina = value
	emit_signal("stamina_changed", current_stamina)
	
func set_attack_damage(value: int) -> void:
	attack_damage = value
	emit_signal("damage_changed", attack_damage)
	
func set_defense(value: int) -> void:
	defense = value
	emit_signal("defense_changed", defense)

# Nueva función para modificar llaves
func set_keys(value: int) -> void:
	keys = value
	emit_signal("keys_changed", keys)

# Función para añadir llaves
func add_keys(value: int) -> void:
	keys += value
	emit_signal("keys_changed", keys)

# Nueva función para modificar puntuación
func set_score(value: int) -> void:
	score = value
	emit_signal("score_changed", score)

# Función para añadir puntuación
func add_score(value: int) -> void:
	score += value
	emit_signal("score_changed", score)
	print("CharacterStats: +", value, " puntos! Puntuación total: ", score)

# Nueva función para añadir experiencia
func add_experience(value: int, player_position = null) -> void:
	current_experience += value
	emit_signal("experience_changed", current_experience, experience_required)
	
	# Verificar si se ha alcanzado la experiencia necesaria para subir de nivel
	if current_experience >= experience_required:
		perform_level_up(player_position)  # Cambiado el nombre de la función

# Nueva función para subir de nivel (renombrada)
func perform_level_up(player_position = null) -> void:  # Cambiado el nombre de 'level_up' a 'perform_level_up'
	current_level += 1
	current_experience -= experience_required  # Restar la experiencia usada
	
	# Aumentar la experiencia requerida para el siguiente nivel
	experience_required = int(experience_required * 1.5)
	
	# Aumentar estadísticas (ataque y defensa +5)
	attack_damage += 5
	defense += 5
	
	# Emitir señales de cambio
	emit_signal("level_up", current_level, player_position)
	emit_signal("damage_changed", attack_damage)
	emit_signal("defense_changed", defense)
	emit_signal("experience_changed", current_experience, experience_required)

# Funciones útiles
func is_player_dead() -> bool:
	return current_health <= 30
	
# Calcula el daño recibido después de aplicar la defensa
func calculate_damage_taken(damage: int) -> int:
	var final_damage = damage - defense
	return max(1, final_damage) # Asegurar que al menos haga 1 de daño
