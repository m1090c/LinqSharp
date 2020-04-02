# LinqSharp

**LinqSharp** is a smart **Linq** extension library that allows you to write simpler code to generate complex queries, perform data checks, customize storage logic, and more commonly used functions.

- [English Readme](https://github.com/zmjack/LinqSharp/blob/master/README.md)
- [中文自述](https://github.com/zmjack/LinqSharp/blob/master/README-CN.md)

<br/>

**LinqSharp** provides the following enhancements to **Entity Frameowk** according to different application scenarios:

- Query expansion (enhanced SQL generation, enhanced memory query)

- Dynamic Linq

- Data checking pattern (to facilitate data consistency checking)

- Auxiliary tools for database generation (compound primary key, field index)

- Database custom function mapping (enhanced SQL generation, such as RAND functions)

- Custom storage extension (data format adjustment, complex data storage, encrypted storage, etc.)

- Column storage agent (global registration information)



**Supported version of Entity Framework:** 

- Entity Framework Core 2.0+

**Restricted supported version of Entity Framework: **

- Entity Framework Core 3.0+ : some internal API changes, not fully supported.



## How to try it?

The example USES the **Northwnd** library as a data source to demonstrate its use.

You can install **Northwnd** through **Nuget**：

```powershell
install-package Northwnd
```

![](https://raw.githubusercontent.com/zmjack/Northwnd/master/Northwnd/%40Resources/Northwnd/Northwnd.png)

**Northwnd** provides code-first **Northwnd** database definitions and **Sqlite** data sources.

You can make a simple query attempt using the following code, and you can output the generated **SQL** statements using the **ToSql** method:

```C#
using (var sqlite = NorthwndContext.UseSqliteResource())
{
    var query = sqlite.Shippers.Where(x => x.CompanyName == "Speedy Express");
    var sql = query.ToSql();
}
```

```sqlite
SELECT "x"."ShipperID", "x"."CompanyName", "x"."Phone"
FROM "Shippers" AS "x"
WHERE "x"."CompanyName" = 'Speedy Express';
```

Use **NorthwndContext.UseSqliteResource()** method to use the default sqlite file:

> **%userprofile%/.nuget/northwnd/{version}/content/@Resources/Northwnd/northwnd.db**



## Extension Simples

### WhereSearch

  Queries records which is contains the specified string in one or any fields.

  For example, if you want to query ***Sweet*** in the field ***Description*** (table **Categories**):

  ```C#
  sqlite.Employees.WhereSearch("Steven", x => x.FirstName);
  ```

  This invoke will generate a SQL query string:

  ```sqlite
  SELECT *
  FROM "Categories" AS "x"
  WHERE instr("x"."FirstName", 'Steven') > 0;
  ```

----

  And, if you want to query ***An*** in the field ***FirstName*** or ***LastName*** (table ***employees***):

  ```C#
  sqlite.Employees.WhereSearch("An", x => new { x.FirstName, x.LastName })
  ```

  The SQL is:

  ```sqlite
  SELECT *
  FROM "Employees" AS "x"
  WHERE (instr("x"."FirstName", 'An') > 0) OR (instr("x"."LastName", 'An') > 0);
  ```

----

  As you see, this method supports some abilities to search single string in more than one field.

  In some complex scenarios, we also allowed you to query a string in any table which is connected by foreign keys.

  For example, if you want to query who sold product to customer ***QUICK***:

  ```c#
  sqlite.Employees.WhereSearch("QUICK", x => x.Orders.Select(o => o.CustomerID));
  ```

  ```sqlite
  SELECT *
  FROM "Employees" AS "x"
  WHERE EXISTS (
      SELECT 1
      FROM "Orders" AS "o"
      WHERE (instr("o"."CustomerID", 'QUICK') > 0) AND ("x"."EmployeeID" = "o"."EmployeeID"));
  ```

----

  In addition, we may also need to use some other special queries. For example, if you want to search for another string in many fields.

  This is an example of querying ***ToFu*** and ***pkg*** in the fields ***ProductName*** and ***QuantityPerUnit***.

  ```c#
  sqlite.Products.WhereSearch(new[] { "Tofu", "pkg" }, x => new 
  { 
  	x.ProductName, x.QuantityPerUnit
  })
  ```

  ```sqlite
  SELECT "x"."ProductID", "x"."CategoryID", "x"."Discontinued", "x"."ProductName", "x"."QuantityPerUnit", "x"."ReorderLevel", "x"."SupplierID", "x"."UnitPrice", "x"."UnitsInStock", "x"."UnitsOnOrder"
  FROM "Products" AS "x"
  WHERE ((instr("x"."ProductName", 'Tofu') > 0) OR (instr("x"."QuantityPerUnit", 'Tofu') > 0)) AND ((instr("x"."ProductName", 'pkg') > 0) OR (instr("x"."QuantityPerUnit", 'pkg') > 0));
  ```

### WhereMatch
  Different from **WhereSearch**, this statement will perform an exact match:

  ```mssql
  /* SQL Server */
  SELECT [x].[Id], [x].[First_Name], [x].[Last_Name]
  FROM [Emplyees] AS [x]
  WHERE [x].[First_Name] = N'Bill' 
  	OR [x].[Last_Name] = N'Bill'
  ```

  ```mysql
  /* MySql */
  SELECT `x`.`Id`, `x`.`First_Name`, `x`.`Last_Name`
  FROM `Emplyees` AS `x`
  WHERE `x`.`First_Name` = 'Bill' 
  	OR `x`.`Last_Name` = 'Bill'
  ```

### WhereBetween
  Queries records which is start at a specified time and end at another time.

  Note: Support type **Nullable\<DateTime\>**: If member expression's result is null, then the main expression's result is false. Here is the simple:

  ```c#
  sqlite.Employees.WhereBetween(x => x.BirthDate, 
  	new DateTime(1960, 5, 1), new DateTime(1960, 5, 31));
  ```

  ```sqlite
  SELECT "x"."EmployeeID", "x"."Address", "x"."BirthDate", "x"."City", "x"."Country", "x"."Extension", "x"."FirstName", "x"."HireDate", "x"."HomePhone", "x"."LastName", "x"."Notes", "x"."Photo", "x"."PhotoPath", "x"."PostalCode", "x"."Region", "x"."ReportsTo", "x"."Title", "x"."TitleOfCourtesy"
  FROM "Employees" AS "x"
  WHERE CASE
      WHEN "x"."BirthDate" IS NOT NULL
      THEN CASE
          WHEN ('1960-05-01 00:00:00' <= "x"."BirthDate") AND ("x"."BirthDate" <= '1960-05-31 00:00:00')
          THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
      END ELSE CAST(0 AS BIT)
  END = 1;
  ```

### WhereBefore

### WhereAfter

### WhereMax
  Selects the entire record with the largest value for a field.

### WhereMin
  Selects the entire record with the smallest value for a field.

### OrderByCase
  Queries records and order the result by a specified sequence.

  ```c#
  sqlite.Regions
  	.OrderByCase(x => x.RegionDescription, 
      	new[] { "Northern", "Eastern", "Western", "Southern" });
  ```

  ```sqlite
  SELECT "x"."RegionID", "x"."RegionDescription"
  FROM "Region" AS "x"
  ORDER BY CASE
      WHEN "x"."RegionDescription" = 'Northern'
      THEN 0 ELSE CASE
          WHEN "x"."RegionDescription" = 'Eastern'
          THEN 1 ELSE CASE
              WHEN "x"."RegionDescription" = 'Western'
              THEN 2 ELSE CASE
                  WHEN "x"."RegionDescription" = 'Southern'
                  THEN 3 ELSE 4
              END
          END
      END
  END;
  ```

### OrderByCaseDescending
  Same as **OrderByCase**, but use descending order.

### ThenByCase

  ```c#
  sqlite.Regions
  	.OrderByCase(x => x.RegionDescription, 
      	new[] { "Northern", "Eastern", "Western", "Southern" })
      .ThenByCase(x => x.RegionID, new[] { 4, 3, 2, 1 })
  ```

  ```sqlite
  SELECT "x"."RegionID", "x"."RegionDescription"
  FROM "Region" AS "x"
  ORDER BY CASE
      WHEN "x"."RegionDescription" = 'Northern'
      THEN 0 ELSE CASE
          WHEN "x"."RegionDescription" = 'Eastern'
          THEN 1 ELSE CASE
              WHEN "x"."RegionDescription" = 'Western'
              THEN 2 ELSE CASE
                  WHEN "x"."RegionDescription" = 'Southern'
                  THEN 3 ELSE 4
              END
          END
      END
  END, CASE
      WHEN "x"."RegionID" = 4
      THEN 0 ELSE CASE
          WHEN "x"."RegionID" = 3
          THEN 1 ELSE CASE
              WHEN "x"."RegionID" = 2
              THEN 2 ELSE CASE
                  WHEN "x"."RegionID" = 1
                  THEN 3 ELSE 4
              END
          END
      END
  END;
  ```

### ThenByCaseDescending
  Same as **ThenByCase**, but use descending order.

### WhereOr

  ```C#
  sqlite.Employees.WhereOr(sqlite.Employees
  	.GroupBy(x => x.TitleOfCourtesy)
  	.Select(g => new
  	{
  		TitleOfCourtesy = g.Key,
  		BirthDate = g.Max(x => x.BirthDate),
  	}));
  ```

  This invoke will generated two SQL string, the first is:

  ```sqlite
  SELECT "x"."TitleOfCourtesy", MAX("x"."BirthDate") AS "BirthDate"
  FROM "Employees" AS "x"
  GROUP BY "x"."TitleOfCourtesy";
  ```

  the follow SQL will use all the field of the first result as it's where condition. So, the follow SQL string is:

  ```sqlite
  SELECT "e"."EmployeeID", "e"."Address", "e"."BirthDate", "e"."City", "e"."Country", "e"."Extension", "e"."FirstName", "e"."HireDate", "e"."HomePhone", "e"."LastName", "e"."Notes", "e"."Photo", "e"."PhotoPath", "e"."PostalCode", "e"."Region", "e"."ReportsTo", "e"."Title", "e"."TitleOfCourtesy"
  FROM "Employees" AS "e"
  WHERE (((("e"."TitleOfCourtesy" = 'Dr.') AND ("e"."BirthDate" = '1952-02-19 00:00:00')) OR (("e"."TitleOfCourtesy" = 'Mr.') AND ("e"."BirthDate" = '1963-07-02 00:00:00'))) OR (("e"."TitleOfCourtesy" = 'Mrs.') AND ("e"."BirthDate" = '1937-09-19 00:00:00'))) OR (("e"."TitleOfCourtesy" = 'Ms.') AND ("e"."BirthDate" = '1966-01-27 00:00:00'));
  ```

### TryUpdate

  ```C#
  sqlite.Orders
  	.TryUpdate(x => x.Order_Details.Any(y => y.Discount >= 0.02))
      .Set(x => x.ShipCity, "Reims")
      .Save();
  ```

  ```sqlite
  UPDATE "Orders" SET "ShipCity"='Reims' WHERE EXISTS (
      SELECT 1
      FROM "Order Details" AS "y"
      WHERE ("y"."Discount" >= 0.02) AND ("Orders"."OrderID" = "y"."OrderID"));
  ```

  **Next feature**: to support set a value which is calculated by the specified entity ***x***.

### TryDelete

  ```C#
  sqlite.Orders
  	.TryDelete(x => x.Order_Details.Any(y => y.Discount >= 0.02))
      .Save();
  ```

  ```sqlite
  DELETE FROM "Orders" WHERE EXISTS (
      SELECT 1
      FROM "Order Details" AS "y"
      WHERE ("y"."Discount" >= 0.02) AND ("Orders"."OrderID" = "y"."OrderID"));
  ```

