

using System;

namespace AZLinli.ORM.ILReader
{

	class ByteBuffer {

		internal byte [] buffer;
		internal int position;

		public ByteBuffer (byte [] buffer)
		{
			this.buffer = buffer;
		}

		public byte ReadByte ()
		{
			CheckCanRead (1);
			return buffer [position++];
		}

		public byte [] ReadBytes (int length)
		{
			CheckCanRead (length);
			var value = new byte [length];
			Buffer.BlockCopy (buffer, position, value, 0, length);
			position += length;
			return value;
		}

		public short ReadInt16 ()
		{
			CheckCanRead (2);
			short value = (short) (buffer [position]
				| (buffer [position + 1] << 8));
			position += 2;
			return value;
		}

		public int ReadInt32 ()
		{
			CheckCanRead (4);
			int value = buffer [position]
				| (buffer [position + 1] << 8)
				| (buffer [position + 2] << 16)
				| (buffer [position + 3] << 24);
			position += 4;
			return value;
		}

		public long ReadInt64 ()
		{
			CheckCanRead (8);
			uint low = (uint) (buffer [position]
				| (buffer [position + 1] << 8)
				| (buffer [position + 2] << 16)
				| (buffer [position + 3] << 24));

			uint high = (uint) (buffer [position + 4]
				| (buffer [position + 5] << 8)
				| (buffer [position + 6] << 16)
				| (buffer [position + 7] << 24));

			long value = (((long) high) << 32) | low;
			position += 8;
			return value;
		}

		public float ReadSingle ()
		{
			if (!BitConverter.IsLittleEndian) {
				var bytes = ReadBytes (4);
				Array.Reverse (bytes);
				return BitConverter.ToSingle (bytes, 0);
			}

			CheckCanRead (4);
			float value = BitConverter.ToSingle (buffer, position);
			position += 4;
			return value;
		}

		public double ReadDouble ()
		{
			if (!BitConverter.IsLittleEndian) {
				var bytes = ReadBytes (8);
				Array.Reverse (bytes);
				return BitConverter.ToDouble (bytes, 0);
			}

			CheckCanRead (8);
			double value = BitConverter.ToDouble (buffer, position);
			position += 8;
			return value;
		}

		void CheckCanRead (int count)
		{
			if (position + count > buffer.Length)
				throw new ArgumentOutOfRangeException ();
		}
	}
}
