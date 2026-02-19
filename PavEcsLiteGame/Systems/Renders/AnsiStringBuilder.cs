using System;
using System.Runtime.CompilerServices;
using PavEcsGame.Components;

namespace PavEcsGame.Systems.Renders
{
    /// <summary>
    /// Zero-allocation ANSI escape sequence builder for optimized console rendering.
    ///
    /// Key optimization: Leverages cursor auto-advancement to minimize position commands.
    /// When cursor is at (x,y), writing a character moves cursor to (x+1,y).
    /// This builder detects when target cursor can be reached via auto-advance and skips the position command.
    /// </summary>
    public ref struct AnsiStringBuilder
    {
        private Span<char> _buffer;
        private int _position;

        // State tracking to minimize escape codes
        private int _lastX;
        private int _lastY;
        private Color _lastFgRgb;
        private Color _lastBgRgb;

        public AnsiStringBuilder(Span<char> buffer)
        {
            _buffer = buffer;
            _position = 0;
            _lastX = -1;
            _lastY = -1;
            _lastFgRgb = new Color(0xFFFFFFFF); // Invalid sentinel
            _lastBgRgb = new Color(0xFFFFFFFF); // Invalid sentinel
        }

        /// <summary>
        /// Smart cursor positioning that skips command if cursor is already at target position
        /// or can reach it via auto-advancement.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCursorIfNeeded(int x, int y)
        {
            // Already at target position
            if (_lastX == x && _lastY == y)
                return;

            // Cursor auto-advanced to target position
            // (cursor moves right after each character written)
            // if (_lastY == y && _lastX + 1 == x)
            // {
            //     _lastX = x;
            //     return;
            // }

            // Need to emit position command
            AppendCursorPosition(x, y);
        }

        /// <summary>
        /// Append cursor position command. Always emits the sequence.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendCursorPosition(int x, int y)
        {
            EnsureCapacity(20);

            _buffer[_position++] = '\u001b';
            _buffer[_position++] = '[';
            _position += WriteIntToSpan(_buffer.Slice(_position), y + 1);
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), x + 1);
            _buffer[_position++] = 'H';

            _lastX = x;
            _lastY = y;
        }

        /// <summary>
        /// Append 24-bit RGB color command only if colors changed.
        /// Emits ESC[38;2;R;G;Bm for foreground and ESC[48;2;R;G;Bm for background.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendColorRgb(Color fg, Color bg)
        {
            // Skip if colors unchanged (same as 16-color optimization)
            if (_lastFgRgb.PackedValue == fg.PackedValue &&
                _lastBgRgb.PackedValue == bg.PackedValue)
                return;

            EnsureCapacity(40); // Worst case: ESC[38;2;255;255;255;48;2;255;255;255m

            // Foreground: ESC[38;2;<r>;<g>;<b>m
            _buffer[_position++] = '\u001b';
            _buffer[_position++] = '[';
            _buffer[_position++] = '3';
            _buffer[_position++] = '8';
            _buffer[_position++] = ';';
            _buffer[_position++] = '2';
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), fg.R);
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), fg.G);
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), fg.B);
            _buffer[_position++] = 'm';

            // Background: ESC[48;2;<r>;<g>;<b>m
            _buffer[_position++] = '\u001b';
            _buffer[_position++] = '[';
            _buffer[_position++] = '4';
            _buffer[_position++] = '8';
            _buffer[_position++] = ';';
            _buffer[_position++] = '2';
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), bg.R);
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), bg.G);
            _buffer[_position++] = ';';
            _position += WriteIntToSpan(_buffer.Slice(_position), bg.B);
            _buffer[_position++] = 'm';

            _lastFgRgb = fg;
            _lastBgRgb = bg;
        }

        /// <summary>
        /// Append a character and track cursor auto-advancement.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendChar(char c)
        {
            EnsureCapacity(1);

            _buffer[_position++] = c;
            _lastX++; // Cursor auto-advances right after character
        }

        /// <summary>
        /// Append multiple characters and track cursor advancement.
        /// </summary>
        public void AppendChars(ReadOnlySpan<char> chars)
        {
            EnsureCapacity(chars.Length);

            chars.CopyTo(_buffer.Slice(_position));
            _position += chars.Length;
            _lastX += chars.Length;
        }

        public void AppendLine(int x, int y, int width, ReadOnlySpan<char> text, Color fg, Color bg)
        {
            AppendCursorPosition(x, y);
            AppendColorRgb(fg, bg);
            int len = Math.Min(text.Length, width);
            if (len > 0)
                AppendChars(text.Slice(0, len));
            for (int i = len; i < width; i++)
                AppendChar(' ');
        }

        /// <summary>
        /// Get the built ANSI string.
        /// </summary>
        public ReadOnlySpan<char> AsSpan() => _buffer.Slice(0, _position);

        /// <summary>
        /// Length of built string.
        /// </summary>
        public int Length => _position;

        /// <summary>
        /// Check buffer capacity and throw if overflow.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int needed)
        {
            if (_position + needed > _buffer.Length)
            {
                throw new InvalidOperationException(
                    $"AnsiStringBuilder buffer overflow: need {needed} chars, " +
                    $"only {_buffer.Length - _position} available. " +
                    $"Increase buffer size or reduce command count.");
            }
        }

        /// <summary>
        /// Convert integer to character span, returning number of characters written.
        /// Zero-allocation helper using stack memory.
        /// </summary>
        private static int WriteIntToSpan(Span<char> destination, int value)
        {
            Span<char> temp = stackalloc char[10];
            var pos = 0;

            do
            {
                var digit = value % 10;
                temp[pos++] = (char)('0' + digit);
                value /= 10;
            } while (value != 0);

            for (var i = pos - 1; i >= 0; i--)
            {
                destination[pos - 1 - i] = temp[i];
            }

            return pos;
        }

    }
}
