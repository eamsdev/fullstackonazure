variable "resource_group" {
  type = object({
    location = string
    name     = string
  })
}

variable "suffix" {
  type = string
}

variable "database_credentials" {
  type = object({
    admin_username    = string
    admin_password    = string
  })
  sensitive = true
}

variable "database_remote_ip_range" {
  type = object({
    start = string
    end = string
  })
  sensitive = true
}