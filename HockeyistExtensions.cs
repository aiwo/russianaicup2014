using System;
using System.Linq;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public static class HockeyistExtensions
	{
		public static bool InNetFrame(this Hockeyist me)
		{
			Player p = Get<World>.Current().GetMyPlayer();

			return me.Y < p.NetBottom && me.Y > p.NetTop;
		}

		public static bool IsHunter(this Hockeyist me)
		{
			var world = Get<World>.Current();
			var teammate = world.MyTeam().First(x => x.Id != me.Id);
			return world.Puck.OwnerHockeyistId == me.Id || me.GetDistanceTo(world.Puck) < teammate.GetDistanceTo(world.Puck);
		}

		public static Hockeyist NearestOpponent(this Hockeyist me)
		{
			var world = Get<World>.Current();
			Hockeyist nearestOpponent = null;
			double nearestOpponentRange = 0.0D;

			foreach (Hockeyist hockeyist in world.Hockeyists)
			{
				if (hockeyist.IsTeammate
					|| hockeyist.Type == HockeyistType.Goalie
					|| hockeyist.State == HockeyistState.KnockedDown
					|| hockeyist.State == HockeyistState.Resting)
				{
					continue;
				}

				double opponentRange = Hypot(me.X - hockeyist.X, me.Y - hockeyist.Y);

				if (nearestOpponent == null || opponentRange < nearestOpponentRange)
				{
					nearestOpponent = hockeyist;
					nearestOpponentRange = opponentRange;
				}
			}

			return nearestOpponent;
		}

		private static double Hypot(double x, double y)
		{
			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}
	}
}

