
namespace MonkeyAround;

class Peg : MGameObject
{
	static MRandom rand = new MRandom();
	int mPegCount;
	MTexturePart[] mNumTextures;

	public Peg(Vector2 pos)
	{
		mPegCount = rand.GetIntRange(1, 3);
		mPosition = pos;
		mSize = new Point(16,16);

		Texture2D tex = MData.I.Load<Texture2D>("Pegs");
		mNumTextures = new MTexturePart[3];
		for(int i = 0; i < mNumTextures.Length; i++)
		{
			mNumTextures[i] = new MTexturePart(tex, i * 16, 0, 16, 16);
		}
	}

	public override void Update(MUpdateInfo info)
	{
	}

	public override void Draw(MDrawInfo info)
	{
		int idx = Math.Clamp(mPegCount, 1, mNumTextures.Length) - 1;
		info.mCanvas.DrawTexture(mNumTextures[idx], mPosition, Layer.ENTITY);
	}

	public int DecrementCount()
	{
		mPegCount--;

		return mPegCount;
	}
}
