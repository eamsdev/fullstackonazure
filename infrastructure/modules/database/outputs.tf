output "azurerm_mssql_server" {
  value = {
    id                          = azurerm_mssql_server.mssqlserver.id
    fully_qualified_domain_name = azurerm_mssql_server.mssqlserver.fully_qualified_domain_name
  }
}

output "azurerm_mssql_database" {
  value = {
    id   = azurerm_mssql_database.mssqldatabase.id
    name = azurerm_mssql_database.mssqldatabase.name
  }
}
