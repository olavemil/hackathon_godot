using Godot;

public class Player : Area2D
{
	[Signal]
	public delegate void Hit();
	
	[Export]
	public int Speed = 400;
	public Vector2 ScreenSize;
	public Vector2 Velocity;
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		var impulse = Vector2.Zero;
		if (Input.IsActionPressed("move_right")) {
			impulse.x += 1.0f;
		}
		if (Input.IsActionPressed("move_left")) {
			impulse.x -= 1.0f;
		}
		if (Input.IsActionPressed("move_up")) {
			impulse.y -= 1.0f;
		}
		if (Input.IsActionPressed("move_down")) {
			impulse.y += 1.0f;
		}
		impulse = impulse.Normalized();
		Velocity += impulse * Speed * delta - Velocity * delta * 2;
		
		var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		if (Velocity.Length() < 0.5f) {
			Velocity = Vector2.Zero;
			animatedSprite.Stop();
		} else {
			Velocity = Velocity.Clamped(Speed);
			animatedSprite.Play();
			if (Mathf.Abs(Velocity.x) > 1.5 * Mathf.Abs(Velocity.y)) {
				animatedSprite.Animation = "walk";
				animatedSprite.FlipH = Velocity.x < 0;
				animatedSprite.FlipV = Velocity.y > 0;
			} else {
				animatedSprite.Animation = "up";
				animatedSprite.FlipH = Velocity.x < 0;
				animatedSprite.FlipV = Velocity.y > 0;
			}
		}

		Position += Velocity * delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.x, 0, ScreenSize.x),
			y: Mathf.Clamp(Position.y, 0, ScreenSize.y)
		);
  	}

	private void OnPlayerBodyEntered(PhysicsBody2D body)
	{
		Hide();
		EmitSignal(nameof(Hit));
		GetNode("CollisionShape2D").SetDeferred("disabled", true);
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		Show();
	}
}

