using MugEngine.Scene;
using MugEngine.Screen;
using System.Linq;

namespace MonkeyAround;
internal class GameScreen : MScreen
{
	enum State
	{
		Game,
		GameOver
	}

	MScene mGameScene;
	MRandom mRand = new();
	MTimer mSpikeTimer = new MTimer();
	float mSpawnSpikeTime = 3.0f;
	State mState = State.Game;
	int mScore = 0;
	int mHighScore = 0;

	SpriteFont mUIFont;
	SpriteFont mGameOverFont;
	SpriteFont mGameOverFontSmall;

	SoundEffect mGameOverSFX;
	SoundEffectInstance mPlayingTheme = null;

	public GameScreen(Point resolution) : base(resolution)
	{
		CreateScene();
	}

	public override void OnActivate()
	{
		mUIFont = MData.I.Load<SpriteFont>("Fonts/PixicaMicro24b");
		mGameOverFont = MData.I.Load<SpriteFont>("Fonts/Pixica24");
		mGameOverFontSmall = MData.I.Load<SpriteFont>("Fonts/Pixica12");

		if(mPlayingTheme is null)
		{
			SoundEffect theme = MData.I.Load<SoundEffect>("Sounds/Theme");
			mPlayingTheme = theme.CreateInstance();
			mPlayingTheme.IsLooped = true;
			mPlayingTheme.Volume = 0.8f;
			mPlayingTheme.Play();
		}

		mGameOverSFX = MData.I.Load<SoundEffect>("Sounds/GameOver");

		mState = State.Game;
		mScore = 0;
		CreateScene();

		mCanvas.GetCamera().ClearMovements(true);

		base.OnActivate();
	}

	private void CreateScene()
	{
		mGameScene = new MScene();
		mGameScene.AddUnique(new MGameObjectManager());

		// Create obstacles
		mGameScene.GO.LoadLevel(new EmptyLevel());

		mGameScene.GO.Add(new Trampoline(new Vector2(-24.0f, 70.0f)));
		mGameScene.GO.Add(new Peg(new Vector2(-44.0f, -20.0f)));

		// Player
		mGameScene.GO.Add(new Player(new Vector2(-8.0f, -40.0f)));

		mSpikeTimer.Start();
	}

	public override void Update(MUpdateInfo info)
	{
		// Update the game's state.
		mGameScene.Update(info);
		mSpikeTimer.Update(info);

		MGameObjectManager go = mGameScene.GO;
		Player player = go.GetFirst<Player>();

		// Manage spawns
		if (mState == State.Game)
		{
			if (player is null) return;

			Vector2 playerPos = player.GetCentreOfMass();

			int numPegs = go.GetAll<Peg>().Count();
			int numTramps = go.GetAll<Trampoline>().Count();

			while (numPegs < 3)
			{
				SpawnPeg(playerPos);
				numPegs++;
			}

			if(numPegs < 5)
			{
				if(mRand.PercentChance(1.0f))
				{
					SpawnPeg(playerPos);
				}
			}

			if (numTramps == 0 && mRand.PercentChance(2.0f))
			{
				SpawnTrampoline(playerPos);
			}

			if (mSpikeTimer.GetElapsed() > mSpawnSpikeTime)
			{
				mSpawnSpikeTime = mRand.GetFloatRange(0.2f, 3.0f);
				mSpikeTimer.Start();
				Vector2 pos = new Vector2(mRand.GetFloatRange(-80.0f, 80.0f), -220.0f);
				mGameScene.GO.Add(new SpikeBall(pos));
			}
		}

		// Score
		mScore += player.GetScoreDelta();

		// Game over
		if (player.IsDead())
		{
			if(mState == State.Game)
			{
				mGameOverSFX.Play(1.0f, 0.2f, 0.0f);
				mCanvas.GetCamera().StartMovement(new MCameraShake(new Vector2(4.0f, 3.0f)), 1.0f);
				mState = State.GameOver;

				mHighScore = Math.Max(mHighScore, mScore);
			}
		}

		if(mState==State.GameOver)
		{
			if(MugInput.I.ButtonPressed(GInput.Confirm))
			{
				// Just reload this
				MScreenManager.I.ActivateScreen(typeof(GameScreen));
			}
		}

		base.Update(info);
	}

	void SpawnPeg(Vector2 playerPos)
	{
		Vector2 pos = FindPegPos(playerPos);

		int maxTries = 15;
		while(!ValidPegPos(playerPos, pos) && maxTries-- > 0)
		{
			pos = FindPegPos(pos);
		}

		if(maxTries > 0)
		{
			mGameScene.GO.Add(new Peg(pos));
		}
	}

