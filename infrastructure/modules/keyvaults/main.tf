data "azurerm_client_config" "current" {}

data "azuread_user" "global_admin" {
  object_id = var.key_vault.global_admin_id
}

resource "azurerm_key_vault" "key_vault" {
  name                        = var.key_vault.vault_name
  location                    = var.resource_group.location
  resource_group_name         = var.resource_group.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"
}

resource "azurerm_key_vault_access_policy" "global_admin_access" {
  key_vault_id = azurerm_key_vault.key_vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azuread_user.global_admin.object_id

  secret_permissions  = ["Backup", "Delete", "Get", "List", "Purge", "Recover", "Restore", "Set", ]
  key_permissions     = ["Backup", "Create", "Decrypt", "Delete", "Encrypt", "Get", "Import", "List", "Purge", "Recover", "Restore", "Sign", "UnwrapKey", "Update", "Verify", "WrapKey", ]
  storage_permissions = ["Backup", "Delete", "DeleteSAS", "Get", "GetSAS", "List", "ListSAS", "Purge", "Recover", "RegenerateKey", "Restore", "Set", "SetSAS", "Update", ]

  depends_on = [azurerm_key_vault.key_vault]
}

resource "azurerm_key_vault_access_policy" "webapp_get_access" {
  key_vault_id = azurerm_key_vault.key_vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = var.webapp_principal_id

  secret_permissions = ["Get"]

  depends_on = [azurerm_key_vault.key_vault]
}

resource "azurerm_key_vault_secret" "secrets" {
  for_each = toset(values(var.key_vault.vault_secret_names))

  name         = each.value
  value        = "changeme"
  key_vault_id = azurerm_key_vault.key_vault.id

  lifecycle {
    ignore_changes = [
      # secrets value managed manually
      value,
    ]
  }

  depends_on = [
    azurerm_key_vault_access_policy.global_admin_access,
    azurerm_key_vault_access_policy.webapp_get_access
  ]
}
