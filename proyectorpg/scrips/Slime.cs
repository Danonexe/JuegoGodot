using Godot;
using System;

public partial class Slime : Enemy
{
	public override void _Ready()
	{
		// Llamar al método Ready del padre
		base._Ready();
		
		// Estadísticas específicas del Slime
		MaxHealth = 100; 
		Damage = 15;     
		Speed = 30;    
		CurrentHealth = MaxHealth;
		
		// El slime da menos experiencia por ser más fácil
		ExperienceValue = 10;
		
		// El slime da pocos puntos por ser un enemigo fácil
		ScoreValue = 100;
	}
}
