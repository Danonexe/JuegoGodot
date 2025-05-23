using Godot;
using System;

public partial class Character : CharacterBody2D
{
	// Estados del personaje
	private enum CharacterState
	{
		Idle,       
		Moving,     
		Attacking,
		Death  
	}

	// Referencias al sistema de estadísticas global
	private Node characterStats;

	// Constantes de movimiento
	private const float MaxSpeed = 110.0f; 
	private const float Acceleration = 500.0f; 
	private const float Friction = 500.0f; 

	// Variables para el ataque no estadísticas
	private const int AttackFrames = 30; // Duración del ataque en fotogramas
	
	// Variables para muerte
	private const int DeathFrames = 45; // Duración de la animación de muerte
	private int _currentDeathFrame = 0;

	// Gestión del estado
	private CharacterState _currentState = CharacterState.Idle; 
	private int _currentAttackFrame = 0; // Fotogramas actuales del ataque

	// Dirección y movimiento
	private Vector2 _lastDirection = Vector2.Down; 
	private Vector2 _currentDirection = Vector2.Zero; 

	// Nodos
	private AnimatedSprite2D _animatedSprite; 
	private TextureProgressBar _staminaBar; 
	private TextureProgressBar _healthBar; 
	private Camera2D _camera; 
	
	// Ruta a la escena de texto flotante
	private const string FloatingTextScene = "res://menu/FloatingText.tscn";  // Ruta actualizada

	public override void _Ready()
	{
		// Obtener referencia a CharacterStats
		characterStats = GetNode<Node>("/root/CharacterStats");
		
		// Conectar a las señales de CharacterStats
		characterStats.Connect("health_changed", Callable.From((float newHealth) => UpdateHealthBar(newHealth)));
		characterStats.Connect("stamina_changed", Callable.From((float newStamina) => UpdateStaminaBar(newStamina)));
		characterStats.Connect("level_up", Callable.From((int newLevel, Vector2 position) => ShowLevelUpText(newLevel)));

		// Inicializar nodos
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		var healthStaminaBar = GetNode<Node2D>("HealthBar");
		_staminaBar = healthStaminaBar.GetNode<TextureProgressBar>("Stamina");
		_healthBar = healthStaminaBar.GetNode<TextureProgressBar>("Health");
		_camera = GetNode<Camera2D>("Camera2D");

		// Actualizar las barras con valores iniciales de CharacterStats
		UpdateHealthAndStaminaBars();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Solo regenerar estamina si no está muerto
		if (_currentState != CharacterState.Death)
		{
			RegenerateStamina();
		}

		// Manejar el estado actual
		switch (_currentState)
		{
			case CharacterState.Attacking:
				ProcessAttackState(delta);
				break;
			case CharacterState.Idle:
			case CharacterState.Moving:
				ProcessMovementState(delta);
				break;
			case CharacterState.Death:
				ProcessDeathState(delta);
				break;
		}
	}

