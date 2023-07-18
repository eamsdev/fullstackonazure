variable "location" {
  type = string
  default = "australiaeast"
}

variable "stack_name" {
  type = string
}

variable "docker" {
  type = object({
    image = string 
    url = string
    username = string
    port = number
  })
}

variable "docker_password" {
  type = string
  sensitive = true
}

variable "app_service" {
  type = object({
    sku_name = string
    health_check_path = string
  })
}