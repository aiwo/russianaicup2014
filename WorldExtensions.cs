using System;
using System.Collections.Generic;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public static class WorldExtensions
	{
		public static IEnumerable<Hockeyist> MyTeam(this World world)
		{
			return world.Hockeyists.Where (x => x.IsTeammate && x.Type != HockeyistType.Goalie);
		}

		public static IEnumerable<Hockeyist> EnemyTeam(this World world)
		{
			return world.Hockeyists.Where (x => !x.IsTeammate && x.Type != HockeyistType.Goalie);
		}

		public static Point MyNetDefensePoint(this World _world)
		{
			Player player = _world.GetMyPlayer();

			Hockeyist goalie =
				_world.Hockeyists.FirstOrDefault(
					x => x.Type == HockeyistType.Goalie && x.PlayerId == player.Id);

			double netX = 0.5D * (player.NetBack + player.NetFront);
			double netY = 0.5D * (player.NetBottom + player.NetTop);

			if (goalie != null)
			{
				var game = Get<Game>.Current ();
				netY += (goalie.Y > netY ? 0.5D : -0.5D) * game.GoalNetHeight / 2;
			}

			return new Point(netX, netY);
		}

		public static Point NetStrikePoint(this World world)
		{
			Player player = world.GetOpponentPlayer();

			Hockeyist goalie =
				world.Hockeyists.FirstOrDefault(
					x => x.PlayerId == player.Id && x.Type == HockeyistType.Goalie);

			double netX = player.NetFront;//0.5D * (player.NetBack + player.NetFront);
			double netY = 0.5D * (player.NetBottom + player.NetTop);

			double puckSize = world.Puck.Radius + 5;

			if (goalie == null)
			{
				return new Point(netX, netY);
			}

			double strikeY = goalie.Y < netY ? player.NetBottom - puckSize : player.NetTop + puckSize;

			return new Point(netX, strikeY);
		}

		public static Hockeyist MyGoalie(this World world)
		{
			return world.Hockeyists.First (x => x.Type == HockeyistType.Goalie && x.IsTeammate);
		}

		public static Hockeyist EnemyGoalie(this World world)
		{
			return world.Hockeyists.First (x => x.Type == HockeyistType.Goalie && !x.IsTeammate);
		}
	}
}

