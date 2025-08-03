using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace MonkeyAround;

public class Main : MugMainGame
{
	public Main() : base(CreateGameSettings())
	{
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		Window.AllowUserResizing = true;
		Window.Title = "Monkeying Around";
		Content.RootDirectory = "@Data";
		

		base.Initialize();

		InputConfig.SetDefaultButtons();
		SetWindowSize(2.0f);

#if DEBUG
		DRectLayer.NameLayers();
		//InitTuner<Tune>(Tuning.I, "@Data/Tune/Values.xml");
#endif // DEBUG
	}

	private static MugEngineSettings CreateGameSettings()
	{
		MugEngineSettings settings = new MugEngineSettings();
		settings.mFPS = 60;
		settings.mNumLayers = Layer.MAX_LAYERS;
		settings.mResolution = new Point(200, 320);

		settings.mScreenTypes =
			[
				typeof(TitleScreen),
				typeof(GameScreen),
			];


		settings.mStartScreen = typeof(TitleScreen);

		return settings;
	}
}
