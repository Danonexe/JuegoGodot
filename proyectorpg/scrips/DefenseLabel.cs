using Godot;

public partial class DefenseLabel : Label
{
	private Node characterStats;

	public override void _Ready()
	{
		characterStats = GetNode<Node>("/root/CharacterStats");
	}

	public override void _Process(double delta)
	{
		if (characterStats != null)
		{
			var defense = (int)characterStats.Get("defense");
			Text = $"{defense}";
		}
	}
}
