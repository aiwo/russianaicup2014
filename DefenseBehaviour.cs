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


	    private double defenseLine
	    {
	        get
	        {
	            bool myFieldSideIsOnTheRight = world.GetMyPlayer().NetLeft < world.GetMyPlayer().NetRight;
	            const double defenseDistanceFromNet = 60;
                return myFieldSideIsOnTheRight ? world.GetMyPlayer().NetLeft - defenseDistanceFromNet : world.GetMyPlayer().NetRight + defenseDistanceFromNet;
	        }
	    }

	    Point myNetDefensePoint { 
			get
			{
			    bool holdTop = false;
			    double topSpace;
                double bottomSpace;
			    if (world.MyGoalie() != null)
			    {
                    topSpace = world.MyGoalie().Y - world.GetMyPlayer().NetTop;
                    bottomSpace = world.GetMyPlayer().NetBottom - world.MyGoalie().Y;
                    holdTop = topSpace > bottomSpace;         
			    }
			    else
			    {
			        topSpace = bottomSpace = (world.GetMyPlayer().NetTop - world.GetMyPlayer().NetBottom) / 2;
			    }
					
				if (holdTop)
					return new Point (defenseLine, world.GetMyPlayer().NetTop + (topSpace / 2));
				else
					return new Point (defenseLine, world.GetMyPlayer().NetBottom - (bottomSpace / 2));
			}
		}

		Point turnOverPoint {
			get {
				return new Point (defenseLine - 60 * Math.Sign(world.GetMyPlayer().NetFront - world.GetOpponentPlayer().NetFront), (world.GetMyPlayer ().NetTop + world.GetMyPlayer ().NetBottom) / 2);
			}
		}

		bool goalieBetweenMeAndDefensePoint {
			get
			{
			    if (world.MyGoalie() == null)
			        return false;
				var point = myNetDefensePoint;
				var goalieY = world.MyGoalie ().Y;
				return (me.Y < goalieY && goalieY < point.Y) || (me.Y > goalieY && goalieY > point.Y);
			}
		}

		bool meNearDefensePoint {
			get {
				return me.GetDistanceTo (myNetDefensePoint) < 45;
			}
		}

		bool canHuntThePuck {
			get {
				return me.GetDistanceTo (myNetDefensePoint) < 200;
			}
		}

		double distanceToPuck {
			get {
				return me.GetDistanceTo (world.Puck);
			}
		}

		public override IEnumerable<Action<Move>> Perform ()
		{
			while (true) {
				if (me.OwnsThePuck ()) {
					foreach (var action in passThePuck.Perform()) {
						yield return action;
					}
					continue;
				}

				if (distanceToPuck < 120) {
					yield return move => {
						move.Turn = me.GetAngleTo (world.Puck);
						move.Action = ActionType.TakePuck;
					};
					continue;
				}

				if (!meNearDefensePoint) {
					if (goalieBetweenMeAndDefensePoint) {
						foreach (var action in new ReachAndSlowdownBehaviour(me, turnOverPoint, true).Perform()) {
							yield return action;
						} 
					}

					foreach (var action in new ReachAndSlowdownBehaviour(me, myNetDefensePoint, true).Perform()) {
						yield return action;
					} 
				}

				yield return move => {
                    move.Action = ActionType.TakePuck;
					move.Turn = me.GetAdjustedAngleTo(world.Puck);
				};
			}
		}
	}
}

