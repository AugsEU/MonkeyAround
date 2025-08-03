
namespace MonkeyAround;

class Peg : MGameObject
{
	static MRandom rand = new MRandom();
	int mPegCount;

	public Peg(Vector2 pos)
	{
		mPegCount = rand.GetIntRange(2, 4);
		mPosition = pos;
		mSize = new Point(16,16);
	}

	public override void Update(MUpdateInfo info)
	{
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawRect(BoundsRect(), Color.Yellow, Layer.ENTITY);
	}

	public int DecrementCount()
	{
		mPegCount--;

		return mPegCount;
	}
}
