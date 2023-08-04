resource "azurerm_log_analytics_workspace" "ws" {
  name                = "ws-${var.suffix}"
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "insight" {
  name                = "insight-${var.suffix}"
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name
  workspace_id        = azurerm_log_analytics_workspace.ws.id
  application_type    = "web"
}

resource "azurerm_service_plan" "appserviceplan" {
  name                = "webapp-asp-${var.suffix}"
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name
  os_type             = "Linux"
  sku_name            = var.app_config.sku_name
}

resource "azurerm_linux_web_app" "webapp" {
  name                = "webapp-${var.suffix}"
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id
  https_only          = true

  identity {
    type = "SystemAssigned"
  }

  lifecycle {
    ignore_changes = [
      # docker image to be managed by the application's CICD pipeline
      site_config[0].application_stack[0].docker_image_name,
    ]
  }

  site_config {
    always_on         = false
    health_check_path = var.app_config.health_check_path

    application_stack {
      docker_image_name        = var.docker_registry_config.image
      docker_registry_url      = var.docker_registry_config.url
      docker_registry_username = var.docker_registry_config.username
      docker_registry_password = var.app_secrets.docker_registry_password
    }
  }

  app_settings = {
    "WEBSITES_PORT"                                   = var.app_config.port
    "APPINSIGHTS_INSTRUMENTATIONKEY"                  = azurerm_application_insights.insight.instrumentation_key
    "APPINSIGHTS_PROFILERFEATURE_VERSION"             = "1.0.0"
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION"             = "1.0.0"
    "APPLICATIONINSIGHTS_CONNECTION_STRING"           = azurerm_application_insights.insight.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION"      = "~3"
    "DiagnosticServices_EXTENSION_VERSION"            = "~3"
    "InstrumentationEngine_EXTENSION_VERSION"         = "~1"
    "SnapshotDebugger_EXTENSION_VERSION"              = "disabled"
    "XDT_MicrosoftApplicationInsights_BaseExtensions" = "~1"
    "XDT_MicrosoftApplicationInsights_Mode"           = "recommended"
    "XDT_MicrosoftApplicationInsights_PreemptSdk"     = "disabled"
    "ConnectionStrings__Database"                     = "@Microsoft.KeyVault(VaultName=${var.key_vault.vault_name};SecretName=${var.key_vault.vault_secret_names.dbConnectionString})"
  }
}
