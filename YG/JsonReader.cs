using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YG.Json
{
	public class JsonReader
	{
		private string _message;
		private int _bc;

		public JObject AddData(char data)
		{
			if (_bc == 0 && data != '{')
				return null;
			_message += data;
			if (data == '{')
				_bc++;
			if (data == '}')
				_bc--;

			if (_bc == 0 && !string.IsNullOrWhiteSpace(_message))
			{
				var result = JObject.Parse(_message);
				_message = "";
				return result;
			}
			return null;
		}
		public JObject AddData(byte data) => AddData((char)data);
	}

	public static class JsonHelper
	{
		public static void ExecuteAllTocken(this JObject jObject, Action<string, JToken> action)
		{
			foreach(var entry in jObject)
				try
				{
					action?.Invoke(entry.Key, entry.Value);
				}catch(Exception ex)
				{
					YG.Console.WritelnError(ex.Message);
				}
		}
	}
}
