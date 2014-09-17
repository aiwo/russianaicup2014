using System;
using System.Collections.Generic;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class DefenseBehaviour : HockeyistBehaviour
	{
		PassThePuckBehaviour passThePuck;
		public DefenseBehaviour (Hockeyist h) : base(h)
		{
			passThePuck = new PassThePuckBehaviour (h, world.MyTeam().First (x => x.Id != h.Id));
		}


		double defenseLine { get { return world.GetMyPlayer ().NetLeft - 30; } }

		public override IEnumerable<Action<Move>> Perform ()
		{
			while (true) {
				if (me.OwnsThePuck ()) {
					foreach (var action in passThePuck.Perform()) {
						yield return action;
					}
					continue;
				}
				if (me.GetDistanceTo (world.Puck) < 120) {
					yield return move => {
						move.Turn = me.GetAngleTo (world.Puck);
						move.Action = ActionType.TakePuck;
					};
					continue;
				}

				yield return move => {
					var point = world.MyNetDefensePoint ();
					double angle = me.GetAngleTo (point.X, point.Y);
					move.SpeedUp = -1.0D + MathUtil.ReverseAngle (angle);
					move.Turn = MathUtil.ReverseAngle (angle);
				};
			}
		}
	}
}

