﻿using System.Linq;
using Xunit;

namespace ShatteredGenerator.Tests
{
	public class Eu4DataConvertTests
	{
		[Fact]
		public void Deserialize_OneField_Serializes()
		{
			// Arrange
			const string text = "blah=test";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(1, data.Count);
			Assert.Equal("test", data.One("blah"));
		}

		[Fact]
		public void Deserialize_OneFieldLiteralKey_Serializes()
		{
			// Arrange
			const string text = "\"#1 blah\"=test";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(1, data.Count);
			Assert.Equal("test", data.One("#1 blah"));
		}

		[Fact]
		public void Deserialize_TwoFields_Serializes()
		{
			// Arrange
			const string text = "blah=test\ntest=blah";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			Assert.Equal("test", data.One("blah"));
			Assert.Equal("blah", data.One("test"));
		}

		[Fact]
		public void Deserialize_TwoFieldsInline_Serializes()
		{
			// Arrange
			const string text = "blah=test test=blah";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			Assert.Equal("test", data.One("blah"));
			Assert.Equal("blah", data.One("test"));
		}

		[Fact]
		public void Deserialize_TwoFieldsUnclosedLiteralOnFirst_Serializes()
		{
			// Arrange
			const string text = "blah=\"test\ntest=blah";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			Assert.Equal("test", data.One("blah"));
			Assert.Equal("blah", data.One("test"));
		}

		[Fact]
		public void Deserialize_KeylessValues_Serializes()
		{
			// Arrange
			const string text = "blah test";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			var keyless = data.Many("").ToList();
			Assert.Equal(1, keyless.Count(v => v == "blah"));
			Assert.Equal(1, keyless.Count(v => v == "test"));
		}

		[Fact]
		public void Deserialize_MixedKeylessAndKeyValue_Serializes()
		{
			// Arrange
			const string text = "blah test=stuff";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			var keyless = data.Many("").ToList();
			Assert.Equal(1, keyless.Count(v => v == "blah"));
			Assert.Equal("stuff", data.One("test"));
		}

		[Fact]
		public void Deserialize_TwoFieldsWithEmptyLine_Serializes()
		{
			// Arrange
			const string text = "blah=test\n\ntest=blah";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			Assert.Equal("test", data.One("blah"));
			Assert.Equal("blah", data.One("test"));
		}

		[Fact]
		public void Deserialize_TwoSameKeyFields_Serializes()
		{
			// Arrange
			const string text = "blah=test\n\nblah=testing";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			var fields = data.Many("blah").ToList();
			Assert.Equal(2, fields.Count);
			Assert.Equal("test", fields[0]);
			Assert.Equal("testing", fields[1]);
		}

		[Fact]
		public void Deserialize_NestedObject_Serializes()
		{
			// Arrange
			const string text = "blah={\nbluh=test bleh=blegh\nflargh=flemish}";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(1, data.Count);
			var nested = data.OneNested("blah");
			Assert.Equal(3, nested.Count);
			Assert.Equal("test", nested.One("bluh"));
			Assert.Equal("blegh", nested.One("bleh"));
			Assert.Equal("flemish", nested.One("flargh"));
		}

		[Fact]
		public void Deserialize_CommentBreakingValue_Serializes()
		{
			// Arrange
			const string text = "blah=test#blughablargh\ntest=blah";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			Assert.Equal("test", data.One("blah"));
			Assert.Equal("blah", data.One("test"));
		}

		[Fact]
		public void Deserialize_WeirdSpacing_Serializes()
		{
			// Arrange
			const string text = "blah =   test\n test \t =blah ";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(2, data.Count);
			Assert.Equal("test", data.One("blah"));
			Assert.Equal("blah", data.One("test"));
		}

		[Fact]
		public void Deserialize_QuotedString_Serializes()
		{
			// Arrange
			const string text = "blah = \"this is a test\"";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(1, data.Count);
			Assert.Equal("this is a test", data.One("blah"));
		}

		[Fact]
		public void Deserialize_NestedObjectOpeningBracketOnNewLine_Serializes()
		{
			// Arrange
			const string text = "blah=\n{\nbluh=test bleh=blegh\nflargh=flemish}";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(1, data.Count);
			var nested = data.OneNested("blah");
			Assert.Equal(3, nested.Count);
			Assert.Equal("test", nested.One("bluh"));
			Assert.Equal("blegh", nested.One("bleh"));
			Assert.Equal("flemish", nested.One("flargh"));
		}

		[Fact]
		public void Deserialize_NestedObjectInLiteralComment_SerializesWithoutRemovingComment()
		{
			// Arrange
			const string nestedString = "I am test #whatever";
			const string text = "blah = {test=\"" + nestedString + "\"}";

			// Act
			var data = Eu4DataConvert.Deserialize(text);

			// Assert
			Assert.Equal(1, data.Count);
			var nested = data.OneNested("blah");
			Assert.Equal(nestedString, nested.One("test"));
		}

		[Fact]
		public void Serialize_OneField_Serializes()
		{
			// Arrange
			var data = new Eu4Data();

			// Act
			data.Set("blah", "test");
			var result = Eu4DataConvert.Deserialize(data.Serialize());

			// Assert
			Assert.Equal(1, result.Count);
			Assert.Equal("test", result.One("blah"));
		}

		[Fact]
		public void Serialize_StringWithSpaces_SerializesWithQuotes()
		{
			// Arrange
			var data = new Eu4Data();

			// Act
			data.Set("blah", "this is a test");
			var result = Eu4DataConvert.Deserialize(data.Serialize());

			// Assert
			Assert.Equal(1, result.Count);
			Assert.Equal("this is a test", result.One("blah"));
		}

		/*[Fact] Turns out EU4's file data supports an unclosed literal that's closed at the end of a line automatically
		public void Serialize_StringWithEnters_SerializesWithQuotes()
		{
			// Arrange
			var data = new Eu4Data();

			// Act
			data.Set("blah", "this is\na test");
			var serialized = data.Serialize();

			// Assert
			var result = Eu4DataConvert.Deserialize(serialized);
			Assert.Equal(1, result.Count);
			Assert.Equal("this is\na test", result.One("blah"));
		}*/
	}
}