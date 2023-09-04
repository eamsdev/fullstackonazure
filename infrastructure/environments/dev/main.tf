terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.65.0"
    }

    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.9.0"
    }
  }

  required_version = "~> 1.5.3"
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }

    key_vault {
      purge_soft_delete_on_destroy = true
    }
  }
}

locals {
  suffix = "${var.stack_name}"
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${local.suffix}"
  location = var.location
}

module "frontend" {
  source = "../../modules/staticwebapp"

  suffix         = local.suffix
  resource_group = azurerm_resource_group.rg
}

module "db" {
  source = "../../modules/database"

  suffix                   = local.suffix
  resource_group           = azurerm_resource_group.rg
  database_credentials     = var.database_credentials
  database_remote_ip_range = var.database_remote_ip_range
}

module "api" {
  source = "../../modules/appservice"

  suffix                 = local.suffix
  resource_group         = azurerm_resource_group.rg
  docker_registry_config = var.docker_registry_config
  app_secrets            = var.app_secrets
  app_config             = var.app_config
  key_vault              = var.key_vault
}

module "secrets" {
  source = "../../modules/keyvaults"

  suffix              = local.suffix
  key_vault           = var.key_vault
  webapp_principal_id = module.api.webapp.identity[0].principal_id
  resource_group      = azurerm_resource_group.rg
}

