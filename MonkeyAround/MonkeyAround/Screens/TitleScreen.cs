

using MugEngine.Scene;

namespace MonkeyAround;

class TitleScreen : MScreen
{
	SpriteFont mFont;
	SpriteFont mFontSmall;

	public TitleScreen(Point resolution) : base(resolution)
	{
		mFont = MData.I.Load<SpriteFont>("Fonts/Pixica36b");
		mFontSmall = MData.I.Load<SpriteFont>("Fonts/Pixica12");
	}

	public override void Update(MUpdateInfo info)
	{
		if(MugInput.I.ButtonPressed(GInput.Confirm))
		{
			MScreenManager.I.ActivateScreen(typeof(GameScreen));
		}
		base.Update(info);
	}

	public override void Draw(MDrawInfo info)
	{
		MDrawInfo canvasInfo = mCanvas.BeginDraw(info.mDelta);

		mCanvas.DrawRect(new Rectangle(-500, -500, 1000, 1000), new Color(152, 176, 57), Layer.BACKGROUND);
		mCanvas.DrawStringCentred(mFont, new Vector2(0.0f, -20.0f), new Color(15, 56, 15), "Monkeying", Layer.UI);
		mCanvas.DrawStringCentred(mFont, new Vector2(0.0f, 20.0f), new Color(15, 56, 15), "Around", Layer.UI);

		mCanvas.DrawStringCentred(mFontSmall, new Vector2(0.0f, 70.0f), new Color(15, 56, 15), "[WASD]/Arrows - Move", Layer.UI);
		mCanvas.DrawStringCentred(mFontSmall, new Vector2(0.0f, 85.0f), new Color(15, 56, 15), "[Space] - Grab", Layer.UI);
		mCanvas.DrawStringCentred(mFontSmall, new Vector2(0.0f, 100.0f), new Color(15, 56, 15), "Press [Enter] to begin", Layer.UI);

		mCanvas.EndDraw();
	}
}
