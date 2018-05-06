using System;
using ImageDL.Classes.SettingParsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImageDL.Tests.SettingParsingTests
{
	[TestClass]
	public class SettingParsingTest
	{
		private TestStruct TestStruct;
		private SettingParser SettingParser;

		public SettingParsingTest()
		{
			TestStruct = new TestStruct();
			SettingParser = new SettingParser("--", "-", "/")
			{
				new Setting<string>(new[] { nameof(TestStruct.StringValue), }, x => TestStruct.StringValue = x),
				new Setting<int>(new[] { nameof(TestStruct.IntValue), }, x => TestStruct.IntValue = x),
				new Setting<bool>(new[] { nameof(TestStruct.BoolValue), }, x => TestStruct.BoolValue = x),
				new Setting<bool>(new[] { nameof(TestStruct.FlagValue), }, x => TestStruct.FlagValue = x)
				{
					IsFlag = true,
				},
				new Setting<bool>(new[] { nameof(TestStruct.FlagValue2), }, x => TestStruct.FlagValue2 = x)
				{
					IsFlag = true,
				},
				new Setting<ulong>(new[] { nameof(TestStruct.UlongValue), }, x => TestStruct.UlongValue = x),
				new Setting<DateTime>(new[] { nameof(TestStruct.DateTimeValue), }, x => TestStruct.DateTimeValue = x,
					s => (DateTime.TryParse(s, out var result), result)),
			};
		}

		[TestMethod]
		public void BasicParsing_Test()
		{
			var pf = SettingParser.Prefixes[0];
			SettingParser.Parse($"{pf}{nameof(TestStruct.StringValue)} StringValueTest");
			Assert.AreEqual("StringValueTest", TestStruct.StringValue);
			SettingParser.Parse($"{pf}{nameof(TestStruct.IntValue)} 1");
			Assert.AreEqual(1, TestStruct.IntValue);
			SettingParser.Parse($"{pf}{nameof(TestStruct.BoolValue)} true");
			Assert.AreEqual(true, TestStruct.BoolValue);
			SettingParser.Parse($"{pf}{nameof(TestStruct.FlagValue)}");
			Assert.AreEqual(true, TestStruct.FlagValue);
			SettingParser.Parse($"{pf}{nameof(TestStruct.UlongValue)} 18446744073709551615");
			Assert.AreEqual(ulong.MaxValue, TestStruct.UlongValue);
			SettingParser.Parse($"{pf}{nameof(TestStruct.DateTimeValue)} 05/06/2018");
			Assert.AreEqual(new DateTime(2018, 5, 6), TestStruct.DateTimeValue);
		}
		[TestMethod]
		public void ComplicatedParsing_Test()
		{
			var pf = SettingParser.Prefixes[0];
			var result = SettingParser.Parse($"{pf}{nameof(TestStruct.StringValue)} StringValueTest2 " +
				$"{pf}{nameof(TestStruct.FlagValue2)} " +
				$"{pf}{nameof(TestStruct.BoolValue)} true " +
				$"{pf}{nameof(TestStruct.UlongValue)} asdf " +
				$"{pf}help {nameof(TestStruct.FlagValue2)} " +
				$"extra");
			Assert.AreEqual(3, result.Successes.Length);
			Assert.AreEqual(1, result.Errors.Length);
			Assert.AreEqual(1, result.UnusedParts.Length);
			Assert.AreEqual(1, result.Help.Length);
			Assert.AreEqual("StringValueTest2", TestStruct.StringValue);
			Assert.AreEqual(true, TestStruct.FlagValue2);
			Assert.AreEqual(true, TestStruct.BoolValue);
			Assert.AreEqual(default(ulong), TestStruct.UlongValue);
		}
	}

	struct TestStruct
	{
		public string StringValue { get; set; }
		public int IntValue { get; set; }
		public bool BoolValue { get; set; }
		public bool FlagValue { get; set; }
		public bool FlagValue2 { get; set; }
		public ulong UlongValue { get; set; }
		public DateTime DateTimeValue { get; set; }
	}
}