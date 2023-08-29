variable "webapp_principal_id" {
  type = string
}

variable "resource_group" {
  type = object({
    location = string
    name     = string
  })
}

variable "suffix" {
  type = string
}

variable "key_vault" {
  type = object({
    global_admin_id          = string
    pipeline_sp_display_name = string
    vault_name               = string
    vault_secret_names = object({
      dbConnectionString = string
    })
  })
}
