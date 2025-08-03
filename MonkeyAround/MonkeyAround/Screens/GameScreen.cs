using MugEngine.Scene;
using MugEngine.Screen;

namespace MonkeyAround;
internal class GameScreen : MScreen
{
	MScene mGameScene;

	public GameScreen(Point resolution) : base(resolution)
	{
		CreateScene();
	}

	public override void OnActivate()
	{
		CreateScene();

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
	}

	public override void Update(MUpdateInfo info)
	{
		// Update the game's state.
		mGameScene.Update(info);

		base.Update(info);
	}

	public override void Draw(MDrawInfo info)
	{
		MDrawInfo canvasInfo = mCanvas.BeginDraw(info.mDelta);

		// Draw the scene...
		mGameScene.Draw(canvasInfo);

		MugDebug.FinalizeDebug(canvasInfo, Layer.FRONT);

		mCanvas.EndDraw();
	}
}
