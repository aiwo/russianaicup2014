using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class HunterBehaviour : HockeyistBehaviour
	{
		const Double attackLine = 400;
		public HunterBehaviour (Hockeyist h) : base(h)
		{
			strikePoints = new[]
			{
				new Point(attackLine, 650),
				new Point(attackLine, 250)
			};
		}

		private IEnumerable<Action<Move>> GoGetThePuck()
		{
			double distance = me.GetDistanceTo(world.Puck);

			var puck = world.Puck;

			double angleToPuck = me.GetAngleTo(puck);
			angleToPuck -= Math.Abs(angleToPuck) > Math.PI/2 ? Math.Sign(angleToPuck)*Math.PI/2 : 0;

			double x = puck.X + puck.SpeedX * Math.Abs(Math.Sin(angleToPuck) * me.SpeedX);
			double y = puck.Y + puck.SpeedY * Math.Abs(Math.Sin(angleToPuck) * me.SpeedY);

			double angle = me.GetAngleTo(x, y);

			//Console.WriteLine(angleToPuck - angle);

			yield return move => {
				move.Turn = angle;
				move.SpeedUp = 1.0D;

				move.Action = ActionType.TakePuck;
			};
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

		private bool StrikePositionTaken()
		{
			var point = CurrentStrikePosition();
			if (!point.Passed)
			{
				point.Passed = me.GetDistanceTo(point.X, point.Y) < DistanceDelta;
				//Console.WriteLine("SET PASSED TRUE");
			}
			return point.Passed;
		}

		private const double strikeDistanceFromNet = 385;
		private Point currentStrikePosition;
		private Point CurrentStrikePosition()
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

		Point[] strikePoints = null;
		private Point PickClosestStrikePoint()
		{
			double closestPoint = strikePoints.Min(x => me.GetDistanceTo(x.X, x.Y));
			return strikePoints.First(x => me.GetDistanceTo(x.X, x.Y) <= closestPoint);
		}

		private IEnumerable<Action<Move>> TakeStrikePosition()
		{
			var point = CurrentStrikePosition();
			if (point.Passed)
			{
				Console.WriteLine("WTF");
			}

			point.Passed = false;
			//Console.WriteLine("SET PASSED FALSE");
			double distance = me.GetDistanceTo(point.X, point.Y);

			double angle = me.GetAngleTo(point.X, point.Y);

			yield return move => {
				move.Turn = angle;
				//_move.SpeedUp = Math.Min(1.0D - angle, 1.0D - angle + speedPID.ValueForError(distance));
				move.SpeedUp = 1.0D - angle;
			};
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

		private const double StrikeAngle = 1.0D * Math.PI/180.0D;
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

				if (world.Puck.OwnerHockeyistId != me.Id)
				{
					foreach (var action in GoGetThePuck()) {
						yield return action;
					}
					continue;
				}

				if (!StrikePositionTaken() || TooClose())
				{
					foreach (var action in TakeStrikePosition()) {
						yield return action;
					}
					continue;
				}

				var point = world.NetStrikePoint();
				double angleToNet = me.GetAngleTo(point.X, point.Y);
				yield return move => {
					move.Turn = angleToNet;

					if (Math.Abs(angleToNet) < StrikeAngle)
					{
						move.Action = ActionType.Strike;
					}
				};
			}
		}
	}
}

