﻿using LinqSharp.EFCore.Data;
using LinqSharp.EFCore.Data.Test;
using System;
using System.Linq;
using Xunit;

namespace LinqSharp.EFCore.Test
{
    public class MySqlTests
    {
        [Fact]
        public void Test1()
        {
            using var db = ApplicationDbContext.UseMySql();
            using (var trans = db.Database.BeginTransaction())
            {
                db.YearMonthModels.AddRange(new[]
                {
                    new YearMonthModel { Date = new DateTime(2012, 1, 1), Year = 2012, Month = 1, Day = 1 },
                    new YearMonthModel { Date = new DateTime(2012, 4, 16), Year = 2012, Month = 4, Day = 16 },
                    new YearMonthModel { Date = new DateTime(2012, 5, 18), Year = 2012, Month = 5, Day = 18 },
                });
                db.SaveChanges();

                var query = db.YearMonthModels.Where(x => DbFunc.DateTime(x.Year, x.Month, x.Day) >= DbFunc.DateTime(2012, 4, 16));
                var sql = query.ToSql();
                Assert.Equal(2, query.Count());

                trans.Rollback();
            }
        }

    }
}
