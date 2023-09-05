resource "azurerm_storage_account" "static_website_storage" {
  name                     = "${var.suffix}blob"
  location                 = var.resource_group.location
  resource_group_name      = var.resource_group.name
  account_replication_type = "LRS"
  account_tier             = "Standard"
  min_tls_version          = "TLS1_2"

  static_website {
    index_document     = "index.html"
    error_404_document = "index.html"
  }
}
