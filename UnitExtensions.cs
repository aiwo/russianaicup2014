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
	}
}

