SELECT RecordEntry.Id [REID], Product.Name [Name], RecordEntry.Quantity [Quantity], RecordEntry.Locations [Locations], Product.Id [PID], RecordEntry.RecordId [RecordId] FROM RecordEntry INNER JOIN Product ON RecordEntry.ProductId = Product.Id
--SELECT * FROM Product ORDER BY [Name]
SELECT * FROM Record WHERE Record.Id = '6A61AF2C-ABBB-4E56-B559-C38CB1471EDF'