using Godot;
using System;

public partial class Goblin : Enemy
{
	// Áreas de ataque específicas del Goblin
	private Area2D _attackAreaRight;
	private Area2D _attackAreaLeft;
	
	// Variables propias del Goblin para el sistema de ataque
	private int _goblinAttackFrame = 0;
	private bool _goblinAttacking = false;
	private const int GOBLIN_ATTACK_FRAMES = 45; // Más frames para animación completa
	private const float GOBLIN_ATTACK_COOLDOWN = 2.5f; // Más tiempo entre ataques

	public override void _Ready()
	{
		// Llamar al método Ready del padre
		base._Ready();
		
		// Estadísticas específicas del Goblin
		MaxHealth = 100;  // Misma vida que el Slime
		Damage = 25;      // Más daño que el Slime (que tiene 15)
		Speed = 25f;      // Un poco más lento que el Slime
		CurrentHealth = MaxHealth;
		
		// Experiencia y puntuación del Goblin (más que el Slime)
		ExperienceValue = 15;
		ScoreValue = 150;
		
		// Obtener las áreas de ataque usando los nombres correctos
		_attackAreaRight = GetNode<Area2D>("AttackAreaRight");
		_attackAreaLeft = GetNode<Area2D>("AttackAreaLeft");
		
		// Asegurarse de que estén desactivadas inicialmente
		_attackAreaRight.Monitoring = false;
		_attackAreaLeft.Monitoring = false;
		
		// Configurar el cooldown de ataque más largo
		_attackCooldown.WaitTime = GOBLIN_ATTACK_COOLDOWN;
		
		// Conectar las señales de las áreas de ataque
		_attackAreaRight.BodyEntered += _on_attack_area_right_body_entered;
		_attackAreaLeft.BodyEntered += _on_attack_area_left_body_entered;
		
		GD.Print("Goblin: Inicializado con vida:", MaxHealth, " daño:", Damage, " cooldown:", GOBLIN_ATTACK_COOLDOWN);
	}

	public override void _PhysicsProcess(double delta)
	{
		// Llamar al proceso físico del padre
		base._PhysicsProcess(delta);
		
		// Manejar el sistema de ataque propio del Goblin
		if (_goblinAttacking)
		{
			ProcessGoblinAttack(delta);
		}
	}

	private void ProcessGoblinAttack(double delta)
	{
		_goblinAttackFrame++;
		
		// Activar área de ataque en la mitad de la animación
		if (_goblinAttackFrame == GOBLIN_ATTACK_FRAMES / 2)
		{
			ActivateDirectionalAttack();
		}
		
		// Desactivar área de ataque después de un momento
		if (_goblinAttackFrame == (GOBLIN_ATTACK_FRAMES / 2) + 5)
		{
			DeactivateAllAttackAreas();
		}
		
		// Finalizar ataque cuando la animación esté completa
		if (_goblinAttackFrame >= GOBLIN_ATTACK_FRAMES)
		{
			_goblinAttacking = false;
			_goblinAttackFrame = 0;
			GD.Print("Goblin: Animación de ataque completada");
		}
	}
	
	private void ActivateDirectionalAttack()
	{
		if (_character == null) return;
		
		// Determinar dirección del personaje
		Vector2 directionToPlayer = (_character.GlobalPosition - GlobalPosition).Normalized();
		bool playerOnRight = directionToPlayer.X > 0;
		
		// Desactivar todas las áreas primero
		DeactivateAllAttackAreas();
		
		// Activar el área correspondiente
		if (playerOnRight)
		{
			_attackAreaRight.Monitoring = true;
			GD.Print("Goblin: Activando ataque hacia la DERECHA");
		}
		else
		{
			_attackAreaLeft.Monitoring = true;
			GD.Print("Goblin: Activando ataque hacia la IZQUIERDA");
		}
	}
	
	private void DeactivateAllAttackAreas()
	{
		_attackAreaRight.Monitoring = false;
		_attackAreaLeft.Monitoring = false;
	}

	// Sobrescribir PerformAttack para usar nuestro sistema
	protected override void GiveRewardsToPlayer()
	{
		// Interceptar cuando el Enemy va a atacar y usar nuestro sistema
		if (_character == null || !_attackCooldown.IsStopped() || _goblinAttacking) 
		{
			// Llamar al método padre si no podemos atacar
			base.GiveRewardsToPlayer();
			return;
		}

		// Si podemos atacar, usar nuestro sistema personalizado
		StartGoblinAttack();
	}

	private void StartGoblinAttack()
	{
		// Iniciar el sistema de ataque personalizado del Goblin
		_goblinAttacking = true;
		_goblinAttackFrame = 0;
		_attackCooldown.Start();
		
		// Determinar dirección y reproducir animación
		Vector2 directionToPlayer = (_character.GlobalPosition - GlobalPosition).Normalized();
		bool playerOnRight = directionToPlayer.X > 0;
		
		_animatedSprite.Play(playerOnRight ? "attackRight" : "attackLeft");
		
		GD.Print($"Goblin: Iniciando ataque hacia {(playerOnRight ? "DERECHA" : "IZQUIERDA")}");
	}

	// Método para manejar el daño cuando las áreas detectan al personaje
	private void _on_attack_area_right_body_entered(Node body)
	{
		if (body is Character character)
		{
			DealDamageToPlayer(character);
			GD.Print("Goblin: Golpe conectado desde la DERECHA");
		}
	}

	private void _on_attack_area_left_body_entered(Node body)
	{
		if (body is Character character)
		{
			DealDamageToPlayer(character);
			GD.Print("Goblin: Golpe conectado desde la IZQUIERDA");
		}
	}
	
	private void DealDamageToPlayer(Character character)
	{
		// Aplicar retroceso y daño
		Vector2 direction = (character.GlobalPosition - GlobalPosition).Normalized();
		Vector2 knockbackDirection = direction.Normalized();
		
		character.Velocity = knockbackDirection * KNOCKBACK_FORCE;
		character.TakeDamage(Damage);
		
		GD.Print($"Goblin: Daño aplicado: {Damage}");
	}

	// Sobrescribir las funciones de detección para usar los nombres correctos
	protected override void _on_detection_area_body_entered(Node body)
	{
		base._on_detection_area_body_entered(body);
		GD.Print("Goblin: Jugador detectado en área de detección");
	}

	protected override void _on_detection_area_body_exited(Node body)
	{
		base._on_detection_area_body_exited(body);
		GD.Print("Goblin: Jugador salió del área de detección");
	}
}
