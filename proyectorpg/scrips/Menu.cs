using Godot;

public partial class Menu : Control
{
	protected Sprite2D SpriteFondo;
	private float _tiempoAcumulado = 0f; // Acumulador de tiempo
	private int _direccion = -1; // Dirección inicial

	public override void _Ready()
	{
		SpriteFondo = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		_tiempoAcumulado += (float)delta;

		// timer a mano
		if (_tiempoAcumulado >= 40f)
		{
			_tiempoAcumulado = 0f;
			_direccion *= -1; 
		}

		_MoverFondo(delta);
	}

	private void _MoverFondo(double delta)
	{
		// Modifica la posición en función de la dirección
		SpriteFondo.Position += new Vector2(100 * _direccion * (float)delta, 0);
	}
}
