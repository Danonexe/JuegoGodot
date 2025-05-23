using Godot;
using System;

public partial class ObjetoRompible : StaticBody2D
{
	// Clase de la que heredan todas las cosas que se rompen, comentar y añadir que puedan soltar vida o perks

	protected Sprite2D SpriteNormal;
	protected Sprite2D SpriteRoto;
	
	// Variable para controlar si el objeto ya está destruido
	private bool _isDestroyed = false;
	
	// Ruta a la escena de la poción - ajusta según tu estructura de carpetas
	private const string PotionScene = "res://world/potion.tscn";  // Cambia por la ruta correcta
	
	// Probabilidad de que aparezca una poción (20% = 2 de cada 10)
	private const float PotionDropChance = 0.2f;
	
	private Random _random = new Random();

	public override void _Ready()
	{
		SpriteNormal = GetNode<Sprite2D>("Sprite2D");
		SpriteRoto = GetNode<Sprite2D>("Sprite2DRoto");
		
		SpriteRoto.Visible = false;
	}

	public void Romperse()
	{
		// Si ya está destruido, no hacer nada
		if (_isDestroyed)
		{
			GD.Print("ObjetoRompible: El objeto ya está destruido, ignorando golpe adicional.");
			return;
		}
		
		// Marcar como destruido para evitar múltiples ejecuciones
		_isDestroyed = true;
		
		GD.Print("ObjetoRompible: ¡Objeto destruido por primera vez!");
		
		// Cambiar sprites
		SpriteNormal.Visible = false; 
		SpriteRoto.Visible = true;
		
		// Añadir movimiento suave pero más rápido al sprite roto
		AddFasterSmoothBreakMovement();
		
		// Posibilidad de crear una poción
		TryDropPotion();
	}
	
	private void AddFasterSmoothBreakMovement()
	{
		// Crear un tween para el movimiento suave pero un poco más rápido
		var tween = CreateTween();
		
		// Guardar la posición original
		Vector2 originalPosition = SpriteRoto.Position;
		
		// Movimiento suave pero más rápido
		// 1. Subir suavemente (un poco más rápido)
		tween.TweenProperty(SpriteRoto, "position:y", originalPosition.Y - 3, 0.06f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.Out);
		
		// 2. Pequeña oscilación lateral muy suave y rápida
		tween.TweenProperty(SpriteRoto, "position:x", originalPosition.X + 1.5f, 0.04f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
		
		tween.TweenProperty(SpriteRoto, "position:x", originalPosition.X - 1.5f, 0.04f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
		
		tween.TweenProperty(SpriteRoto, "position:x", originalPosition.X, 0.04f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
		
		// 3. Volver a la posición original suavemente pero rápido
		tween.TweenProperty(SpriteRoto, "position:y", originalPosition.Y, 0.08f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
	}
	
	private void TryDropPotion()
	{
		// Generar número aleatorio entre 0 y 1
		float randomValue = (float)_random.NextDouble();
		
		GD.Print($"ObjetoRompible: Valor aleatorio: {randomValue:F2}, Probabilidad: {PotionDropChance} (20%)");
		
		// Si el valor es menor que la probabilidad, crear poción
		if (randomValue < PotionDropChance)
		{
			GD.Print("ObjetoRompible: ¡Va a aparecer una poción!");
			CreatePotion();
		}
		else
		{
			GD.Print("ObjetoRompible: No apareció poción esta vez.");
		}
	}
	
	private void CreatePotion()
	{
		try
		{
			// Cargar la escena de la poción
			PackedScene potionScene = ResourceLoader.Load<PackedScene>(PotionScene);
			if (potionScene == null)
			{
				GD.PrintErr($"ObjetoRompible: No se pudo cargar la escena de poción en: {PotionScene}");
				return;
			}
			
			// Instanciar la poción
			Node2D potion = potionScene.Instantiate<Node2D>();
			
			// Posicionar la poción más cerca PERO AÚN ABAJO del objeto
			Vector2 potionPosition = GlobalPosition + new Vector2(0, 15); // 15 píxeles abajo (antes era 25)
			potion.GlobalPosition = potionPosition;
			
			// Añadir la poción al árbol de nodos (añadirla a la escena principal)
			GetTree().CurrentScene.AddChild(potion);
			
			// Añadir efecto de aparición suave a la poción
			AddSmoothPotionSpawnEffect(potion);
			
			GD.Print("ObjetoRompible: ¡Poción creada cerca del objeto!");
		}
		catch (Exception e)
		{
			GD.PrintErr($"ObjetoRompible: Error al crear poción: {e.Message}");
		}
	}
	
	private void AddSmoothPotionSpawnEffect(Node2D potion)
	{
		// Obtener el sprite de la poción
		Sprite2D potionSprite = potion.GetNodeOrNull<Sprite2D>("Sprite2D");
		if (potionSprite == null)
		{
			GD.Print("ObjetoRompible: No se encontró el sprite de la poción para efectos");
			return;
		}
		
		// Configurar estado inicial para el efecto
		potionSprite.Scale = Vector2.Zero;
		potionSprite.Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		potion.Position += new Vector2(0, 10); // Empezar un poco más abajo (reducido de 15 a 10)
		
		// Crear tween para el efecto de aparición suave
		var tween = potion.CreateTween();
		
		// Fase 1: Aparecer suavemente
		tween.Parallel().TweenProperty(potionSprite, "scale", Vector2.One, 0.4f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.Out);
		
		tween.Parallel().TweenProperty(potionSprite, "modulate:a", 1.0f, 0.4f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.Out);
		
		// Fase 2: Subir suavemente a la posición final (movimiento más pequeño)
		tween.Parallel().TweenProperty(potion, "position:y", potion.Position.Y - 10, 0.5f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.Out);
		
		// Fase 3: Brillo suave final
		tween.TweenProperty(potionSprite, "modulate", new Color(1.3f, 1.3f, 1.0f, 1.0f), 0.2f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
		
		tween.TweenProperty(potionSprite, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.2f)
			 .SetTrans(Tween.TransitionType.Sine)
			 .SetEase(Tween.EaseType.InOut);
	}
	
	// Método público para verificar si está destruido (opcional, por si lo necesitas)
	public bool IsDestroyed()
	{
		return _isDestroyed;
	}
}