using Godot;
using System;

public partial class ButtonStart : Button
{
	public override void _Ready()
	{
		// Conectar la señal Pressed al método que maneja el evento
		Pressed += OnButtonPressed;
	}
	
	private void OnButtonPressed()
	{
		// Resetear las estadísticas del personaje
		ResetPlayerStats();
		
		// Acceder al singleton de variables globales
		var temporizador = GetNode<Node>("/root/Temporizador");
		
		if (temporizador != null)
		{
			// Iniciar el timer
			temporizador.Call("start_timer");
			GD.Print("Timer iniciado desde botón. Cambiando a escena dungeon...");
		}
		else
		{
			GD.PrintErr("No se pudo acceder a las variables globales");
		}
		
		// Cambiar a la escena dungeon
		GetTree().ChangeSceneToFile("res://scenes/dungeon.tscn");
	}
	
	private void ResetPlayerStats()
	{
		// Acceder al singleton de estadísticas del personaje
		var characterStats = GetNode<Node>("/root/CharacterStats");
		
		if (characterStats != null)
		{
			// Resetear vida al máximo (100)
			characterStats.Call("set_health", 100.0f);
			
			// Resetear estamina al máximo (85)
			characterStats.Call("set_stamina", 85.0f);
			
			// Resetear nivel a 1
			characterStats.Set("current_level", 1);
			
			// Resetear experiencia a 0
			characterStats.Set("current_experience", 0);
			
			// Resetear experiencia requerida para nivel 2
			characterStats.Set("experience_required", 20);
			
			// Resetear ataque al valor inicial (20)
			characterStats.Call("set_attack_damage", 20);
			
			// Resetear defensa al valor inicial (0)
			characterStats.Call("set_defense", 0);
			
			// Resetear llaves a 0
			characterStats.Call("set_keys", 0);
			
			// Resetear puntuación a 0
			characterStats.Call("set_score", 0);
			
			GD.Print("ButtonStart: Estadísticas del personaje reseteadas");
			GD.Print("- Vida: 100/100");
			GD.Print("- Estamina: 85/85");
			GD.Print("- Nivel: 1");
			GD.Print("- Experiencia: 0/20");
			GD.Print("- Ataque: 20");
			GD.Print("- Defensa: 0");
			GD.Print("- Llaves: 0");
			GD.Print("- Puntuación: 0");
		}
		else
		{
			GD.PrintErr("ButtonStart: No se pudo acceder a CharacterStats");
		}
	}
}
