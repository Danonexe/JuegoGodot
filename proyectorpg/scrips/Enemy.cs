using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Clase de la que van a heredar todos los enemigos
	// Estadísticas del enemigo
	protected int maxHealth = 100;
	protected int currentHealth;
	protected int damage = 10;
	protected float speed = 30f;
	protected AnimatedSprite2D _animatedSprite;

	// Persecución
	protected bool characterChase = false;
	protected Node2D character = null;
	protected bool gameStarted = false;
	protected bool attackArea = false;

	// Variables del ataque
	protected float ATTACK_DISTANCE = 40.0f;
	protected const float KNOCKBACK_FORCE = 220.0f;
	protected Timer attackCooldown;
	protected Timer startDelay;
	protected TextureProgressBar healthBar;
	private Node2D enemyHealthBar;

	public override void _Ready()
	{
		// Inicializar nodos
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		attackCooldown = GetNode<Timer>("attackCooldown");
		startDelay = GetNode<Timer>("startDelay");
		
		startDelay.Timeout += OnStartDelayTimeout;
		startDelay.Start();

		//Inicializar la health
		currentHealth = maxHealth;
		enemyHealthBar= GetNode<Node2D>("EnemyHealthBar");
		healthBar= enemyHealthBar.GetNode<TextureProgressBar>("Health");
		healthBar.Hide();
	}

	// Pequeño cooldown
	protected virtual void OnStartDelayTimeout()
	{
		gameStarted = true;
	}

	
	public virtual void TakeDamage(int damageAmount)
	{
		// Reducir daño añadir defensa y más cosas
		currentHealth -= damageAmount;
		
		if (currentHealth <= 0)
		{
			Die();
		}
	}

	protected virtual void Die()
	{
		_animatedSprite.Play("death");
		speed=0;
		//esperar 3 segundos AÑADIR CUANDO TENGA TIEMPO
		QueueFree();
	}

	// Detectar al character
	protected virtual void _on_detection_area_body_entered(Node body)
	{
		if (gameStarted && body is Node2D && character == null)
		{
			character = (Node2D)body;
			characterChase = true;
			_animatedSprite.Play("walking");
			healthBar.Show();
		}
	}

	// Perder de vista
	protected virtual void _on_detection_area_body_exited(Node body)
	{
		if (body == character)
		{
			character = null;
			characterChase = false;
			_animatedSprite.Play("idle");
			healthBar.Hide();
		}
	}

	// ARREGLAR LA ANIMACIÓN CUANDO TENGA TIEMPO
	// Area de ataque
	protected virtual void _on_attack_area_body_entered(Node body)
	{
		if (body == character)
		{
			attackArea = true;
		}
	}

	// Salir de area de ataque
	protected virtual void _on_attack_area_body_exited(Node body)
	{
		if (body == character)
		{
			attackArea = false;
		}
	}
}
