using System;
using System.Linq;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk {

    public sealed class MyStrategy : IStrategy {
//        private const double StrikeDistanceDelta = 50.0D;
//
//        private static World _world;
//        private static Hockeyist _self;
//        private static Game _game;
//        private static Move _move;
//
//        private static PID anglePID;
//        private static PID speedPID;
//
//        private static Point[] strikePoints;
//        private static Point[] attackPath;
//
//        private static double defenseLine;
//        private static double attackLine;

		static Dictionary<long, IEnumerator<Action<Move>>> behaviours = new Dictionary<long, IEnumerator<Action<Move>>>();

        public void Move(Hockeyist self, World world, Game game, Move move)
        {
			world.SetCurrent();
			game.SetCurrent();

			if (!behaviours.ContainsKey (self.Id)) { //do we need to init behaviour?
				HockeyistBehaviour b = null;
				if (self.IsHunter ()) {
					b = new HunterBehaviour (self);
				} else {
					b = new DefenseBehaviour (self);
				}
				behaviours[self.Id] = b.Perform().GetEnumerator();
			} 

			{ //take corresponding actions
				var enumerator = behaviours [self.Id];
				enumerator.MoveNext ();
				enumerator.Current (move);
				return;
			}

//            const double speedKp = 0.005;
//            speedPID = new PID(speedKp, 1.0D);

            // TODO: Angle Max Output
            /*
            const double angleKp = 0.5;
            anglePID = new PID(angleKp, 1.0D);
             */

//            strikeDistanceFromNet = 385;
//            var opponentNetFront = _world.GetOpponentPlayer().NetFront;
//            attackLine = opponentNetFront - strikeDistanceFromNet < 0
//                ? opponentNetFront + strikeDistanceFromNet
//                : opponentNetFront - strikeDistanceFromNet;

//            attackPath = new Point[]
//            {
//                new Point(700, 250),
//                new Point(400, 250), 
//            };
				
            
//            decelerateLine = 550;
//            minStrikeLine = 200;

//            if (IsHunter())
            {
//                PerformAttack();
                /*
                if (world.Puck.OwnerHockeyistId == self.Id)
                {
                    PassThePuck();
                }
                else
                {
                    GoGetThePuck();
                }  
                 */
            }
        }

//        private static double GetInterceptAngleToUnit(Unit unit)
//        {
//            double x = Math.Abs(_self.X - unit.X) * _self.SpeedX / (_self.SpeedX - unit.SpeedX);
//            double y = Math.Abs(_self.Y - unit.Y) * _self.SpeedY / (_self.SpeedY - unit.SpeedY);
//
//            return _self.GetAngleTo(-x, y);
//        }

//        private static void PassThePuck()
//        {
//            Hockeyist striker = _world.Hockeyists.First(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.Id != _self.Id);
//
//            double angle = _self.GetAngleTo(striker);
//            _move.Turn = angle;
//
//            if (Math.Abs(angle) < StrikeAngle)
//            {
//                _move.Action = ActionType.Strike;
//            }
//            /*
//            if (_world.Puck.OwnerHockeyistId == _self.Id)
//            {
//                PerformAttack();
//            }
//            else
//            {
//                PerformSupport();
//            }
//             */
//        }

//        private static void PerformSupport()
//        {
//			Hockeyist nearestOpponent = _self.NearestOpponent();
//            if (nearestOpponent != null)
//            {
//                if (_self.GetDistanceTo(nearestOpponent) > _game.StickLength)
//                {
//                    _move.SpeedUp = 1.0D;
//                }
//                else if (Math.Abs(_self.GetAngleTo(nearestOpponent)) < 0.5D * _game.StickSector)
//                {
//                    _move.Action = ActionType.Strike;
//                }
//                _move.Turn = _self.GetAngleTo(nearestOpponent);
//            }
//        }
    }

    
}