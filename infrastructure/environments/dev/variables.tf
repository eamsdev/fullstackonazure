variable "location" {
  type    = string
  default = "australiaeast"
}

variable "stack_name" {
  type = string
}

variable "dns" {
  type = object({
    custom_domain_verification_id = string
  })
  sensitive = true
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

variable "database_credentials" {
  type = object({
    admin_username = string
    admin_password = string
  })
  sensitive = true
}

variable "database_remote_ip_range" {
  type = object({
    start = string
    end   = string
  })
}

variable "key_vault" {
  type = object({
    global_admin_id = string
    vault_name      = string
    vault_secret_names = object({
      dbConnectionString = string
    })
  })
}
