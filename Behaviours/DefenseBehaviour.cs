using System;
using System.Collections.Generic;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class DefenseBehaviour : HockeyistBehaviour
	{
		public DefenseBehaviour (Hockeyist h) : base(h)
		{

		}


		double defenseLine { get { return world.GetMyPlayer ().NetLeft - 30; } }

		public override IEnumerable<Action<Move>> Perform ()
		{
			while (true) {
				yield return move => {
					if (Math.Abs (me.X - defenseLine) > DistanceDelta || !me.InNetFrame ()) {
						var point = world.MyNetDefensePoint ();
						double angle = me.GetAngleTo (point.X, point.Y);
						move.SpeedUp = -1.0D + MathUtil.ReverseAngle (angle);
						move.Turn = MathUtil.ReverseAngle (angle);
					} else {
						move.Turn = me.GetAngleTo (world.Puck);
						move.Action = ActionType.TakePuck;
					}
				};
			}
		}
	}
}

