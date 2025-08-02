using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyAround;

/// <summary>
/// Level with a single floor collider.
/// </summary>
class SimpleLevel : MLevel
{
	Rectangle mCollider;

	public SimpleLevel(Rectangle r)
	{
		mCollider = r;
	}

	public override void Update(MScene scene, MUpdateInfo info)
	{
	}

	public override void Draw(MScene scene, MDrawInfo info)
	{
		info.mCanvas.DrawRect(mCollider, Color.AliceBlue, Layer.BACKGROUND);
	}

	public override bool QueryCollides(Rectangle bounds, MCardDir travelDir, MCollisionFlags flags)
	{
		return bounds.Intersects(mCollider);
	}


}
