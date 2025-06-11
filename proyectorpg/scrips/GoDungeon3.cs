using Godot;
using System;

public partial class GoDungeon3 : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Asegúrate de que el área esté conectada a la señal body_entered
		var area2D = GetNode<Area2D>("Area2D"); // Asume que tienes un nodo Area2D hijo
		if (area2D != null)
		{
			area2D.BodyEntered += OnArea2DBodyEntered;
		}
		else
		{
			GD.PrintErr("No se encontró el nodo Area2D");
		}
	}

	// Método correcto para manejar la señal (sin asteriscos)
	private void OnArea2DBodyEntered(Node2D body)
	{
		GD.Print($"Cuerpo entró en el área: {body.Name}");
		
		// Verificar si el cuerpo es un personaje
		if (body is Character)
		{			
			// Cambiar a la escena de fin de demo
			GetTree().ChangeSceneToFile("res://scenes/dungeon3.tscn");
		}
	}
}
