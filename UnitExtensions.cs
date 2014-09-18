using System;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public static class UnitExtensions
	{
		public static double GetAdjustedAngleTo(this Unit me, Unit target)
		{
			double x = target.X + target.SpeedX;
			double y = target.Y + target.SpeedY;

			return me.GetAngleTo (x, y);
		}

        public static double GetAdjustedBackwardsAngleTo(this Unit me, Unit target)
        {
            double x = target.X + target.SpeedX;
            double y = target.Y + target.SpeedY;

            double angle = me.GetAngleTo(x, y) + Math.PI;

            while (angle > Math.PI)
            {
                angle -= 2.0D * Math.PI;
            }

            while (angle < -Math.PI)
            {
                angle += 2.0D * Math.PI;
            }

            return angle;
        }
	}
}

