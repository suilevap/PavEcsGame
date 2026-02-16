using System;
using System.Diagnostics;

namespace PavEcsGame.Systems.Utils
{
    internal class GameDebugLog : TraceListener
    {
        private readonly string[] _lines = new string[64];
        private int _head;
        private int _count;

        public int Version { get; private set; }

        public override void Write(string? message)
        {
            if (!string.IsNullOrEmpty(message))
                AddLine(message);
        }

        public override void WriteLine(string? message)
        {
            AddLine(message ?? string.Empty);
        }

        private static readonly char[] LineSeparators = { '\n', '\r' };

        private void AddLine(string message)
        {
            var parts = message.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in parts)
            {
                _lines[_head] = line;
                _head = (_head + 1) & 63;
                if (_count < 64) _count++;
            }
            Version++;
        }

        public void GetLastLines(int maxLines, string[] outLines, out int count)
        {
            count = Math.Min(maxLines, _count);
            for (int i = 0; i < count; i++)
            {
                int idx = (_head - count + i) & 63;
                outLines[i] = _lines[idx];
            }
        }
    }
}
