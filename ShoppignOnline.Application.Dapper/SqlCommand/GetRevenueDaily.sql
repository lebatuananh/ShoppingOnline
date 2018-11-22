USE [ShoppingOnline]
GO
/****** Object:  StoredProcedure [dbo].[GetRevenueDaily]    Script Date: 11/10/2018 9:35:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROC [dbo].[GetRevenueDaily]
	@fromDate VARCHAR(10),
	@toDate VARCHAR(10)
AS
BEGIN
		  select
                CAST(b.DateCreated AS DATE) as Date,
                sum(bd.Quantity*bd.Price) as Revenue,
                sum((bd.Quantity*bd.Price)-(bd.Quantity * p.OriginalPrice)) as Profit
                from Bills b
                inner join dbo.BillDetails bd
                on b.Id = bd.BillId
                inner join Products p
                on bd.ProductId  = p.Id
                where convert(date,b.DateCreated) <= @toDate 
				AND convert(date,b.DateCreated) >= @fromDate
                group by b.DateCreated
END
