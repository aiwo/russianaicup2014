using System;
using System.Linq;
using Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeHockey2014.DevKit.CSharpCgdk
{
	public class PassThePuckBehaviour : HockeyistBehaviour
	{
		long teammateId;
		Hockeyist teammate {get {return world.MyTeam ().First (x => x.Id == teammateId);}} //TODO: generalize this id + getter logic

		public PassThePuckBehaviour (Hockeyist me, Hockeyist teammate) : base(me)
		{
			this.teammateId = teammate.Id;
		}
			
		public override IEnumerable<Action<Move>> Perform ()
		{
			while (Math.Abs (me.GetAdjustedAngleTo(teammate)) > (game.PassSector / 2)) {
				yield return move => {
					move.Turn = me.GetAdjustedAngleTo(teammate);
				};
			}

			yield return move => {
				move.PassAngle = me.GetAdjustedAngleTo(teammate);
				move.PassPower = 100500;
				move.Action = ActionType.Pass;
			};
		}
	}
}

