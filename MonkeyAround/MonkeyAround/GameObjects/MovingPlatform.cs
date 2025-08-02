
namespace MonkeyAround;

internal class MovingPlatform : MSSolid
{
	bool mForwards;
	Vector2 mStart;
	Vector2 mEnd;
	float mSpeed;
	float mProgress;

	public MovingPlatform(Vector2 pos, Point size, MCardDir dir, float distance, float speed)
	{
		mForwards = true;
		mStart = pos;
		mEnd = mStart +dir.ToVec() * distance;

		mSpeed = speed;
		mProgress = 0.0f;

		mSize = size;
		mPosition = pos;
	}

	public override void Update(MUpdateInfo info)
	{
		if(mForwards) // Going from start to end.
		{
			mProgress += info.mDelta * mSpeed;
			if(mProgress > 1.0f)
			{
				mForwards = false; // Start going other way.
				mProgress = 1.0f;
			}
		}
		else // Going from end to start
		{
			mProgress -= info.mDelta * mSpeed;
			if(mProgress < 0.0f)
			{
				mForwards = true; // Start going forwards again.
				mProgress = 0.0f;
			}
		}

		Vector2 nextPos = Vector2.Lerp(mStart, mEnd, mProgress);
		Move(nextPos-mPosition);
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawRect(BoundsRect(), Color.Beige, Layer.BACKGROUND);
	}
}
