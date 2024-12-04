using Godot;
using System;

public partial class Jarron : StaticBody2D
{
	protected Sprite2D Sprite; 
	protected Sprite2D SpriteRoto; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D");
		SpriteRoto = GetNode<Sprite2D>("Sprite2DRoto");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_area_2d_area_entered(Area2D area){
		Sprite.Visible = false; 
		SpriteRoto.Visible = true;
	 GD.Print("Rompete");
	}
}