	private void ProcessMovementState(double delta)
	{
		// Bloquear movimiento si estás muerto
		if (_currentState == CharacterState.Death) return;

		// Obtener dirección de entrada
		_currentDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();

		Vector2 velocity = Velocity;

		// Manejar movimiento
		if (_currentDirection != Vector2.Zero)
		{
			velocity = velocity.MoveToward(_currentDirection * MaxSpeed, Acceleration * (float)delta);
			_currentState = CharacterState.Moving;
			_lastDirection = _currentDirection;
			UpdateAnimation(_currentDirection, true);
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
			_currentState = CharacterState.Idle;
			UpdateAnimation(_lastDirection, false);
		}

		// Bloquear ataque si estás muerto
		if (_currentState == CharacterState.Death) return;

		// Manejar ataque
		if (Input.IsActionJustPressed("ui_accept") && CanAttack())
		{
			StartAttack();
			return;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ProcessAttackState(double delta)
	{
		// Manejar la duración del ataque
		_currentAttackFrame++;
		if (_currentAttackFrame >= AttackFrames)
		{
			EndAttack();
		}
	}

	private void ProcessDeathState(double delta)
	{
		_currentDeathFrame++;

		// Reproducir animación de muerte
		if (_currentDeathFrame == 1)
		{
			_animatedSprite.Play("death");
		}

		// Finalizar y eliminar el personaje
		if (_currentDeathFrame >= DeathFrames)
		{
			_animatedSprite.Visible=false;
			if(_currentDeathFrame>=60){
				  GetTree().ChangeSceneToFile("res://menu/menu.tscn");
			}
		}
	}

	private bool CanAttack()
	{
		// Obtener estamina de CharacterStats
		float currentStamina = (float)characterStats.Get("current_stamina");
		float attackStaminaCost = 35f; // Coste de estamina del ataque
		
		// No se puede atacar si estás muerto o sin suficiente estamina
		return _currentState != CharacterState.Attacking 
			&& _currentState != CharacterState.Death 
			&& currentStamina >= attackStaminaCost;
	}

	private void StartAttack()
	{
		// Iniciar ataque
		_currentState = CharacterState.Attacking;
		_currentAttackFrame = 0;
		
		// Reducir estamina
		float currentStamina = (float)characterStats.Get("current_stamina");
		float attackStaminaCost = 35f;
		characterStats.Call("set_stamina", currentStamina - attackStaminaCost);

		// Determinar última dirección
		string attackAnimation = DetermineAttackAnimation();
		_animatedSprite.Play(attackAnimation);

		// Activar la zona de golpe correspondiente
		ActivateAttackArea();
	}

	private void EndAttack()
	{
		// Finalizar ataque
		_currentState = CharacterState.Idle;
		_currentAttackFrame = 0;
		UpdateAnimation(_lastDirection, false);

		// Desactivar todas las áreas de ataque
		DeactivateAllAttackAreas();
	}

	private string DetermineAttackAnimation()
	{
		// Determinar animación de ataque según la última dirección
		if (_lastDirection.X > 0) return "attackRight";
		if (_lastDirection.X < 0) return "attackLeft";
		if (_lastDirection.Y > 0) return "attack";
		if (_lastDirection.Y < 0) return "attackBack";
		return "attack";
	}

	private void UpdateAnimation(Vector2 direction, bool isMoving)
	{
		// Actualizar la animación del personaje según la dirección y el movimiento
		if (direction.X > 0)
		{
			_animatedSprite.Play(isMoving ? "walkRight" : "idleRight");
		}
		else if (direction.X < 0)
		{
			_animatedSprite.Play(isMoving ? "walkLeft" : "idleLeft");
		}
		else if (direction.Y > 0)
		{
			_animatedSprite.Play(isMoving ? "walk" : "idle");
		}
		else if (direction.Y < 0)
		{
			_animatedSprite.Play(isMoving ? "walkBack" : "idleBack");
		}
	}

	private void ActivateAttackArea()
	{
		// Por si acaso de nuevo
		DeactivateAllAttackAreas();

		// Activar el área de ataque según la dirección
		switch (DetermineAttackDirection())
		{
			case "Right":
				GetNode<Area2D>("AreaRightAttack").Monitoring = true;
				break;
			case "Left":
				GetNode<Area2D>("AreaLeftAttack").Monitoring = true;
				break;
			case "Front":
				GetNode<Area2D>("AreaFrontAttack").Monitoring = true;
				break;
			case "Back":
				GetNode<Area2D>("AreaBackAttack").Monitoring = true;
				break;
		}
	}

	//Areas de ataque
	private void _on_area_front_attack_body_entered(Node body)
	{
		hit(body);
	}

	private void _on_area_back_attack_body_entered(Node body)
	{
		hit(body);
	}

	private void _on_area_right_attack_body_entered(Node body)
	{
		hit(body);
	}

	private void _on_area_left_attack_body_entered(Node body)
	{
		hit(body);
	}

	private void hit(Node body){
		if (body is Enemy enemy)
		{
			// Obtener daño de ataque desde CharacterStats
			int attackDamage = (int)characterStats.Get("attack_damage");
			enemy.TakeDamage(attackDamage);
		}
		if (body is ObjetoRompible objeto)
		{
			objeto.Romperse(); 
		}
	}

	private void DeactivateAllAttackAreas()
	{
		// Desactivar todas las áreas de golpe
		GetNode<Area2D>("AreaRightAttack").Monitoring = false;
		GetNode<Area2D>("AreaLeftAttack").Monitoring = false;
		GetNode<Area2D>("AreaFrontAttack").Monitoring = false;
		GetNode<Area2D>("AreaBackAttack").Monitoring = false;
	}

	private string DetermineAttackDirection()
	{
		// Elegir dirección del ataque según la dirección
		if (_lastDirection.X > 0) return "Right";
		if (_lastDirection.X < 0) return "Left";
		if (_lastDirection.Y > 0) return "Front";
		if (_lastDirection.Y < 0) return "Back";
		return "Front";
	}

	// Salud y Stamina
	private void RegenerateStamina()
	{
		// Obtener estamina actual
		float currentStamina = (float)characterStats.Get("current_stamina");
		float maxStamina = 85f;
		
		// Regenerar estamina
		currentStamina = Math.Min(currentStamina + 0.35f, maxStamina);
		characterStats.Call("set_stamina", currentStamina);
	}

	private void UpdateHealthAndStaminaBars()
	{
		// Obtener valores actuales
		float currentHealth = (float)characterStats.Get("current_health");
		float currentStamina = (float)characterStats.Get("current_stamina");
		
		// Actualizar las barras
		_healthBar.Value = currentHealth;
		_staminaBar.Value = currentStamina;
	}
	
	// Funciones para actualizar individualmente las barras
	private void UpdateHealthBar(float newHealth)
	{
		_healthBar.Value = newHealth;
	}
	
	private void UpdateStaminaBar(float newStamina)
	{
		_staminaBar.Value = newStamina;
	}
	
	// Método para mostrar el texto de subida de nivel
	private void ShowLevelUpText(int newLevel)
	{
		// Cargar la escena de texto flotante
		PackedScene textScene = ResourceLoader.Load<PackedScene>(FloatingTextScene);
		Node2D floatingText = textScene.Instantiate<Node2D>();
		
		// Configurar el texto usando los métodos en GDScript
		floatingText.Call("set_text", "LVL UP");
		floatingText.Call("set_color", new Color(1.0f, 0.8f, 0.2f)); // Color dorado
		
		// Posicionar encima del personaje
		floatingText.Position = new Vector2(0, -30); // Un poco arriba
		
		// Añadir al árbol de nodos
		AddChild(floatingText);
	}

	public void TakeDamage(int damage)
	{
		// Obtener valores de CharacterStats
		float currentHealth = (float)characterStats.Get("current_health");
		int defense = (int)characterStats.Get("defense");
		
		// Calcular daño después de defensa
		int finalDamage = (int)characterStats.Call("calculate_damage_taken", damage);
		
		// Aplicar daño
		currentHealth = Math.Max(0, currentHealth - finalDamage);
		characterStats.Call("set_health", currentHealth);
		
		// Comprobar si está muerto
		bool isDead = (bool)characterStats.Call("is_player_dead");
		
		// Si la salud llega a 0, cambiar al estado de muerte
		if (isDead)
		{
			_currentState = CharacterState.Death;
			_currentDeathFrame = 0;
			// Detener cualquier movimiento o acción
			Velocity = Vector2.Zero;
		}
		else
		{
			ShakeCameraOnHit();
		}
	}

	private void ShakeCameraOnHit()
	{
		// Sacudir la cámara al recibir daño
		var tween = GetTree().CreateTween();
		tween.TweenProperty(_camera, "offset:x", 5.0, 0.1)
			 .SetTrans(Tween.TransitionType.Bounce)
			 .SetEase(Tween.EaseType.Out);
		tween.TweenProperty(_camera, "offset:x", -5.0, 0.1)
			 .SetTrans(Tween.TransitionType.Bounce)
			 .SetEase(Tween.EaseType.Out);
		tween.TweenProperty(_camera, "offset:x", 0.0, 0.1)
			 .SetTrans(Tween.TransitionType.Bounce)
			 .SetEase(Tween.EaseType.Out);
	}
}