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
  }
}

locals {
  suffix = "${var.stack_name}-${random_integer.ri.result}"
}

resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${local.suffix}"
  location = var.location
}


module "db" {
  source = "../../modules/database"

  suffix               = local.suffix
  resource_group       = azurerm_resource_group.rg
  database_credentials = var.database_credentials
}

module "api" {
  source = "../../modules/appservice"

  suffix                 = local.suffix
  resource_group         = azurerm_resource_group.rg
  docker_registry_config = var.docker_registry_config
  app_secrets            = var.app_secrets
  app_config             = var.app_config
}


