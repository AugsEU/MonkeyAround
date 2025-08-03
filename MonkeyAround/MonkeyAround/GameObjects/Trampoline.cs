
namespace MonkeyAround;

internal class Trampoline : MSSolid
{
	public Trampoline(Vector2 pos)
	{
		mPosition = pos;
		mSize = new Point(48, 8);
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawRect(BoundsRect(), Color.LightBlue, Layer.ENTITY);
	}

	public override void Update(MUpdateInfo info)
	{
	}
}
