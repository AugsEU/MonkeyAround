using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyAround;

/// <summary>
/// Level with a single floor collider.
/// </summary>
class EmptyLevel : MLevel
{
	public EmptyLevel()
	{
	}

	public override void Update(MScene scene, MUpdateInfo info)
	{
	}

	public override void Draw(MScene scene, MDrawInfo info)
	{
		info.mCanvas.DrawRect(new Rectangle(-1000, -1000, 2000, 2000), new Color(152, 176, 57), Layer.BACKGROUND);
	}

	public override bool QueryCollides(Rectangle bounds, MCardDir travelDir, MCollisionFlags flags)
	{
		return false;
	}


}
