using Chip.Net.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Data
{
	public enum TestEnum {
		One = 1,
		Two = 10,
		Three = 11,
	}

	public class TestModelISerializable : ISerializable {
		public UInt16 UShort { get; set; }
		public UInt32 UInt { get; set; }
		public UInt64 ULong { get; set; }

		public void ReadFrom(DataBuffer buffer) {
			UShort = buffer.ReadUInt16();
			UInt = buffer.ReadUInt32();
			ULong = buffer.ReadUInt64();
		}

		public void WriteTo(DataBuffer buffer) {
			buffer.Write((UInt16)UShort);
			buffer.Write((UInt32)UInt);
			buffer.Write((UInt64)ULong);
		}
	}

	public class TestModelDynamic : Serializable {
		public string Data { get; set; }
		public float Float { get; set; }
		public double Double { get; set; }
	}

	public class TestModel {
		public string Data { get; set; }
		public byte ByteValue { get; set; }
		public short ShortValue { get; set; }
		public int IntValue { get; set; }
		public long LongValue { get; set; }
	}

	public class TestLargeModel {
		public string B1 { get; set; }
		public string B2 { get; set; }
		public string B3 { get; set; }
		public string B4 { get; set; }
		public string B5 { get; set; }
		public string B6 { get; set; }
		public string B7 { get; set; }
		public string B8 { get; set; }
		public string B9 { get; set; }
		public string B10 { get; set; }
	}

	public class TestSimpleModel {
		public string Data { get; set; }
	}

	public class TestNestedModel {
		public TestNestedModel Inner { get; set; }
		public string Data { get; set; }
	}

	[TestClass]
    public class DynamicSerializerTests
    {
		[TestMethod]
		public void DynamicSerializer_WriteReadTestModel() {
			Random r = new Random();
			TestModel m = new TestModel();

			m.Data = "Hello world";
			m.ByteValue = (byte)r.Next();
			m.ShortValue = (short)r.Next();
			m.IntValue = (int)r.Next();
			m.LongValue = (long)r.Next();

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write(b, typeof(TestModel), m);

			b.Seek(0);
			var mresult = DynamicSerializer.Read<TestModel>(b);

			Assert.IsNotNull(mresult);
			Assert.IsNotNull(mresult.Data);
			Assert.IsFalse(mresult.Data == "");
			Assert.AreEqual(m.Data, mresult.Data);
			Assert.AreEqual(m.ByteValue, mresult.ByteValue);
			Assert.AreEqual(m.ShortValue, mresult.ShortValue);
			Assert.AreEqual(m.IntValue, mresult.IntValue);
			Assert.AreEqual(m.LongValue, mresult.LongValue);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadNestedModel() {
			TestNestedModel m = new TestNestedModel();
			m.Inner = new TestNestedModel();
			m.Data = "Root";
			m.Inner.Data = "Inner";

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<TestNestedModel>(b, m);

			b.Seek(0);
			var mresult = DynamicSerializer.Read<TestNestedModel>(b);

			Assert.IsNotNull(mresult);
			Assert.IsNotNull(mresult.Inner);
			Assert.IsNotNull(mresult.Data);
			Assert.IsNotNull(mresult.Inner.Data);
			Assert.AreEqual(m.Data, mresult.Data);
			Assert.AreEqual(m.Inner.Data, mresult.Inner.Data);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadListOfPrimitives() {
			List<byte> data = new List<byte>() {
				1, 2, 4, 8, 16, 32
			};

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<List<byte>>(b, data);

			b.Seek(0);
			var result = DynamicSerializer.Read<List<byte>>(b);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Count == data.Count);
			for (int i = 0; i < data.Count; i++)
				Assert.AreEqual(data[i], result[i]);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadListOfModels() {
			List<TestSimpleModel> data = new List<TestSimpleModel>() {
				new TestSimpleModel() {
					Data = "d1"
				},
				new TestSimpleModel() {
					Data = "d2"
				},
				new TestSimpleModel() {
					Data = "d3"
				},
				new TestSimpleModel() {
					Data = "d4"
				},
			};


			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<List<TestSimpleModel>>(b, data);

			b.Seek(0);
			var result = DynamicSerializer.Read<List<TestSimpleModel>>(b);

			Assert.IsNotNull(result);
			Assert.AreEqual(result.Count, data.Count);
			for (int i = 0; i < result.Count; i++)
				Assert.AreEqual(result[i].Data, data[i].Data);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadArrayOfPrimitives() {
			byte[] data = new byte[] {
				1, 2, 4, 8, 16, 32
			};

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<byte[]>(b, data);

			b.Seek(0);
			var result = DynamicSerializer.Read<byte[]>(b);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Length == data.Length);
			for (int i = 0; i < data.Length; i++)
				Assert.AreEqual(data[i], result[i]);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadArrayOfModels() {
			TestSimpleModel[] data = new TestSimpleModel[] {
				new TestSimpleModel() {
					Data = "d1"
				},
				new TestSimpleModel() {
					Data = "d2"
				},
				new TestSimpleModel() {
					Data = "d3"
				},
				new TestSimpleModel() {
					Data = "d4"
				},
			};

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<TestSimpleModel[]>(b, data);

			b.Seek(0);
			var result = DynamicSerializer.Read<TestSimpleModel[]>(b);

			Assert.IsNotNull(result);
			Assert.AreEqual(result.Length, data.Length);
			for (int i = 0; i < result.Length; i++)
				Assert.AreEqual(result[i].Data, data[i].Data);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadLargeModel() {
			TestLargeModel m = new TestLargeModel();
			m.B1 = "1";
			m.B2 = "2";
			m.B3 = "3";
			m.B4 = "4";
			m.B5 = "5";
			m.B6 = "6";
			m.B7 = "7";
			m.B8 = "8";
			m.B9 = "9";
			m.B10 = "10";

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<TestLargeModel>(b, m);

			b.Seek(0);
			var mresult = DynamicSerializer.Read<TestLargeModel>(b);

			Assert.IsNotNull(mresult);
			Assert.AreEqual(m.B1, mresult.B1);
			Assert.AreEqual(m.B2, mresult.B2);
			Assert.AreEqual(m.B3, mresult.B3);
			Assert.AreEqual(m.B4, mresult.B4);
			Assert.AreEqual(m.B5, mresult.B5);
			Assert.AreEqual(m.B6, mresult.B6);
			Assert.AreEqual(m.B7, mresult.B7);
			Assert.AreEqual(m.B8, mresult.B8);
			Assert.AreEqual(m.B9, mresult.B9);
			Assert.AreEqual(m.B10, mresult.B10);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadISerializable() {
			Random r = new Random();
			TestModelISerializable m = new TestModelISerializable();
			m.UInt = (UInt32)r.Next();
			m.UShort = (UInt16)r.Next();
			m.ULong = (UInt64)r.Next();

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<TestModelISerializable>(b, m);

			b.Seek(0);
			var result = DynamicSerializer.Read<TestModelISerializable>(b);

			Assert.IsNotNull(result);
			Assert.AreEqual(m.UInt, result.UInt);
			Assert.AreEqual(m.UShort, result.UShort);
			Assert.AreEqual(m.ULong, result.ULong);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadSerializable() {
			Random r = new Random();
			TestModelDynamic m = new TestModelDynamic();

			m.Data = "Data";
			m.Double = r.NextDouble();
			m.Float = (float)r.NextDouble();

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<TestModelDynamic>(b, m);

			b.Seek(0);
			var result = DynamicSerializer.Read<TestModelDynamic>(b);

			Assert.IsNotNull(result);
			Assert.AreEqual(m.Data, result.Data);
			Assert.AreEqual(m.Double, result.Double);
			Assert.AreEqual(m.Float, result.Float);
		}

		[TestMethod]
		public void DynamicSerializerTests_TestModelISerializable_WriteRead() {
			Random r = new Random();
			TestModelISerializable m = new TestModelISerializable();
			m.UInt = (UInt32)r.Next();
			m.UShort = (UInt16)r.Next();
			m.ULong = (UInt64)r.Next();

			DataBuffer b = new DataBuffer();
			m.WriteTo(b);

			b.Seek(0);
			TestModelISerializable result = new TestModelISerializable();
			result.ReadFrom(b);

			Assert.IsNotNull(result);
			Assert.AreEqual(m.UInt, result.UInt);
			Assert.AreEqual(m.UShort, result.UShort);
			Assert.AreEqual(m.ULong, result.ULong);
		}

		[TestMethod]
		public void DynamicSerializer_WriteReadEnum() {
			TestEnum e1 = TestEnum.One;
			TestEnum e2 = TestEnum.Two;
			TestEnum e3 = TestEnum.Three;

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write<TestEnum>(b, e1);
			DynamicSerializer.Write<TestEnum>(b, e2);
			DynamicSerializer.Write<TestEnum>(b, e3);

			b.Seek(0);
			var r1 = DynamicSerializer.Read<TestEnum>(b);
			var r2 = DynamicSerializer.Read<TestEnum>(b);
			var r3 = DynamicSerializer.Read<TestEnum>(b);

			Assert.AreEqual(e1, r1);
			Assert.AreEqual(e2, r2);
			Assert.AreEqual(e3, r3);
		}
	}
}
