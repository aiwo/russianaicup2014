using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class GetThePuckBehaviour : HockeyistBehaviour
	{
		public GetThePuckBehaviour (Hockeyist h) : base(h)
		{
		}

		public override IEnumerable<Action<Move>> Perform ()
		{
			while (world.Puck.OwnerHockeyistId != me.Id) {
				yield return move => {
					move.Turn = me.GetAdjustedAngleTo(world.Puck);
					move.SpeedUp = 1.0D;

					move.Action = ActionType.TakePuck;
				};
			}
			//			return;
			//
			//			if (_world.Tick % 2 == 0)
			//			{
			//				Hockeyist nearestOpponent = NearestOpponent(_self.X, _self.Y, _world);
			//				if (nearestOpponent != null)
			//				{
			//					if (_self.GetDistanceTo(nearestOpponent) <= _game.StickLength
			//						&& Math.Abs(_self.GetAngleTo(nearestOpponent)) < 0.5D * _game.StickSector)
			//					{
			//						_move.Action = ActionType.Strike;
			//					}
			//				}
			//			}
		}
	}
}

