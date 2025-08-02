
namespace MonkeyAround;

internal class WinDog : MGameObject
{
	Texture2D mTexture;

	public WinDog(Vector2 pos)
	{
		mPosition = pos;
		mTexture = MData.I.Load<Texture2D>("HotDog");
		mSize = mTexture.Bounds.Size;
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawTexture(mTexture, mPosition, Layer.ENTITY);
	}

	public override void Update(MUpdateInfo info)
	{
		foreach(MGameObject touchingObject in GetTouching())
		{
			if(touchingObject is Player)
			{
				// WIN
				MScreenManager.I.ActivateScreen(typeof(WinScreen));
			}
		}
	}
}
