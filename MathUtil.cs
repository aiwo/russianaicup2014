using System;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public static class MathUtil
	{
		public static Double ReverseAngle(Double angle)
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
	}
}

