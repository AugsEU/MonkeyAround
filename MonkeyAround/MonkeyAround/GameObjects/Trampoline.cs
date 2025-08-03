
namespace MonkeyAround;

internal class Trampoline : MSSolid
{
	MAnimation mWaitAnim;
	MAnimation mBounceAnim;
	bool mBeenHit;

	public Trampoline(Vector2 pos)
	{
		mPosition = pos;
		mSize = new Point(48, 8);

		mWaitAnim = MData.I.LoadAnimation("Trampoline/Trampoline_Wait.max");
		mBounceAnim = MData.I.LoadAnimation("Trampoline/Trampoline_Bounce.max");
		mBounceAnim.Stop();
	}

	public override void Draw(MDrawInfo info)
	{
		if (mBeenHit)
		{
			info.mCanvas.DrawTexture(mBounceAnim.GetCurrentTexture(), mPosition, Layer.ENTITY);
		}
		else
		{
			info.mCanvas.DrawTexture(mWaitAnim.GetCurrentTexture(), mPosition, Layer.ENTITY);
		}
	}

	public void HitTrampoline()
	{
		if (mBeenHit) return;

		mBeenHit = true;
		mBounceAnim.Play();
		mSize = Point.Zero;
	}

	public override void Update(MUpdateInfo info)
	{
		mBounceAnim.Update(info);
		if(mBeenHit && !mBounceAnim.IsPlaying())
		{
			Kill();
		}
	}
}
