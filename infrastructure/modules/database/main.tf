resource "azurerm_storage_account" "sqlsa" {
  name                     = "sql-storage-account-${var.suffix}"
  location                 = var.resource_group.location
  resource_group_name      = var.resource_group.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_mssql_server" "mssqlserver" {
  name                         = "mssqlserver-${var.suffix}"
  location                     = var.resource_group.location
  resource_group_name          = var.resource_group.name
  version                      = "12.0"
  administrator_login          = var.database_credentials.admin_username
  administrator_login_password = var.database_credentials.admin_password
}

resource "azurerm_mssql_database" "mssqldatabase" {
  name         = "mssqldatabase-${var.suffix}"
  server_id    = azurerm_mssql_server.mssqlserver.id
  collation    = "SQL_Latin1_General_CP1_CI_AS"
  license_type = "LicenseIncluded"
  max_size_gb  = 2
  sku_name     = "Basic"
}

resource "azurerm_mssql_database_extended_auditing_policy" "auditing_policy" {
  database_id                             = azurerm_mssql_database.mssqldatabase.id
  storage_endpoint                        = azurerm_storage_account.sqlsa.primary_blob_endpoint
  storage_account_access_key              = azurerm_storage_account.sqlsa.primary_access_key
  storage_account_access_key_is_secondary = false
  retention_in_days                       = 6
}
