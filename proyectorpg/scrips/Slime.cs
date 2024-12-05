using Godot;
using System;

public partial class Slime : Enemy
{
	// Arreglar, pasar cosas al padre y poner bonito

	private bool isAttacking = false;

	public override void _Ready()
	{
		// Llamar al método Ready del padre
		base._Ready();
		
		// Estadísticas específicas del Slime
		maxHealth = 100; 
		damage = 15;     
		speed = 30;    
		currentHealth = maxHealth;

		// Conectar la señal de animación terminada
		_animatedSprite.AnimationFinished += OnAttackAnimationFinished;
	}

	private void OnAttackAnimationFinished()
	{
		// Solución temporal
		if (isAttacking)
		{
			isAttacking = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Esto debería tenerlo el padre CAMBIAR
		healthBar.Value = currentHealth;

		if (characterChase && character != null)
		{
			Vector2 direction = (character.GlobalPosition - GlobalPosition).Normalized();
			
			// Determinar la dirección del sprite
			_animatedSprite.FlipH = (character.GlobalPosition.X - GlobalPosition.X) < 0;

			// Manejar los estados de movimiento y ataque
			if (attackArea)
			{
				HandleAttack(direction, delta);
			}
			else
			{
				HandleMovement(direction, delta);
			}

			MoveAndSlide();
		}
	}

	private void HandleMovement(Vector2 direction, double delta)
	{
		// Moverse hacia el personaje
		GlobalPosition += direction * speed * (float)delta;
		
		// Reproducir la animación de caminar solo si no está atacando
		if (!isAttacking)
		{
			_animatedSprite.Play("walking");
		}
	}

	private void HandleAttack(Vector2 direction, double delta)
	{
		// Iniciar la animación de ataque si no está atacando ya FUNCIONA MAL
		if (!isAttacking)
		{
			_animatedSprite.Play("attack");
			isAttacking = true;
		}

		// Verificar el ataque real
		float distance = GlobalPosition.DistanceTo(character.GlobalPosition);
		
		if (distance < ATTACK_DISTANCE && attackCooldown.IsStopped())
		{
			PerformAttack(direction);
		}
	}

	private void PerformAttack(Vector2 direction)
	{
		// Iniciar el cooldown para el ataque
		attackCooldown.Start();
		
		Vector2 knockbackDirection = direction.Normalized();
		
		if (character is Character characterBody)
		{
			// Aplicar retroceso y daño al character
			characterBody.Velocity = knockbackDirection * KNOCKBACK_FORCE;
			characterBody.TakeDamage(damage);
		}
	}
}
