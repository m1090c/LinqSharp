﻿using LinqSharp.EFCore.Data.Test;
using Northwnd;
using System;

namespace DbCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sqlite = NorthwndContext.UseSqliteResource())
            using (var mysql = ApplicationDbContext.UseMySql())
            using (var sqlserver = ApplicationDbContext.UseSqlServer())
            {
                sqlite.WriteTo(mysql);
                sqlite.WriteTo(sqlserver);
            }

            Console.WriteLine("Complete");
        }
    }
}
