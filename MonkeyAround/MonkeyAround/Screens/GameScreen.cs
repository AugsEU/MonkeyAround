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
		mGameScene.GO.LoadLevel(new SimpleLevel(new Rectangle(-100, 100, 200, 200)));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(-200.0f, 90.0f), new Point(50, 10), MCardDir.Up, 60.0f, 1.0f));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(-100.0f, 30.0f), new Point(50, 10), MCardDir.Right, 60.0f, 1.0f));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(30.0f, 30.0f), new Point(30, 10), MCardDir.Up, 60.0f, 1.0f));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(-40.0f, -30.0f), new Point(10, 10), MCardDir.Left, 60.0f, 1.0f));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(-160.0f, -40.0f), new Point(10, 10), MCardDir.Up, 60.0f, 1.0f));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(-70.0f, -90.0f), new Point(5, 10), MCardDir.Up, 60.0f, 0.0f));
		mGameScene.GO.Add(new MovingPlatform(new Vector2(-20.0f, -100.0f), new Point(5, 10), MCardDir.Up, 60.0f, 0.0f));

		// Player
		mGameScene.GO.Add(new Player(new Vector2(0.0f, 40.0f)));

		// Win condition
		mGameScene.GO.Add(new WinDog(new Vector2(50.0f, -130.0f)));
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
