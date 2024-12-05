using Godot;
using System;

public partial class ObjetoRompible : StaticBody2D
{
	// Clase de la que heredan todas las cosas que se rompen, comentar y añadir que puedan soltar vida o perks

	protected Sprite2D SpriteNormal;
	protected Sprite2D SpriteRoto;

	public override void _Ready()
	{
	  
		SpriteNormal = GetNode<Sprite2D>("Sprite2D");
		SpriteRoto = GetNode<Sprite2D>("Sprite2DRoto");

		
		SpriteRoto.Visible = false;
	}

	
	public void Romperse(){
		SpriteNormal.Visible = false; 
		SpriteRoto.Visible = true;
	}
}
