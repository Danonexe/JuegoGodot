using Godot;

public partial class AttackLabel : Label
{
	private Node characterStats;

	public override void _Ready()
	{
		// Obtener referencia al singleton
		characterStats = GetNode<Node>("/root/CharacterStats");
	}

	public override void _Process(double delta)
	{
		// Actualizar el texto cada frame
		if (characterStats != null)
		{
			var attack = (int)characterStats.Get("attack_damage");
			Text = $"{attack}";
		}
	}
}
