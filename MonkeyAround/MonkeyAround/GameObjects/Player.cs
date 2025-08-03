
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

	public Player(Vector2 pos) : base()
	{
		mPosition = pos;
		mSize = new Point(PLAYER_SIZE, PLAYER_SIZE);
	}

	public override void Update(MUpdateInfo info)
	{
		mGrabbing = MugInput.I.ButtonDown(GInput.Grab);

		if (mGrabbedPeg is not null)
		{
			float prevAngle = mGrabPegAngle;
			mGrabPegAngle += (mGrabClockwise ? 1.0f : -1.0f) * mGrabSpinSpeed * info.mDelta;

			if((prevAngle > mGrabInitAngle && mGrabInitAngle > mGrabPegAngle) || 
				(mGrabPegAngle > mGrabInitAngle && mGrabInitAngle > prevAngle))
			{
				int remaining = mGrabbedPeg.DecrementCount();
				if(remaining <= 0)
				{
					ReleaseFromPeg(mGrabbedPeg);
					mGrabbedPeg.Kill();
					mGrabbedPeg = null;
					return;
				}

				// Add score..
			}

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
			MScreenManager.I.ActivateScreen(typeof(GameScreen)); // Reload on death.
		}
		else if (com.X > 120.0f)
		{
			mPosition.X = 1.0f - mPosition.X;
		}
		else if (com.X < -120.0f)
		{
			mPosition.X = -1.0f - mPosition.X;
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

				mGrabSpinSpeed = Math.Max(7.0f, mVelocity.Length() / mGrabPegDist);
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
	}

	Rectangle CalcGrabBox()
	{
		const float BOX_DIST = 18.0f;
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
			Jump(300.0f);
		}
		else
		{
			base.OnHitSolid(solid, normal);
		}
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawRect(BoundsRect(), Color.Aqua, Layer.ENTITY);

		if (mGrabbing)
		{
			info.mCanvas.DrawRect(CalcGrabBox(), Color.Red, Layer.ENTITY);
		}
	}
}
