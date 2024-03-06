namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable;

    [TestClass()]
    public class HexConverterTests
    {
        [TestMethod()]
        public void ToHexStringTest_ASCII()
        {
            string input = "Hello World";
            string hexString = HexConverter.ToHexString(input);
            string output = HexConverter.FromHexString(hexString);

            Assert.AreEqual(input, output);
        }

        [TestMethod()]
        public void ToHexStringTest_Unicode()
        {
            string input = "⫋⨖≯⿍⸮∾⽏⬷⺃⟙⟐▔⓶ⵕⴜ⋡∝ⵖ⥣⏄⑗⟆ⵦⱒ❮⯀⤨⑖⇜┎⮮ⱈ␚₺✲➦✦⡇⬜⃪♹⽰⠲⌹⻊ₘ♧⓷⼞⯂⎙ⵉ⏧⪢⧆❲┄➸⚵∑⚡␟⇈⪒⍉⏳⚑⋮↗⒅⠞⌣⏱♽ⵕ⥢⡜⎠Ᵽ⻿⏋⏝↊⏱☋⿰⋊™⺊⧤⋋℔⧲ⱼ⨡╁⒐⣄⩆ⴃ";
            string hexString = HexConverter.ToHexString(input);
            string output = HexConverter.FromHexString(hexString);

            Assert.AreEqual(input, output);
        }

        [TestMethod()]
        public void FromHexString_NotEncoded()
        {
            string input = "This is a regular string";
            string output = HexConverter.FromHexString(input);

            Assert.AreEqual(input, output);
        }
    }
}