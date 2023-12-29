using Dapper.Entities.SqlServer;
using Testing.Common.Models;
using Testing.Models;

namespace Testing.SqlServer;

[TestClass]
public class SqlBuilder
{
	[TestMethod]
	public void SampleEntity()
	{
		var builder = new DefaultSqlBuilder();
		var statements = builder.BuildStatements(typeof(SampleEntity));

		Assert.IsTrue(statements.Insert.Equals(
			"INSERT INTO [whatever].[Sample] ([Name], [Description]) VALUES (@Name, @Description); SELECT SCOPE_IDENTITY()"));

		Assert.IsTrue(statements.Update.Equals(
			"UPDATE [whatever].[Sample] SET [Name]=@Name, [Description]=@Description WHERE [Id]=@Id"));

		Assert.IsTrue(statements.Delete.Equals(
			"DELETE [whatever].[Sample] WHERE [Id]=@Id"));

		Assert.IsTrue(statements.GetByAlternateKey.Equals(
			"SELECT * FROM [whatever].[Sample] WHERE [Name]=@Name"));
	}

	[TestMethod]
	public void ExoticEntity()
	{
		var builder = new DefaultSqlBuilder();
		var statements = builder.BuildStatements(typeof(ExoticEntity));

		Assert.IsTrue(statements.Insert.Equals(
			"INSERT INTO [dbo].[ExoticEntity] ([Name], [Value], [Aliased], [DateCreated]) VALUES (@Name, @Value, @AliasedColumn, @DateCreated); SELECT SCOPE_IDENTITY()"));

		Assert.IsTrue(statements.Update.Equals(
			"UPDATE [dbo].[ExoticEntity] SET [Name]=@Name, [Value]=@Value, [Aliased]=@AliasedColumn, [DateModified]=@DateModified WHERE [Id]=@Id"));

		Assert.IsTrue(statements.Delete.Equals(
			"DELETE [dbo].[ExoticEntity] WHERE [Id]=@Id"));

		Assert.IsTrue(statements.GetByAlternateKey.Equals(string.Empty));
	}

	[TestMethod]
	public void CompositeEntity()
	{
		var builder = new DefaultSqlBuilder();
		var statements = builder.BuildStatements(typeof(CompositeKeyEntity));

		Assert.IsTrue(statements.Insert.Equals(
			"INSERT INTO [dbo].[CompositeKeyEntity] ([SomethingId], [Name], [Description]) VALUES (@SomethingId, @Name, @Description); SELECT SCOPE_IDENTITY()"));

		Assert.IsTrue(statements.Update.Equals(
			"UPDATE [dbo].[CompositeKeyEntity] SET [SomethingId]=@SomethingId, [Name]=@Name, [Description]=@Description WHERE [Id]=@Id"));

		Assert.IsTrue(statements.Delete.Equals(
			"DELETE [dbo].[CompositeKeyEntity] WHERE [Id]=@Id"));

		Assert.IsTrue(statements.GetByAlternateKey.Equals(
			"SELECT * FROM [dbo].[CompositeKeyEntity] WHERE [SomethingId]=@SomethingId AND [Name]=@Name"));
	}
}