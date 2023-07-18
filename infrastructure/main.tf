terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.65.0"
    }
  }

  required_version = "~> 1.5.3"
}

provider "azurerm" {
  features {}
}

resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.stack_name}-${random_integer.ri.result}"
  location = var.location
}

resource "azurerm_service_plan" "appserviceplan" {
  name                = "webapp-asp-${var.stack_name}-${random_integer.ri.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = var.app_service.sku_name
}

resource "azurerm_linux_web_app" "webapp" {
  name                = "webapp-${var.stack_name}-${random_integer.ri.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id
  https_only          = true

  lifecycle {
    ignore_changes = [
      # docker image to be managed by the application's CICD pipeline
      site_config[0].application_stack[0].docker_image_name,
    ]
  }

  site_config {
    always_on         = false
    health_check_path = var.app_service.health_check_path

    application_stack {
      docker_image_name        = var.docker.image
      docker_registry_url      = var.docker.url
      docker_registry_username = var.docker.username
      docker_registry_password = var.docker_password
    }
  }

  app_settings = {
    "WEBSITES_PORT" = var.docker.port
  }
}
