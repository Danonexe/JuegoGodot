using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Estados del enemigo
	private enum EnemyState
	{
		Idle,
		Chase,
		Attack,
		GetHit,
		Death
	}

	// Estadísticas del enemigo
	[Export] protected int MaxHealth { get; set; } = 100;
	[Export] protected int CurrentHealth { get; set; }
	[Export] protected int Damage { get; set; } = 10;
	[Export] protected float Speed { get; set; } = 30f;

	// Nodos
	protected AnimatedSprite2D _animatedSprite;
	protected TextureProgressBar _healthBar;
	protected Node2D _enemyHealthBar;
	protected Timer _attackCooldown;
	protected Timer _startDelay;
	protected Node _characterStats;

	// Estado actual
	private EnemyState _currentState = EnemyState.Idle;

	// Persecución y ataque
	protected Node2D _character = null;
	protected bool _gameStarted = false;
	protected float ATTACK_DISTANCE = 20.0f;
	protected const float KNOCKBACK_FORCE = 220.0f;

	// Variables de estado
	private int _currentAttackFrame = 0;
	private const int ATTACK_FRAMES = 30;
	private const int GET_HIT_FRAMES = 35;
	private int _currentGetHitFrame = 0;
	private const int DEATH_FRAMES = 35;
	private int _currentDEATHFrame = 0;

	public override void _Ready()
	{
		// Inicializar nodos
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_attackCooldown = GetNode<Timer>("attackCooldown");
		_startDelay = GetNode<Timer>("startDelay");
		
		// Obtener referencia a CharacterStats
		_characterStats = GetNode<Node>("/root/CharacterStats");
		
		_startDelay.Timeout += OnStartDelayTimeout;
		_startDelay.Start();

		// Inicializar salud
		CurrentHealth = MaxHealth;
		_enemyHealthBar = GetNode<Node2D>("EnemyHealthBar");
		_healthBar = _enemyHealthBar.GetNode<TextureProgressBar>("Health");
		_healthBar.Hide(); // Inicialmente oculta
	}

	public override void _PhysicsProcess(double delta)
	{
		// Actualizar barra de salud
		_healthBar.Value = CurrentHealth;

		// Manejar la visibilidad de la barra de salud
		UpdateHealthBarVisibility();

		// Manejar el estado actual
		switch (_currentState)
		{
			case EnemyState.Idle:
				ProcessIdleState(delta);
				break;
			case EnemyState.Chase:
				ProcessChaseState(delta);
				break;
			case EnemyState.Attack:
				ProcessAttackState(delta);
				break;
			case EnemyState.GetHit:
				ProcessGetHitState(delta);
				break;
			case EnemyState.Death:
				ProcessDeathState(delta);
				break;
		}
	}

	private void UpdateHealthBarVisibility()
	{
		// Mostrar barra de salud solo en Chase, Attack y GetHit
		switch (_currentState)
		{
			case EnemyState.Chase:
			case EnemyState.Attack:
			case EnemyState.GetHit:
				_healthBar.Show();
				break;
			case EnemyState.Idle:
			case EnemyState.Death:
				_healthBar.Hide();
				break;
		}
	}

	private void ProcessIdleState(double delta)
	{
		// Si el juego ha comenzado y hay un personaje detectado, cambiar a persecución
		if (_gameStarted && _character != null)
		{
			_currentState = EnemyState.Chase;
			_animatedSprite.Play("walking");
			_healthBar.Show();
		}
	}

	private void ProcessChaseState(double delta)
	{
		if (_character == null)
		{
			_currentState = EnemyState.Idle;
			_animatedSprite.Play("idle");
			_healthBar.Hide();
			return;
		}

		Vector2 direction = (_character.GlobalPosition - GlobalPosition).Normalized();
		
		// Determinar la dirección del sprite
		_animatedSprite.FlipH = (_character.GlobalPosition.X - GlobalPosition.X) < 0;

		// Verificar distancia de ataque
		float distance = GlobalPosition.DistanceTo(_character.GlobalPosition);
		if (distance <= ATTACK_DISTANCE)
		{
			_currentState = EnemyState.Attack;
			_currentAttackFrame = 0;
			return;
		}

		// Moverse hacia el personaje
		GlobalPosition += direction * Speed * (float)delta;
		_animatedSprite.Play("walking");
		MoveAndSlide();
	}

	private void ProcessAttackState(double delta)
	{
		_currentAttackFrame++;

		// Reproducir animación de ataque
		if (_currentAttackFrame == 1)
		{
			_animatedSprite.Play("attack");
		}

		// Realizar ataque
		if (_currentAttackFrame == ATTACK_FRAMES / 2 && _character != null)
		{
			PerformAttack();
		}

		// Finalizar estado de ataque
		if (_currentAttackFrame >= ATTACK_FRAMES)
		{
			_currentState = EnemyState.Chase;
		}
	}

	private void ProcessGetHitState(double delta)
	{
		_currentGetHitFrame++;

		// Reproducir animación de recibir golpe
		if (_currentGetHitFrame == 1)
		{
			_animatedSprite.Play("getHit");
		}

		// Finalizar estado de golpe
		if (_currentGetHitFrame >= GET_HIT_FRAMES)
		{
			_currentState = EnemyState.Chase;
			_currentGetHitFrame = 0;
		}
	}

	private void ProcessDeathState(double delta)
	{
	_currentDEATHFrame++;

   // Animación y muerte
	if (_currentDEATHFrame == 1)
	{
		_animatedSprite.Play("death");
	}

  
	if (_currentDEATHFrame >= DEATH_FRAMES)
	{
		QueueFree();
		_currentDEATHFrame = 0;
	}
}

	private void PerformAttack()
	{
		if (_character == null || !_attackCooldown.IsStopped()) return;

		_attackCooldown.Start();
		
		Vector2 direction = (_character.GlobalPosition - GlobalPosition).Normalized();
		Vector2 knockbackDirection = direction.Normalized();
		
		if (_character is Character characterBody)
		{
			// Aplicar retroceso y daño al personaje
			characterBody.Velocity = knockbackDirection * KNOCKBACK_FORCE;
			characterBody.TakeDamage(Damage);
		}
	}

	public virtual void TakeDamage(int damageAmount)
	{
		// Aplicar daño (en un futuro se podría considerar defensa del enemigo)
		CurrentHealth -= damageAmount;
		
		// Log para depuración
		GD.Print($"Enemigo recibió {damageAmount} de daño. Salud restante: {CurrentHealth}");

		if (CurrentHealth <= 0)
		{
			_currentDEATHFrame = 0;
			_currentState = EnemyState.Death;
		}else{
			// Cambiar al estado de recibir el golpe
			_currentState = EnemyState.GetHit;
			_currentGetHitFrame = 0;
		}
	}

	protected virtual void OnStartDelayTimeout()
	{
		_gameStarted = true;
	}

	// Detectar al personaje
	protected virtual void _on_detection_area_body_entered(Node body)
	{
		if (_gameStarted && body is Node2D && _character == null)
		{
			_character = (Node2D)body;
			if (_currentState != EnemyState.Death)
			{
				_currentState = EnemyState.Chase;
			}
		}
	}

	// Perder de vista
	protected virtual void _on_detection_area_body_exited(Node body)
	{
		if (body == _character)
		{
			_character = null;
			if (_currentState != EnemyState.Death)
			{
				_currentState = EnemyState.Idle;
			}
		}
	}
}