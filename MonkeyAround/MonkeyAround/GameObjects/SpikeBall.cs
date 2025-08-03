
namespace MonkeyAround;

class SpikeBall : MSPlatformingActor
{
	MAnimation mAnim;

	public SpikeBall(Vector2 pos)
	{
		mAnim = MData.I.LoadAnimation("SpikeBall/SpikeBall_Idle.max");
		mPosition = pos;
		mSize = new Point(16, 16);
		mGravityStrength = 100.0f;
	}

	public override void Update(MUpdateInfo info)
	{
		mAnim.Update(info);

		Rectangle hitBox = BoundsRect();

		const int REDUCTION = 5;
		hitBox.X += REDUCTION;
		hitBox.Y += REDUCTION;
		hitBox.Width -= REDUCTION*2;
		hitBox.Height -= REDUCTION*2;

		if (mPosition.Y > -130.0f)
		{
			foreach (MGameObject go in GO().GetInRect(hitBox))
			{
				if (go is Player player)
				{
					player.Kill();
					break;
				}
			}
		}

		base.Update(info);
	}

	public override void OnHitSolid(MSSolid solid, MCardDir normal)
	{
		if (solid is Trampoline tramp)
		{
			float speed = MathF.Max(mVelocity.Y, 300.0f) * 0.5f;
			Jump(speed);
			tramp.HitTrampoline();
		}
		else
		{
			base.OnHitSolid(solid, normal);
		}
	}

	public override void Draw(MDrawInfo info)
	{
		DrawPlatformer(info, mAnim.GetCurrentTexture(), Layer.ENTITY);
	}
}
