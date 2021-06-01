using System;
using System.Collections.Generic;
using System.Text;

namespace YG
{
	public static class Console
	{
		public static void WritelnError(object data)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
			System.Console.WriteLine(data);
			System.Console.ResetColor();
		}
	}
}
