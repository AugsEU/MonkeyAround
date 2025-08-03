namespace MonkeyAround
{
	enum GInput
	{
		MoveLeft,
		MoveRight,
		Grab,
		Confirm,
	}

	static class InputConfig
	{
		public static void SetDefaultButtons()
		{
			// Arrow keys or WASD
			MugInput.I.BindButton(GInput.MoveLeft, new MKeyboardButton(Keys.Left), new MKeyboardButton(Keys.A));
			MugInput.I.BindButton(GInput.MoveRight, new MKeyboardButton(Keys.Right), new MKeyboardButton(Keys.D));

			// Jump
			MugInput.I.BindButton(GInput.Grab, new MKeyboardButton(Keys.Space));

			// Confirm
			MugInput.I.BindButton(GInput.Confirm, new MKeyboardButton(Keys.Enter));
		}
	}
}
