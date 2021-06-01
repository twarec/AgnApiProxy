using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using YG.Json;

namespace AgnApiProxy.States
{
	public class Block : IClientBehavior
	{
		private Data _data;

		/// <summary>
		/// Событие изменения данных блока управления
		/// </summary>
		public Action<Data> OnChangeData;

		/// <summary>
		/// Данные блока управления
		/// </summary>
		public Data BlockData => _data;

		public void Close()
		{
		}

		public void OnExcaption(Exception ex)
		{
		}

		public void OnMessage(JObject data)
		{
			data.ExecuteAllTocken(
				(name, value) =>
				{
					switch (name)
					{
						case "BlockDataMessage":
							_data = value.ToObject<Data>();
							break;
					}
				});
		}

		public void Open()
		{
		}

		public struct Data
		{
			public float Pressure { get; set; }
			public float WaterLevel { get; set; }
			public float FlowRate { get; set; }
			public float Voltage { get; set; }
			public float Speed { get; set; }
		}

	}
}
