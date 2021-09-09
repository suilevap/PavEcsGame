using System;
using System.Text;
using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    //adjusted copy of monogame color https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Color.cs
    public struct Color : IEquatable<Color>
    {
        public static Color Zero = new Color(0);
        public static Color One = new Color(0xFFFFFFFF);


        // Stored as RGBA with R in the least significant octet:
        // |-------|-------|-------|-------
        // A       B       G       R
        private uint _packedValue;

        /// <summary>
        /// Constructs an RGBA color from a packed value.
        /// The value is a 32-bit unsigned integer, with R in the least significant octet.
        /// </summary>
        /// <param name="packedValue">The packed value.</param>
        //[CLSCompliant(false)]
        public Color(uint packedValue)
        {
            _packedValue = packedValue;
        }

        public Color(Float4 color)
            : this((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255), (int)(color.W * 255))
        {
        }

        public Color(Float3 color)
            : this((color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255))
        {
        }

        public Color(Color color, int alpha)
        {
            if ((alpha & 0xFFFFFF00) != 0)
            {
                var clampedA = (uint)MathFast.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                _packedValue = (color._packedValue & 0x00FFFFFF) | (clampedA << 24);
            }
            else
            {
                _packedValue = (color._packedValue & 0x00FFFFFF) | ((uint)alpha << 24);
            }
        }

        public Color(Color color, float alpha) :
            this(color, (int)(alpha * 255))
        {
        }

        public Color(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        {
        }

        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        public Color(int r, int g, int b)
        {
            _packedValue = 0xFF000000; // A = 255

            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)MathFast.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)MathFast.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)MathFast.Clamp(b, Byte.MinValue, Byte.MaxValue);

                _packedValue |= (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                _packedValue |= ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        public Color(int r, int g, int b, int alpha)
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)MathFast.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)MathFast.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)MathFast.Clamp(b, Byte.MinValue, Byte.MaxValue);
                var clampedA = (uint)MathFast.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                _packedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }
        }

        public Color(byte r, byte g, byte b, byte alpha)
        {
            _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);
        }

        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 16);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xff00ffff) | ((uint)value << 16);
            }
        }

        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 8);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffff00ff) | ((uint)value << 8);
            }
        }

        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte)this._packedValue;
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0xffffff00) | value;
            }
        }

        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(this._packedValue >> 24);
                }
            }
            set
            {
                this._packedValue = (this._packedValue & 0x00ffffff) | ((uint)value << 24);
            }
        }

        public static bool operator ==(Color a, Color b)
        {
            return (a._packedValue == b._packedValue);
        }

        public static bool operator !=(Color a, Color b)
        {
            return (a._packedValue != b._packedValue);
        }


        public override int GetHashCode()
        {
            return this._packedValue.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            return ((obj is Color) && this.Equals((Color)obj));
        }

        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Color"/>.</param>
        /// <param name="value2">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        public static Color Lerp(Color value1, Color value2, Single amount)
        {
            amount = MathFast.Clamp(amount, 0, 1);
            return new Color(
                (int)MathFast.Lerp(value1.R, value2.R, amount),
                (int)MathFast.Lerp(value1.G, value2.G, amount),
                (int)MathFast.Lerp(value1.B, value2.B, amount),
                (int)MathFast.Lerp(value1.A, value2.A, amount));
        }


        public static Color Multiply(Color value, float scale)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        public static Color operator +(Color value, Color v2)
        {
            return new Color((int)(value.R + v2.R), (int)(value.G + v2.G), (int)(value.B + v2.B), (int)(value.A + v2.A));
        }


        public static Color operator *(float scale, Color value)
        {
            return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
        }

        public Float3 ToVector3()
        {
            return new Float3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        public Float4 ToVector4()
        {
            return new Float4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }

        /// <summary>
        /// Gets or sets packed value of this <see cref="Color"/>.
        /// </summary>
        //[CLSCompliant(false)]
        public UInt32 PackedValue
        {
            get { return _packedValue; }
            set { _packedValue = value; }
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(25);
            sb.Append("{R:");
            sb.Append(R);
            sb.Append(" G:");
            sb.Append(G);
            sb.Append(" B:");
            sb.Append(B);
            sb.Append(" A:");
            sb.Append(A);
            sb.Append("}");
            return sb.ToString();
        }

  
        public static Color FromNonPremultiplied(Float4 vector)
        {
            return new Color(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
        }

        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color(r * a / 255, g * a / 255, b * a / 255, a);
        }

        #region IEquatable<Color> Members

        public bool Equals(Color other)
        {
            return this.PackedValue == other.PackedValue;
        }

        #endregion

        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }


        public void Deconstruct(out float r, out float g, out float b)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
        }

 
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R / 255f;
            g = G / 255f;
            b = B / 255f;
            a = A / 255f;
        }
    }
}