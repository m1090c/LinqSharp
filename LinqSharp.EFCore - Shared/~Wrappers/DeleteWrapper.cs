﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace LinqSharp.EFCore
{
    public class DeleteWrapper<TEntity>
        where TEntity : class
    {
        public WhereWrapper<TEntity> WhereWrapper { get; }

        public DeleteWrapper(WhereWrapper<TEntity> whereWrapper)
        {
            WhereWrapper = whereWrapper;
        }

        public string ToSql()
        {
            return $"DELETE FROM {WhereWrapper.TableName} WHERE {WhereWrapper.WhereString};";
        }

        public int Save() => WhereWrapper.DbContext.Database.ExecuteSqlCommand(ToSql());
    }
}
