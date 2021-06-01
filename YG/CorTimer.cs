using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YG
{
	public static class CorTimer
	{
		private static bool _isRun;

		private static List<Entry> _entries;

		private static int _duration;

		public static float Duration
		{
			get => (int)(_duration / 1000);
			set => _duration = (int)(value * 1000);
		}

		public static async void Start()
		{
			_isRun = true;
			_entries = new List<Entry>();
			while (_isRun)
			{
				for (int i = 0; i < _entries.Count; i++)
				{
					var entry = _entries[i];
					if (entry.CanExit())
					{
						_entries.RemoveAt(i);
						i--;
						continue;
					}
					entry.Duration -= Duration;
					if (entry.Duration <= 0)
					{
						entry.OnComplite();
						_entries.RemoveAt(i);
						i--;
						continue;
					}
				}
				await Task.Delay(_duration);
			}
		}

		public static void Stop()
		{
			_isRun = false;
		}

		public static void AddEntry(float duration, Func<bool> canExit, Action onComplite)
		{
			_entries.Add(new Entry
			{
				Duration = duration,
				CanExit = canExit,
				OnComplite = onComplite
			});
		}

		private class Entry
		{
			public float Duration { get; set; }
			public Func<bool> CanExit { get; set; }
			public Action OnComplite { get; set; }
		}
	}
}
