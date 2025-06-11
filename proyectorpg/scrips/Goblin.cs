using Godot;
using System;

public partial class Goblin : Enemy
{
	public override void _Ready()
	{
		// Llamar al método Ready del padre
		base._Ready();
		ATTACK_DISTANCE = 12.0f;
		
		// Estadísticas específicas del Slime
		MaxHealth = 100; 
		Damage = 25;     
		Speed = 30;    
		CurrentHealth = MaxHealth;
		
		// El slime da menos experiencia por ser más fácil
		ExperienceValue = 30;
		
		// El slime da pocos puntos por ser un enemigo fácil
		ScoreValue = 300;
	}
}
