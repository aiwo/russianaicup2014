using System;
using System.Linq;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk {

    class Point
    {
        private const double MaxX = 1200;
        private const double MaxY = 800;

        private double _x;
        private double _y;

        public bool Passed = false;

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        public Point(double x, double y)
        {
            _x = Math.Min(x, MaxX);
            _y = Math.Min(y, MaxY);
        }

    }

    public sealed class MyStrategy : IStrategy {
        private const double StrikeAngle = 1.0D * Math.PI/180.0D;
        private const double StrikeDistanceDelta = 50.0D;
        private const double DistanceDelta = 80.0D;

        private static World _world;
        private static Hockeyist _self;
        private static Game _game;
        private static Move _move;

        private static PID anglePID;
        private static PID speedPID;

        private static Point[] strikePoints;
        private static Point[] attackPath;

        private static double defenseLine;
        private static double attackLine;
        private static double strikeDistanceFromNet;
        private static double decelerateLine;
        private static double minStrikeLine;

        private static Point currentStrikePosition;

        public void Move(Hockeyist self, World world, Game game, Move move)
        {
            _world = world;
            _self = self;
            _game = game;
            _move = move;

            const double speedKp = 0.005;
            speedPID = new PID(speedKp, 1.0D);

            // TODO: Angle Max Output
            /*
            const double angleKp = 0.5;
            anglePID = new PID(angleKp, 1.0D);
             */

            strikeDistanceFromNet = 385;
            var opponentNetFront = _world.GetOpponentPlayer().NetFront;
            attackLine = opponentNetFront - strikeDistanceFromNet < 0
                ? opponentNetFront + strikeDistanceFromNet
                : opponentNetFront - strikeDistanceFromNet;
            strikePoints = new[]
            {
                new Point(attackLine, 650),
                new Point(attackLine, 250)
            };

            attackPath = new Point[]
            {
                new Point(700, 250),
                new Point(400, 250), 
            };

            defenseLine = _world.GetMyPlayer().NetLeft - 30;
            attackLine = 400;
            decelerateLine = 550;
            minStrikeLine = 200;

            if (IsHunter())
            {
                if (self.State == HockeyistState.Swinging)
                {
                    move.Action = ActionType.Strike;
                    return;
                }

                PerformAttack();
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
            else
            {
                PerformDefense();
                //TakeStrikePosition();
            }
        }


#region Attack

        private static void PerformAttack()
        {
            if (_world.Puck.OwnerHockeyistId != _self.Id)
            {
                GoGetThePuck();
                return;
            }

            if (!StrikePositionTaken() || TooClose())
            {
                TakeStrikePosition();
                return;
            }

            var point = NetStrikePoint();
            double angleToNet = _self.GetAngleTo(point.X, point.Y);
            _move.Turn = angleToNet;
            
            if (Math.Abs(angleToNet) < StrikeAngle)
            {
                _move.Action = ActionType.Strike;
            }
        }

        private static void GoGetThePuck()
        {
            double distance = _self.GetDistanceTo(_world.Puck);
            
            var puck = _world.Puck;

            double angleToPuck = _self.GetAngleTo(puck);
            angleToPuck -= Math.Abs(angleToPuck) > Math.PI/2 ? Math.Sign(angleToPuck)*Math.PI/2 : 0;

            double x = puck.X + puck.SpeedX * Math.Abs(Math.Sin(angleToPuck) * _self.SpeedX);
            double y = puck.Y + puck.SpeedY * Math.Abs(Math.Sin(angleToPuck) * _self.SpeedY);

            double angle = _self.GetAngleTo(x, y);

            //Console.WriteLine(angleToPuck - angle);

            _move.Turn = angle;
            _move.SpeedUp = 1.0D;

            _move.Action = ActionType.TakePuck;
            return;

            if (_world.Tick % 2 == 0)
            {
                Hockeyist nearestOpponent = NearestOpponent(_self.X, _self.Y, _world);
                if (nearestOpponent != null)
                {
                    if (_self.GetDistanceTo(nearestOpponent) <= _game.StickLength
                        && Math.Abs(_self.GetAngleTo(nearestOpponent)) < 0.5D * _game.StickSector)
                    {
                        _move.Action = ActionType.Strike;
                    }
                }
            }
        }

        private static bool StrikePositionTaken()
        {
            var point = CurrentStrikePosition();
            if (!point.Passed)
            {
                point.Passed = _self.GetDistanceTo(point.X, point.Y) < DistanceDelta;
                //Console.WriteLine("SET PASSED TRUE");
            }
            return point.Passed;
        }

        private static void TakeStrikePosition()
        {
            var point = CurrentStrikePosition();
            if (point.Passed)
            {
                Console.WriteLine("WTF");
            }

            point.Passed = false;
            //Console.WriteLine("SET PASSED FALSE");
            double distance = _self.GetDistanceTo(point.X, point.Y);

            double angle = _self.GetAngleTo(point.X, point.Y);

            _move.Turn = angle;
            //_move.SpeedUp = Math.Min(1.0D - angle, 1.0D - angle + speedPID.ValueForError(distance));
            _move.SpeedUp = 1.0D - angle;
        }

        private static Point CurrentStrikePosition()
        {
            if (Math.Abs(_self.X - _world.GetOpponentPlayer().NetFront) < strikeDistanceFromNet + 100 && currentStrikePosition != null)
            {
                Console.WriteLine("SELF X {0} NET FRONT {1}", _self.X, _world.GetOpponentPlayer().NetFront);
                return currentStrikePosition;
            }

            Hockeyist[] hockeists = _world.Hockeyists.Where(x => !x.IsTeammate && x.Type != HockeyistType.Goalie).ToArray();

            if (hockeists.Sum(x => x.X) / hockeists.Length > _self.X)
            {
                return PickClosestStrikePoint();
            }

            double enemyPosition = hockeists.Sum(x => x.Y) / hockeists.Length;
            double maxDistance = strikePoints.Select(strikePoint => Math.Abs(strikePoint.Y - enemyPosition)).Concat(new double[] {0}).Max();


            var point = strikePoints.First(strikePoint => Math.Abs(Math.Abs(strikePoint.Y - enemyPosition) - maxDistance) < 1);

            return currentStrikePosition = point;
        }

        private static Point PickClosestStrikePoint()
        {
            double closestPoint = strikePoints.Min(x => _self.GetDistanceTo(x.X, x.Y));
            return strikePoints.First(x => _self.GetDistanceTo(x.X, x.Y) <= closestPoint);
        }

        private static bool TooClose()
        {
            return _self.X < minStrikeLine;
        }

        private static bool DecelerateLineHit()
        {
            return _self.X < decelerateLine;
        }

        #endregion

#region Defense

        private static void PerformDefense()
        {
            if (Math.Abs(_self.X - defenseLine) > DistanceDelta || !HockeistInNetFrame())
            {
                var point = MyNetDefensePoint();
                double angle = _self.GetAngleTo(point.X, point.Y);
                _move.SpeedUp = -1.0D + ReverseAngle(angle);
                _move.Turn = ReverseAngle(angle);
            }
            else
            {
                _move.Turn = _self.GetAngleTo(_world.Puck);
                _move.Action = ActionType.TakePuck;
            }
        }

        private static double ReverseAngle(double angle)
        {
            double reverseAngle = angle + Math.PI;
            while (reverseAngle > Math.PI)
            {
                reverseAngle -= 2.0D * Math.PI;
            }

            while (reverseAngle < -Math.PI)
            {
                reverseAngle += 2.0D * Math.PI;
            }
            return reverseAngle;
        }

        private static bool HockeistInNetFrame()
        {
            return _self.Y < _world.GetMyPlayer().NetBottom && _self.Y > _world.GetMyPlayer().NetTop;
        }

        #endregion

#region Utility

        private static Point MyNetDefensePoint()
        {
            Player player = _world.GetMyPlayer();

            Hockeyist goalie =
                _world.Hockeyists.FirstOrDefault(
                x => x.Type == HockeyistType.Goalie && x.PlayerId == player.Id);

            double netX = 0.5D * (player.NetBack + player.NetFront);
            double netY = 0.5D * (player.NetBottom + player.NetTop);

            if (goalie != null)
            {
                netY += (goalie.Y > netY ? 0.5D : -0.5D) * _game.GoalNetHeight / 2;
            }

            return new Point(netX, netY);
        }

        private static Point NetStrikePoint()
        {
            Player player = _world.GetOpponentPlayer();

            Hockeyist goalie =
                _world.Hockeyists.FirstOrDefault(
                x => x.PlayerId == player.Id && x.Type == HockeyistType.Goalie);

            double netX = player.NetFront;//0.5D * (player.NetBack + player.NetFront);
            double netY = 0.5D * (player.NetBottom + player.NetTop);

            double puckSize = _world.Puck.Radius + 5;

            if (goalie == null)
            {
                return new Point(netX, netY);
            }

            double strikeY = goalie.Y < netY ? player.NetBottom - puckSize : player.NetTop + puckSize;

            return new Point(netX, strikeY);
        }

        private static bool IsHunter()
        {
            Hockeyist teammate =
                _world.Hockeyists.First(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.Id != _self.Id);
            return _world.Puck.OwnerHockeyistId == _self.Id || _self.GetDistanceTo(_world.Puck) < teammate.GetDistanceTo(_world.Puck);
        }

        private static Hockeyist NearestOpponent(double x, double y, World world)
        {
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

                double opponentRange = Hypot(x - hockeyist.X, y - hockeyist.Y);

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

#endregion

        private static double GetInterceptAngleToUnit(Unit unit)
        {
            double x = Math.Abs(_self.X - unit.X) * _self.SpeedX / (_self.SpeedX - unit.SpeedX);
            double y = Math.Abs(_self.Y - unit.Y) * _self.SpeedY / (_self.SpeedY - unit.SpeedY);

            return _self.GetAngleTo(-x, y);
        }

        private static void PassThePuck()
        {
            Hockeyist striker = _world.Hockeyists.First(x => x.IsTeammate && x.Type != HockeyistType.Goalie && x.Id != _self.Id);

            double angle = _self.GetAngleTo(striker);
            _move.Turn = angle;

            if (Math.Abs(angle) < StrikeAngle)
            {
                _move.Action = ActionType.Strike;
            }
            /*
            if (_world.Puck.OwnerHockeyistId == _self.Id)
            {
                PerformAttack();
            }
            else
            {
                PerformSupport();
            }
             */
        }

        private static void PerformSupport()
        {
            Hockeyist nearestOpponent = NearestOpponent(_self.X, _self.Y, _world);
            if (nearestOpponent != null)
            {
                if (_self.GetDistanceTo(nearestOpponent) > _game.StickLength)
                {
                    _move.SpeedUp = 1.0D;
                }
                else if (Math.Abs(_self.GetAngleTo(nearestOpponent)) < 0.5D * _game.StickSector)
                {
                    _move.Action = ActionType.Strike;
                }
                _move.Turn = _self.GetAngleTo(nearestOpponent);
            }
        }
    }

    
}