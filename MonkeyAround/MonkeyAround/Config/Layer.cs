namespace MonkeyAround
{
	/// <summary>
	/// Drawing layers. Can be edited.
	/// </summary>
	static class Layer
	{
		public const int BACKGROUND = 0;
		public const int PARRALAX = 1;
		public const int TILEMAP = 2;
		public const int TILE_EFFECTS = 3;
		public const int ENTITY = 4;
		public const int ENTITY_EFFECTS = 5;
		public const int PARTICLE = 5;
		public const int UI = 6;
		public const int FRONT = 7;

		public const int MAX_LAYERS = 16;
	}



	/// <summary>
	/// Debug rectangle layers.
	/// </summary>
	static class DRectLayer
	{
		public const int DEFAULT = 0;
		public const int OTHER_RECT_LAYER = 1;

		public static void NameLayers()
		{
			MugDebug.SetDebugRectLayerName(DEFAULT, "Default");
			MugDebug.SetDebugRectLayerName(OTHER_RECT_LAYER, "Other Rect Layer");
		}
	}
}
