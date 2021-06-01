using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using YG.Json;

namespace AgnApiProxy.States
{
	public class Sections : IClientBehavior
	{
		private Section[] _sections;

		/// <summary>
		/// Изменение состояния секции
		/// </summary>
		public event Action<int, Section> OnChangeSection;
		/// <summary>
		/// Кол-во секций
		/// </summary>
		public int Count => _sections.Length;
		/// <summary>
		/// Получить секцию
		/// </summary>
		/// <param name="id">id секции</param>
		/// <returns></returns>
		public Section GetSection(int id)
		{
			if (id >= 0 && id < Count) return _sections[id];
			else return null;
		}

		public void Close()
		{
			
		}

		public void OnExcaption(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(ex);
			Console.ResetColor();
		}

		public void OnMessage(JObject data)
		{
			data.ExecuteAllTocken((name, value) =>
			{
				switch (name)
				{
					case "SectionChangeMessage":
						try
						{
							var id = value["Id"].ToObject<int>();
							var message = value.ToObject<Section>();

							_sections[id].IsOn = message.IsOn;

							OnChangeSection?.Invoke(id, _sections[id]);
						}
						catch (Exception ex)
						{
							YG.Console.WritelnError(ex.Message);
						}
						break;
					case "SectionChangeFreeze":
						try
						{
							var id = value["Id"].ToObject<int>();
							var message = value.ToObject<Section>();

							_sections[id].IsOn = message.IsOn;
							_sections[id].IsFreeze = message.IsFreeze;

							OnChangeSection?.Invoke(id, _sections[id]);

						}
						catch (Exception ex)
						{
							YG.Console.WritelnError(ex.Message);
						}
						break;
					case "SectionsDataMessage":
						try
						{
							var message = value["Sections"].ToObject<Section[]>();
							_sections = message;
							for (int i = 0; i < _sections.Length; i++)
							{
								OnChangeSection?.Invoke(i, _sections[i]);
							}
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

		public class Section
		{
			public bool IsOn { get; set; }
			public bool IsFreeze { get; set; }
			public float Lenght { get; set; }
			public int Injectors { get; set; }
		}
	}
}
