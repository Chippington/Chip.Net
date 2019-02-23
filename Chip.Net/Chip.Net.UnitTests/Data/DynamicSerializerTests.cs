using Chip.Net.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Data
{
	public class TestModel {
		public string Data { get; set; }
		public byte ByteValue { get; set; }
		public short ShortValue { get; set; }
		public int IntValue { get; set; }
	}

	public class TestNestedModel {
		public TestNestedModel Inner { get; set; }
		public TestModel Model { get; set; }
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

			DataBuffer b = new DataBuffer();
			DynamicSerializer.Write(b, typeof(TestModel), m);

			b.Seek(0);
			var mresult = DynamicSerializer.Read<TestModel>(b);

			Assert.IsNotNull(mresult);
			Assert.AreEqual(m.Data, mresult.Data);
			Assert.AreEqual(m.ByteValue, mresult.ByteValue);
			Assert.AreEqual(m.ShortValue, mresult.ShortValue);
			Assert.AreEqual(m.IntValue, mresult.IntValue);
		}
	}
}
