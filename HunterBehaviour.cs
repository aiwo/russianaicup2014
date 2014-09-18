using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class HunterBehaviour : HockeyistBehaviour
	{
		GetThePuckBehaviour getThePuck;
		ReachAndSlowdownBehaviour reach;

		const Double attackLine = 400;
		public HunterBehaviour (Hockeyist h) : base(h)
		{
			getThePuck = new GetThePuckBehaviour (h);
			strikePoints = new[]
			{
				new Point(attackLine, 650),
				new Point(attackLine, 250)
			};
		}

//		private bool StrikePositionTaken()
//		{
//			var point = CurrentStrikePosition();
//			if (!point.Passed)
//			{
//				point.Passed = me.GetDistanceTo(point.X, point.Y) < DistanceDelta;
//				//Console.WriteLine("SET PASSED TRUE");
//			}
//			return point.Passed;
//		}

		private const double strikeDistanceFromNet = 385;
		private Unit currentStrikePosition;
		private Unit CurrentStrikePosition()
		{
			if (Math.Abs(me.X - world.GetOpponentPlayer().NetFront) < strikeDistanceFromNet + 100 && currentStrikePosition != null)
			{
				Console.WriteLine("SELF X {0} NET FRONT {1}", me.X, world.GetOpponentPlayer().NetFront);
				return currentStrikePosition;
			}
				
			var hockeyists = world.EnemyTeam ().ToArray ();

			if (hockeyists.Average(x => x.X) > me.X)
			{
				return PickClosestStrikePoint();
			}

			double enemyPosition = hockeyists.Average(x => x.Y);
			double maxDistance = strikePoints.Select(strikePoint => Math.Abs(strikePoint.Y - enemyPosition)).Concat(new double[] {0}).Max();
			var point = strikePoints.First(strikePoint => Math.Abs(Math.Abs(strikePoint.Y - enemyPosition) - maxDistance) < 1);

			return currentStrikePosition = point;
		}

		Unit[] strikePoints = null;
		private Unit PickClosestStrikePoint()
		{
			double closestPoint = strikePoints.Min(x => me.GetDistanceTo(x.X, x.Y));
			return strikePoints.First(x => me.GetDistanceTo(x.X, x.Y) <= closestPoint);
		}
			
		private const double minStrikeLine = 200;
		private bool TooClose()
		{
			return me.X < minStrikeLine;
		}

		private const double decelerateLine = 550;
		private bool DecelerateLineHit()
		{
			return me.X < decelerateLine;
		}
			
		public override IEnumerable<Action<Move>> Perform ()
		{
			while (true) {
				if (me.State == HockeyistState.Swinging)
				{
					yield return move => {
						move.Action = ActionType.Strike;
					};
					continue;
				}
					
				foreach (var action in getThePuck.Perform()) {
					yield return action;
				}

				if (reach == null) {
					reach = new ReachAndSlowdownBehaviour (me, PickClosestStrikePoint (), false);
					foreach (var action in reach.Perform()) {
						yield return action;
					}
				}
					
				while (Math.Abs (me.GetAngleTo (world.NetStrikePoint ())) > (game.StrikeAngleDeviation)) {
					yield return move => {
						move.Turn = me.GetAngleTo (world.NetStrikePoint ());
					};
				}

				yield return move => {
					move.Action = ActionType.Strike;
				};

				reach = null;
			}
		}
	}
}

