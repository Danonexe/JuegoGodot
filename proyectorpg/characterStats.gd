extends Node

# Estadísticas dinámicas del jugador
var current_health: float = 100
var current_stamina: float = 85
var attack_damage: int = 20
var defense: int = 0

# Señales para notificar cambios
signal health_changed(new_value)
signal stamina_changed(new_value)
signal damage_changed(new_value)
signal defense_changed(new_value)

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

# Funciones útiles
func is_player_dead() -> bool:
	return current_health <= 30
	
# Calcula el daño recibido después de aplicar la defensa
func calculate_damage_taken(damage: int) -> int:
	var final_damage = damage - defense
	return max(1, final_damage) # Asegurar que al menos haga 1 de daño
