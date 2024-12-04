using Godot;
using System;

public partial class ObjetoRompible : StaticBody2D
{
	protected Sprite2D Sprite; 
	protected Sprite2D SpriteRoto; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D");
		SpriteRoto = GetNode<Sprite2D>("Sprite2DRoto");
	}

	public void breakObject(){
		Sprite.Visible = false; 
		SpriteRoto.Visible = true;
	}


}
