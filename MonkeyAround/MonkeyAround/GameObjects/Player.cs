
namespace MonkeyAround;

internal class Player : MSPlatformingActor
{
	const int PLAYER_SIZE = 16;
	const float PLAYER_SPEED = 150.0f;

	bool mGrabbing = false;
	Peg mGrabbedPeg = null;
	float mGrabPegAngle = 0.0f;
	float mGrabPegDist = 0.0f;
	bool mGrabClockwise = false;
	float mGrabInitAngle = 0.0f;
	float mGrabSpinSpeed = 0.0f;

	int mScoreDelta = 0;

	MAnimation mFallAnim;
	MAnimation mGrabAnim;
	Texture2D mHandTex;
	Texture2D mDeadTex;

	bool mDead = false;

	SoundEffect mBounceSFX;
	SoundEffect mSwingSFX;
	SoundEffect mJumpSFX;

	public Player(Vector2 pos) : base()
	{
		mPosition = pos;
		mSize = new Point(PLAYER_SIZE, PLAYER_SIZE);

		mFallAnim = MData.I.LoadAnimation("Monkey/Monkey_Fall.max");
		mGrabAnim = MData.I.LoadAnimation("Monkey/Monkey_Grab.max");

		mHandTex = MData.I.Load<Texture2D>("Monkey/Hand");
		mDeadTex = MData.I.Load<Texture2D>("Monkey/Dead");

		mBounceSFX = MData.I.Load<SoundEffect>("Sounds/Bounce");
		mSwingSFX = MData.I.Load<SoundEffect>("Sounds/VineSwing");
		mJumpSFX = MData.I.Load<SoundEffect>("Sounds/Jump");

	}

	public override void Update(MUpdateInfo info)
	{
		mFallAnim.Update(info);
		mGrabAnim.Update(info);

		mGrabbing = MugInput.I.ButtonDown(GInput.Grab);

		if(mDead)
		{

		}
		else if (mGrabbedPeg is not null)
		{
			mGrabPegAngle += (mGrabClockwise ? 1.0f : -1.0f) * mGrabSpinSpeed * info.mDelta;

			if (mGrabPegAngle > MathF.PI * 2.0f)
			{
				mGrabPegAngle -= MathF.PI * 2.0f;
			}
			else if(mGrabPegAngle < -MathF.PI * 2.0f)
			{
				mGrabPegAngle += MathF.PI * 2.0f;
			}

			Vector2 spinCentre = mGrabbedPeg.GetCentreOfMass();
			spinCentre += MugMath.FromAngle(mGrabPegAngle) * mGrabPegDist;

			SetCentreOfMass(spinCentre);

			if(!mGrabbing)
			{
				ReleaseFromPeg(mGrabbedPeg);
				mGrabbedPeg = null;
			}
		}
		else
		{
			float xVel = MathF.Abs(mVelocity.X);

			if(xVel < PLAYER_SPEED * 1.2f)
			{
				if (MugInput.I.ButtonDown(GInput.MoveLeft))
				{
					WalkIn(MWalkDir.Left, PLAYER_SPEED);
				}
				else if (MugInput.I.ButtonDown(GInput.MoveRight))
				{
					WalkIn(MWalkDir.Right, PLAYER_SPEED);
				}
				else
				{
					WalkIn(MWalkDir.None, 0.0f);
				}
			}
			else if(mVelocity.X > 0.0f) // Going right
			{
				if (MugInput.I.ButtonDown(GInput.MoveLeft))
				{
					WalkIn(MWalkDir.Left, PLAYER_SPEED);
				}

				mVelocity.X -= 1.0f * info.mDelta;
			}
			else // Going left
			{
				if (MugInput.I.ButtonDown(GInput.MoveRight))
				{
					WalkIn(MWalkDir.Right, PLAYER_SPEED);
				}
				mVelocity.X += 1.0f * info.mDelta;
			}

			if (mGrabbing)
			{
				Rectangle grabBox = CalcGrabBox();
				if(!GrabPegsInBox(grabBox))
				{
					GrabPegsInBox(BoundsRect());
				}
			}

			base.Update(info);
		}

		Vector2 com = GetCentreOfMass();
		if (com.Y > 170.0f)
		{
			Kill();
		}
		else if (com.X > 120.0f)
		{
			mPosition.X = 1.0f - mPosition.X;
		}
		else if (com.X < -120.0f)
		{
			mPosition.X = -BoundsRect().Width-2.0f - mPosition.X;
		}
	}

	bool GrabPegsInBox(Rectangle rect)
	{
		const float MIN_DIST = 32.0f;
		foreach (MGameObject go in GO().GetInRect(rect))
		{
			if (go is Peg peg)
			{
				mGrabbedPeg = peg;
				Vector2 pegToUs = GetCentreOfMass() - peg.GetCentreOfMass();
				if(pegToUs.LengthSquared() < MIN_DIST * MIN_DIST)
				{
					pegToUs.Normalize();
					SetCentreOfMass(peg.GetCentreOfMass() + pegToUs * MIN_DIST);
					pegToUs = pegToUs * MIN_DIST;
				}

				mGrabPegAngle = MathF.Atan2(pegToUs.Y, pegToUs.X);
				mGrabInitAngle = mGrabPegAngle;
				mGrabPegDist = pegToUs.Length();
				mGrabClockwise = GetFacingDir() == MWalkDir.Left ? true : false;

				mGrabSpinSpeed = (Math.Max(7.0f, mVelocity.Length() / mGrabPegDist)) * 0.75f;

				mSwingSFX.Play(0.6f, 1.0f, 0.0f);
				return true;
			}
		}

		return false;
	}

