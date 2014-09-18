using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class ReachAndSlowdownBehaviour : HockeyistBehaviour
	{
		Unit target;
	    bool backwardsMovement;
		public ReachAndSlowdownBehaviour (Hockeyist me, Unit target, bool backwardsMovement) : base(me)
		{
			this.target = target;
		    this.backwardsMovement = backwardsMovement;
		}

		public override IEnumerable<Action<Move>> Perform ()
		{
			double distance = 0;
			do {
				distance = me.GetDistanceTo (target);
				var distanceToPerformFullStop = MathUtil.Hypot (me.SpeedX, me.SpeedY) / game.HockeyistSpeedDownFactor;

				while (me.GetAdjustedAngleTo(target) > Math.PI)
					yield return move => {
						move.Turn = me.GetAdjustedAngleTo(target);
					};

				if (distance < distanceToPerformFullStop)
				{
					yield return move => {
						move.SpeedUp = backwardsMovement ? -1 : 1;
                        move.Turn = backwardsMovement ?  me.GetAdjustedBackwardsAngleTo(target) : me.GetAdjustedAngleTo(target);
					};
					continue;
				}
				yield return move => { //full sail backwards, matey!
                    move.SpeedUp = backwardsMovement ? -1 : 1;
                    move.Turn = backwardsMovement ? me.GetAdjustedBackwardsAngleTo(target) : me.GetAdjustedAngleTo(target);
				};

			} while (distance > 60);
		}
	}
}

