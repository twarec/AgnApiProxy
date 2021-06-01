using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using YG.Json;

namespace AgnApiProxy.States
{
	public class Applicarion : IClientBehavior
	{
		private bool _isPlay;
		private Data _data;

		/// <summary>
		/// Изменения активности программы
		/// </summary>
		public event Action<bool> OnChangePlay;
		/// <summary>
		/// Изменение данных gps
		/// </summary>
		public event Action<Data> OnChangeData;

		/// <summary>
		/// Активность программы
		/// </summary>
		public bool IsPlay => _isPlay;
		/// <summary>
		/// Gps данные программы
		/// </summary>
		public Data Gps => _data;

		public void Close()
		{
		}

		public void OnExcaption(Exception ex)
		{
		}

		public void OnMessage(JObject data)
		{
			data.ExecuteAllTocken((name, value) =>
			{
				switch (name)
				{
					case "GpsMessage":
						try
						{
							_data = value.ToObject<Data>();

							OnChangeData?.Invoke(_data);
						}
						catch (Exception ex)
						{
							YG.Console.WritelnError(ex.Message);
						}
						break;
					case "IsPlayMessage":
						try
						{
							_isPlay = value["IsPlay"].ToObject<bool>();

							OnChangePlay?.Invoke(_isPlay);
						}
						catch (Exception ex)
						{
							YG.Console.WritelnError(ex.Message);
						}
						break;
				}
			});
		}

		public void Open()
		{
		}

		public struct Data
		{
			public double Lat { get; set; }
			public double Long { get; set; }
			public double Alt { get; set; }
			public double Azimut { get; set; }
			public double PDop { get; set; }
			public double HDop { get; set; }
			public double Speed { get; set; }
			public int SatilatesCount { get; set; }
			public DateTime Time { get; set; }
		}

	}
}
