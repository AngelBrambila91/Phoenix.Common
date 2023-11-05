How to create a package

dotnet pack -o ..\..\..\packages\

then add on Phoenix.Catalog the nuget package

dotnet nuget add source {URL where you want the packages}\packages -n PhoenixEconomy

Remove the MongoDB.Driver on Catalog

Add Phoenix.Common package

dotnet add package Phoenix.Common

Remove IEntity from Repositories and Settings dir from Catalog

Run Catalog service and try queries.