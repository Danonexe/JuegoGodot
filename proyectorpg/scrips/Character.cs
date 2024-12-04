using Godot;
using System;

public partial class Character : CharacterBody2D
{
	// Enumeración para los estados del personaje
	private enum CharacterState
	{
		Idle,       
		Moving,     
		Attacking  
	}

	// Estadísticas del personaje
	[Export] public int MaxHealth { get; private set; } = 100; 
	[Export] public float MaxStamina { get; private set; } = 85; 
	[Export] public float CurrentHealth { get; private set; } 
	[Export] public float CurrentStamina { get; private set; } 

	// Constantes de movimiento
	private const float MaxSpeed = 110.0f; 
	private const float Acceleration = 500.0f; 
	private const float Friction = 500.0f; 

	// Constantes de ataque
	private const int AttackFrames = 30; // Duración del ataque en fotogramas
	private const float AttackStaminaCost = 35f; // Resistencia del ataque
	private const int AttackDamage = 20; 

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

	public override void _Ready()
	{
		// Inicializar nodos
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		var healthStaminaBar = GetNode<Node2D>("HealthBar");
		_staminaBar = healthStaminaBar.GetNode<TextureProgressBar>("Stamina");
		_healthBar = healthStaminaBar.GetNode<TextureProgressBar>("Health");
		_camera = GetNode<Camera2D>("Camera2D");

		// Establecer estadísticas iniciales
		CurrentHealth = MaxHealth;
		CurrentStamina = MaxStamina;
		UpdateHealthAndStaminaBars();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Regenerar resistencia
		RegenerateStamina();

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
		}
	}

	private void ProcessMovementState(double delta)
	{
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

	private bool CanAttack()
	{
		// Verificar si el personaje puede atacar
		return _currentState != CharacterState.Attacking && CurrentStamina >= AttackStaminaCost;
	}

	private void StartAttack()
	{
		// Iniciar ataque
		_currentState = CharacterState.Attacking;
		_currentAttackFrame = 0;
		CurrentStamina -= AttackStaminaCost;

		// Determinar animación del ataque según la última dirección
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

		// Desactivar todas las áreas de ataque después de finalizar el ataque
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
		// Desactivar todas las áreas antes de activar la correcta
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

	//areas de ataque
	private void _on_area_front_attack_body_entered(Node body){

	}
	private void _on_area_back_attack_body_entered(Node body){
		
	}

	private void _on_area_right_attack_body_entered(Node body){
		
	}

	private void _on_area_left_attack_body_entered(Node body){
		
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
		// Regenerar resistencia poco a poco
		CurrentStamina = Math.Min(CurrentStamina + 0.35f, MaxStamina);
		UpdateHealthAndStaminaBars();
	}

	private void UpdateHealthAndStaminaBars()
	{
		// Actualizar las barras de salud y resistencia
		_healthBar.Value = CurrentHealth;
		_staminaBar.Value = CurrentStamina;
	}

	public void TakeDamage(int damage)
	{
		// Reducir daño añadir defensa y más cosas
		CurrentHealth = Math.Max(0, CurrentHealth - damage);
		UpdateHealthAndStaminaBars();
		ShakeCameraOnHit();
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