using Godot;
using System;

public partial class PausaMenu : Control
{
    public override void _Ready()
    {
        // Asegúrate de que el menú pueda recibir entrada cuando el juego está pausado
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
    }

    public override void _Process(double delta)
    {
        // Verificar la acción de pausar solo si no estás en un menú
        if (Input.IsActionJustPressed("ui_cancel")) 
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

    public void _on_button_go_to_menu_pressed()
    {
        // Cambiar a la escena del menú principal
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://menu/menu.tscn");
    }
}