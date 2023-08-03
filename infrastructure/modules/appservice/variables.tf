variable "resource_group" {
  type = object({
    location = string
    name     = string
  })
}

variable "suffix" {
  type = string
}

variable "docker_registry_config" {
  type = object({
    image    = string
    url      = string
    username = string
  })
}

variable "app_secrets" {
  type = object({
    docker_registry_password = string
  })
  sensitive = true
}

variable "app_config" {
  type = object({
    sku_name          = string
    health_check_path = string
    port              = number
  })
}

variable "connectionstrings_database" {
  type = string
  sensitive = true
}