using Godot;
using System;

public partial class ExitDungeon : Node2D
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
			// Detener el temporizador
			var temporizador = GetNode<Node>("/root/Temporizador");
			if (temporizador != null)
			{
				float tiempoFinal = (float)temporizador.Call("stop_timer");
				string tiempoFormateado = (string)temporizador.Call("format_time", tiempoFinal);
				GD.Print($"Temporizador detenido. Tiempo final: {tiempoFormateado}");
			}
			else
			{
				GD.PrintErr("No se pudo acceder al temporizador global");
			}
			
			// Cambiar a la escena de fin de demo
			GetTree().ChangeSceneToFile("res://menu/finDemo.tscn");
		}
	}
}
