using Godot;
using System;

public partial class ButtonStart : Button
{
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
	
	public void _on_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/dungeon.tscn");
	}
}
