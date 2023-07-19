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
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}

resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.stack_name}-${random_integer.ri.result}"
  location = var.location
}

module "api" {
  source = "../../modules/appservice"

  resource_group         = azurerm_resource_group.rg
  suffix                 = "${var.stack_name}-${random_integer.ri.result}"
  docker_registry_config = var.docker_registry_config
  app_secrets            = var.app_secrets
  app_config             = var.app_config
}