	void ReleaseFromPeg(Peg peg)
	{
		Vector2 pegCoM = peg.GetCentreOfMass();
		Vector2 toPeg = pegCoM - GetCentreOfMass();
		Vector2 shoot = toPeg.Perpendicular();
		shoot.Normalize();
		if (!mGrabClockwise) shoot = -shoot;
		float speed = MathF.Max(PLAYER_SPEED * 2.0f, mGrabSpinSpeed * mGrabPegDist * 1.30f);
		mVelocity = shoot * speed;

		int remaining = mGrabbedPeg.DecrementCount();
		if (remaining <= 0)
		{
			mGrabbedPeg.Kill();
		}

		// Add Score
		mScoreDelta += 1;

		mJumpSFX.Play(1.0f, 0.4f, 0.0f);

		mGrabbedPeg = null;
	}

	Rectangle CalcGrabBox()
	{
		const float BOX_DIST = 16.0f;
		const float BOX_SIZE = 6.0f;

		Vector2 facing = GetFacingDir().ToVec();
		Vector2 centrePos = GetCentreOfMass() + facing * BOX_DIST;
		centrePos.Y -= BOX_DIST;

		return MugMath.RectFromFloats(centrePos.X - BOX_SIZE, centrePos.Y - BOX_SIZE, BOX_SIZE * 2.0f, BOX_SIZE * 2.0f);
	}

	public override void OnHitSolid(MSSolid solid, MCardDir normal)
	{
		if(solid is Trampoline tramp)
		{
			float speed = MathF.Max(mVelocity.Y, 300.0f);
			Jump(speed);
			tramp.HitTrampoline();
			mBounceSFX.Play(1.0f, 0.4f, 0.0f);
		}
		else
		{
			base.OnHitSolid(solid, normal);
		}
	}

	public override void Kill()
	{
		mDead = true;
	}

	public bool IsDead()
	{
		return mDead;
	}

	public int GetScoreDelta()
	{
		int d = mScoreDelta;
		mScoreDelta = 0;
		return d;
	}

	public override void Draw(MDrawInfo info)
	{
		MAnimation anim = mFallAnim;
		if(mDead)
		{
			DrawPlatformer(info, new MTexturePart(mDeadTex), Layer.ENTITY);
			return;
		}
		else if(mGrabbedPeg is not null)
		{
			float ang = mGrabPegAngle - MathF.PI * 0.5f;
			while (ang < 0.0f) ang += MathF.PI * 2.0f;
			while (ang > MathF.PI * 2.0f) ang -= MathF.PI * 2.0f;

			if (GetFacingDir() == MWalkDir.Right)
			{
				ang = MathF.PI * 2.0f - ang;
			}

			float animQuad = (ang - 0.001f) * 8.0f / (MathF.PI * 2.0f);
			int animQuadInt = Math.Clamp((int)Math.Round(animQuad), 0, 7);

			MTexturePart part = mGrabAnim.GetTexture(animQuadInt);
			DrawPlatformer(info, part, Layer.ENTITY);
		}
		else
		{
			DrawPlatformer(info, mFallAnim.GetCurrentTexture(), Layer.ENTITY);
		}
		
		if(mGrabbedPeg is not null)
		{
			Vector2 com = GetCentreOfMass();
			Vector2 toPeg = mGrabbedPeg.GetCentreOfMass() - com;
			Vector2 toPegNorm = toPeg / toPeg.Length();

			Vector2 lineCenStart = com + toPegNorm * 8.0f;
			Vector2 lineCenEnd = lineCenStart + toPegNorm * (mGrabPegDist - 20.0f);

			Vector2 line1Start = lineCenStart + toPegNorm.Perpendicular() * 2.0f;
			Vector2 line1End = lineCenEnd + toPegNorm.Perpendicular() * 2.0f;

			Vector2 line2Start = lineCenStart - toPegNorm.Perpendicular() * 2.0f;
			Vector2 line2End = lineCenEnd - toPegNorm.Perpendicular() * 2.0f;

			info.mCanvas.DrawLine(line1Start, line1End, new Color(15, 56, 15), 1.0f, Layer.ENTITY);
			info.mCanvas.DrawLine(line2Start, line2End, new Color(15, 56, 15), 1.0f, Layer.ENTITY);
		}
		else if (mGrabbing)
		{
			Rectangle grabBox = CalcGrabBox();
			Vector2 pos = grabBox.Location.ToVector2();
			pos.X -= 2.0f;
			SpriteEffects effect = SpriteEffects.None;
			if(GetFacingDir() == MWalkDir.Left)
			{
				//pos.X -= mHandTex.Width;
				effect = SpriteEffects.FlipHorizontally;
			}
			info.mCanvas.DrawTexture(mHandTex, pos, effect, Layer.ENTITY);
			//info.mCanvas.DrawRect(CalcGrabBox(), Color.Red, Layer.ENTITY);
		}
	}
}
