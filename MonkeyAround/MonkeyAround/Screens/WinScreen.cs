

using MugEngine.Scene;

namespace MonkeyAround;

class WinScreen : MScreen
{
	SpriteFont mFont;

	public WinScreen(Point resolution) : base(resolution)
	{
		mFont = MData.I.Load<SpriteFont>("Fonts/Arial12");
	}

	public override void Update(MUpdateInfo info)
	{
		if(MugInput.I.ButtonPressed(GInput.Confirm))
		{
			MScreenManager.I.ActivateScreen(typeof(TitleScreen));
		}
		base.Update(info);
	}

	public override void Draw(MDrawInfo info)
	{
		MDrawInfo canvasInfo = mCanvas.BeginDraw(info.mDelta);

		mCanvas.DrawRect(new Rectangle(-500, -500, 1000, 1000), Color.Black, Layer.BACKGROUND);
		mCanvas.DrawStringCentred(mFont, Vector2.Zero, Color.White, "YOU WIN.", Layer.UI);


		mCanvas.EndDraw();
	}
}
