
namespace MonkeyAround;

internal class Player : MSPlatformingActor
{
	public Player(Vector2 pos) : base()
	{
		mPosition = pos;
		int size = (int)Tuning.I.Player.Size;
		mSize = new Point(size, size);
	}

	public override void Update(MUpdateInfo info)
	{
		int prevHeight = mSize.Y;
		int size = (int)Tuning.I.Player.Size;
		if(size > prevHeight)
		{
			mPosition.Y -= (size - prevHeight);
		}
		mSize = new Point(size, size);
		float speed = Tuning.I.Player.MoveSpeed;

		if(MugInput.I.ButtonDown(GInput.MoveLeft))
		{
			WalkIn(MWalkDir.Left, speed);
		}
		else if(MugInput.I.ButtonDown(GInput.MoveRight))
		{
			WalkIn(MWalkDir.Right, speed);
		}
		else
		{
			WalkIn(MWalkDir.None, 0.0f);
		}

		if (MugInput.I.ButtonPressed(GInput.Jump) && OnGround())
		{
			Jump(Tuning.I.Player.JumpHeight);
		}

		if(mPosition.Y > 100.0f)
		{
			MScreenManager.I.ActivateScreen(typeof(GameScreen));
		}

		base.Update(info);
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawRect(BoundsRect(), Color.Aqua, Layer.ENTITY);
	}
}
