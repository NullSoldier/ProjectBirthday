using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.Objects;

namespace ProjectB
{
	public static class FriendFactory
	{
		public static Friend GetJason ()
		{
			return new Friend
			{
				Messages = GetDefaultMessages(),
				ThanksMessage = "Thank you!",
				Color = getRandomColor()
			};
		}

		private static List<string> GetDefaultMessages()
		{
			return new List<string>
			{
				"Help me!"
			};
		}

		private static Random random;

		private static Color getRandomColor ()
		{
			if (random == null)
				random = new Random();

			return new Color (
				random.Next (0, 255),
				random.Next (0, 255),
				random.Next (0, 255));
		}
	}
}
