using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AgnApiProxy
{
	public static class Client
	{
		private static List<IClientBehavior> _behaviors = new List<IClientBehavior>();

		private static TcpClient _client;
		private static bool _isOpen;
		private static Stream _stream;
		private static byte[] _buffer = new byte[10240];

		public static event Action OnServerConnect;
		public static event Action OnServerDisconect;
		public static event Action<Exception> OnExcation;
		public static event Action<JObject> OnMessage;


		public static void Start(string ip, int port)
		{
			try
			{
				if (IPAddress.Parse(ip) != null)
				{
				
					_client = new TcpClient(ip, port);
					_stream = _client.GetStream();
					OnServerConnect?.Invoke();
					_isOpen = true;
					
					
				}
			}
			catch (System.InvalidOperationException ex)
			{
				Console.WriteLine(ex);
			}
			

			RunRead();
		}

		private static async void RunRead()
		{
			var jr = new YG.Json.JsonReader();
			while (_isOpen)
			{
				try
				{
					int count = await _stream.ReadAsync(_buffer, 0, _buffer.Length);

					for(int i = 0; i < count; i++)
					{
						var data = jr.AddData(_buffer[i]);
						if (data != null)
						{
							//Console.WriteLine(data);
							OnMessage?.Invoke(data);
						}
					}					

				} catch (Exception ex)
				{
					OnExcation?.Invoke(ex);
				}
			}
			

		}

		public static void Stop()
		{
			_isOpen = false;

			_client.GetStream().Close();
			_client.Close();

			OnServerDisconect?.Invoke();
		}

		public static void AddOption(IClientBehavior behavior)
		{
			_behaviors.Add(behavior);
			OnExcation += behavior.OnExcaption;
			OnMessage += behavior.OnMessage;
			OnServerConnect += behavior.Open;
			OnServerDisconect += behavior.Close;
		}
		public static void AddOptions(params IClientBehavior[] behaviors)
		{
			foreach (var it in behaviors)
			{
				AddOption(it);
			}
		}

		public static void RemoveOption(IClientBehavior behavior)
		{
			_behaviors.Remove(behavior);
			OnExcation -= behavior.OnExcaption;
			OnMessage -= behavior.OnMessage;
			OnServerConnect -= behavior.Open;
			OnServerDisconect -= behavior.Close;
		}
		public static void RemoveOptions(params IClientBehavior[] behaviors)
		{
			foreach (var it in behaviors)
				RemoveOption(it);
		}

		public static async void RunWrite(string name,object Message)
		{

			var jo = new JObject()
			{
				[name] = JToken.FromObject(Message)
			};


			Send(jo);
		}

		public static void Send(byte[] data)
		{
			_stream.Write(data, 0, data.Length);
		}
		public static void Send(string data)
		{
			var message = Encoding.UTF8.GetBytes(data);

			Send(message);
		}
		public static void Send(object data)
		{
			Send(Newtonsoft.Json.JsonConvert.SerializeObject(data));
		}


	}

	public interface IClientBehavior
	{
		void OnMessage(JObject data);
		void OnExcaption(Exception ex);
		void Open();
		void Close();
	}
}
