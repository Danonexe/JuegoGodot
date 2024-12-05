using Godot;
using System;

public partial class PausaMenu : Control
{
	//Arreglar esto
	public override void _Ready()
	{
		Visible = false;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel")) // "ui_cancel" es Escape por defecto
		{
			TogglePauseMenu();
		}
	}

	private void TogglePauseMenu()
	{
		if (Visible)
		{
			// Si el menú está visible, ocultarlo y reanudar el juego
			Visible = false;
			GetTree().Paused = false;
		}
		else
		{
			// Si el menú está oculto, mostrarlo y pausar el juego
			Visible = true;
			GetTree().Paused = true;
		}
	}

	
	public void _on_button_resume_pressed()
	{
		// Ocultar el menú de pausa y reanudar el juego
		Visible = false;
		GetTree().Paused = false;
	}

	// Método llamado al presionar el botón para ir al menú principal
	public void _on_button_go_to_menu_pressed()
	{
		// Cambiar a la escena del menú principal
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://menu/menu.tscn");
	}
}