	Vector2 FindPegPos(Vector2 playerPos)
	{
		const float Y_MAX = 100.0f;
		const float Y_MIN = -150.0f;

		const float X_MAX = 80.0f;
		const float X_MIN = -80.0f;

		bool belowPlayer = playerPos.Y < (Y_MAX - 20.0f) && mRand.PercentChance(70.0f);

		Vector2 spawnPos = Vector2.Zero;
		if (belowPlayer)
		{
			spawnPos.Y = mRand.GetFloatRange(playerPos.Y, Y_MAX);
		}
		else
		{
			spawnPos.Y = mRand.GetFloatRange(Y_MIN, Y_MAX);
		}

		spawnPos.X = mRand.GetFloatRange(X_MIN, X_MAX);

		return spawnPos;
	}

	bool ValidPegPos(Vector2 playerPos, Vector2 pos)
	{
		const float EXCLUDE_RADIUS = 40.0f;

		foreach(Peg peg in mGameScene.GO.GetAll<Peg>())
		{
			Vector2 com = peg.GetCentreOfMass();
			if((com - pos).Length() < EXCLUDE_RADIUS)
			{
				return false;
			}
		}

		if((playerPos - pos).Length() < EXCLUDE_RADIUS)
		{
			return false;
		}

		return true;
	}

	void SpawnTrampoline(Vector2 playerPos)
	{
		Vector2 pos = FindTrampolinePos(playerPos);

		int maxTries = 2;
		while (!ValidTrampolinePos(playerPos, pos) && maxTries-- > 0)
		{
			pos = FindTrampolinePos(pos);
		}

		if (maxTries > 0)
		{
			mGameScene.GO.Add(new Trampoline(pos));
		}
	}

	Vector2 FindTrampolinePos(Vector2 playerPos)
	{
		const float Y_MAX = 100.0f;
		const float Y_MIN = 0.0f;

		const float X_MAX = 30.0f;
		const float X_MIN = -80.0f;

		bool belowPlayer = playerPos.Y < (Y_MAX - 20.0f) && mRand.PercentChance(70.0f);

		Vector2 spawnPos = Vector2.Zero;
		if (belowPlayer)
		{
			spawnPos.Y = mRand.GetFloatRange(playerPos.Y, Y_MAX);
		}
		else
		{
			spawnPos.Y = mRand.GetFloatRange(Y_MIN, Y_MAX);
		}

		spawnPos.X = mRand.GetFloatRange(X_MIN, X_MAX);

		return spawnPos;
	}

	bool ValidTrampolinePos(Vector2 playerPos, Vector2 pos)
	{
		const float EXCLUDE_RADIUS = 50.0f;

		foreach (Peg peg in mGameScene.GO.GetAll<Peg>())
		{
			Vector2 com = peg.GetCentreOfMass();
			if ((com - pos).Length() < EXCLUDE_RADIUS)
			{
				return false;
			}
		}

		if ((playerPos - pos).Length() < EXCLUDE_RADIUS)
		{
			return false;
		}

		return true;
	}

	public override void Draw(MDrawInfo info)
	{
		MDrawInfo canvasInfo = mCanvas.BeginDraw(info.mDelta);

		// Draw the scene...
		mGameScene.Draw(canvasInfo);

		// Draw UI
		Color textColor = new Color(15, 56, 15);
		if(mState == State.Game)
		{
			mCanvas.DrawString(mUIFont, string.Format("Score: {0}", mScore), new Vector2(-80.0f, -150.0f), textColor, Layer.UI);
		}
		else if(mState == State.GameOver)
		{
			mCanvas.DrawStringCentred(mGameOverFont, new Vector2(0.0f, -100.0f), textColor, "GAME OVER", Layer.UI);
			mCanvas.DrawStringCentred(mGameOverFont, new Vector2(0.0f, 0.0f), textColor, string.Format("Score: {0}", mScore), Layer.UI);
			mCanvas.DrawStringCentred(mGameOverFontSmall, new Vector2(0.0f, 50.0f), textColor, string.Format("High Score: {0}", mHighScore), Layer.UI);
			mCanvas.DrawStringCentred(mGameOverFontSmall, new Vector2(0.0f, 100.0f), textColor, "Press [Enter] to retry", Layer.UI);
		}

		mCanvas.EndDraw();
	}
}
