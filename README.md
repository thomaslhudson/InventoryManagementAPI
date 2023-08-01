# Introduction

This Web API is used by the [Inventory Management Mobile app](https://github.com/thomaslhudson/InventoryManagement.Mobile) to communicate with MS SQL Server.

#### Data Connection String

The data connection string must be updated in the appsettings.json file before compilation and must include the database name "InventoryManagement" (e.g. "Database=InventoryManagement", "Initial Catalog=InventoryManagement").

#### Creating The Database Schema

After updating the data connection string, build and run from within Visual Studio. This automatically creates the database schema and opens the Swagger UI. 

#### Populating Initial Data

Initial data can be added to the database for Groups and Products from the Swagger UI. Due to the 1:N relationship between Groups and Products, the Group data must be populated first, then the Product data.

If initial data is not added using the Swagger UI, each individual Group will need to be added one-at-a-time through the mobile app. Once groups are added, each Product will need to be added one-at-a-time.

###### Populate Groups

Run the last POST command in Group section of the Swagger UI ([POST] /api/Group/PopulateGroups) and confirm that it was successful.

###### Populate Products

After successfully populating the Group table with initial data (see above), run the last POST command in the Product section of the Swagger UI ([POST] /api/Product/PopulateProducts) and confirm that is was successful. Note: each Product record created this way will be assigned a random UPC which doesn't break anything. However, to use the scanning feature of the mobile app, each product will need to be updated with it's actual UPC. Updating the UPC for a product can be done with the mobile app.

#### Webserver

Configuring a web server is beyond the scope of this document but the URL for the Web API will be needed when configuring and using the mobile app.

#### Mobile App User Manual

See the [Mobile App User Manual](https://github.com/thomaslhudson/InventoryManagement.Mobile/wiki/User-Manual-Wiki) for instructions on setting up and using the mobile app.
