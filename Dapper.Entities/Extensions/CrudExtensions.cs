﻿using Dapper.Entities.Interfaces;
using System.Data;

namespace Dapper.Entities.Extensions;

public static class CrudExtensions
{
	/// <summary>
	/// global SQL builder convention -- set this in startup before using any of the crud extension methods
	/// </summary>
	public static ISqlBuilder? SqlBuilder { get; set; }

	private static readonly Dictionary<Type, SqlStatements> TypeSqlBuilders = [];

	public static async Task<TEntity?> GetAsync<TEntity, TKey>(this IDbConnection connection, TKey id, IDbTransaction? transaction = null)
		where TEntity : IEntity<TKey>
		where TKey : struct
	{
		var sql = GetSqlStatements<TEntity, TKey>().GetById;
		return await connection.QuerySingleOrDefaultAsync<TEntity>(sql, new { id }, transaction);
	}

	public static async Task<TEntity> InsertAsync<TEntity, TKey>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null)
		where TEntity : IEntity<TKey>
		where TKey : struct
	{
		var sql = GetSqlStatements<TEntity, TKey>().Insert;
		var id = await connection.ExecuteScalarAsync<TKey>(sql, entity, transaction);
		entity.Id = id;

		return entity;
	}

	public static async Task UpdateAsync<TEntity, TKey>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null)
		where TEntity : IEntity<TKey>
		where TKey : struct
	{
		var sql = GetSqlStatements<TEntity, TKey>().Update;
		await connection.ExecuteAsync(sql, entity, transaction);
	}

	public static async Task DeleteAsync<TEntity, TKey>(this IDbConnection connection, TKey id, IDbTransaction? transaction = null)
		where TEntity : IEntity<TKey>
		where TKey : struct
	{
		var sql = GetSqlStatements<TEntity, TKey>().Delete;
		await connection.ExecuteAsync(sql, new { id }, transaction);
	}

	private static SqlStatements GetSqlStatements<TEntity, TKey>()
		where TEntity : IEntity<TKey>
		where TKey : struct
	{
		ArgumentNullException.ThrowIfNull(SqlBuilder, nameof(SqlBuilder));

		if (!TypeSqlBuilders.ContainsKey(typeof(TEntity)))
		{
			TypeSqlBuilders.Add(typeof(TEntity), SqlBuilder.BuildStatements(typeof(TEntity)));
		}

		return TypeSqlBuilders[typeof(TEntity)];		
	}
}
