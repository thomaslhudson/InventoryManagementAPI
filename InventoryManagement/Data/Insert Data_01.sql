USE [InventoryManagement]
GO
CREATE USER [BarInvAPI] FOR LOGIN [BarInvAPI] WITH DEFAULT_SCHEMA=[dbo]
GO

USE [InventoryManagement]
GO
INSERT [dbo].[Group] ([Name]) VALUES (N'Beer On Tap')
INSERT [dbo].[Group] ([Name]) VALUES (N'Bitters / Syrups')
INSERT [dbo].[Group] ([Name]) VALUES (N'Bottled Beer')
INSERT [dbo].[Group] ([Name]) VALUES (N'Cooking Wine')
INSERT [dbo].[Group] ([Name]) VALUES (N'Gin')
INSERT [dbo].[Group] ([Name]) VALUES (N'Grappa / Sambuca / Eau de Vie')
INSERT [dbo].[Group] ([Name]) VALUES (N'Liqueurs')
INSERT [dbo].[Group] ([Name]) VALUES (N'Non-Alcoholic Beverages')
INSERT [dbo].[Group] ([Name]) VALUES (N'Red Wine')
INSERT [dbo].[Group] ([Name]) VALUES (N'Rum / Cachaça')
INSERT [dbo].[Group] ([Name]) VALUES (N'Sparkling / Rose / Dessert / Fortified')
INSERT [dbo].[Group] ([Name]) VALUES (N'Tequila / Mezcal')
INSERT [dbo].[Group] ([Name]) VALUES (N'Vermouth')
INSERT [dbo].[Group] ([Name]) VALUES (N'Vodka')
INSERT [dbo].[Group] ([Name]) VALUES (N'Whiskey / Bourbon / Scotch / Irish')
INSERT [dbo].[Group] ([Name]) VALUES (N'White Wine')
GO

USE [InventoryManagement]
GO
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'Charbay Grapefruit', N'123654789165', CAST(25.000 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'Golden State Vodka', N'723159769822', CAST(0.000 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'7 Leguas Tequila Anejo', N'537198246756', CAST(41.800 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'7 Leguas Tequla Blanco', N'654009321755', CAST(35.000 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'Black Cow', N'753159769823', CAST(25.000 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'Balvenie Doublewood 17yr', N'321789456911', CAST(113.300 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'Amador', N'123987654237', CAST(33.800 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'7 Leguas Tequila Reposado', N'654789321755', CAST(41.800 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
INSERT [dbo].[Product] ([Name], [Upc], [UnitPrice], [GroupId]) VALUES (N'Belvedere', N'753159769521', CAST(36.740 AS Decimal(9, 3)), N'DB7AF1C7-7329-ED11-9DAD-240A640007EF')
GO
