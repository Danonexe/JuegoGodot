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
}
