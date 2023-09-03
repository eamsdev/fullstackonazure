# Meta
location = "australiaeast"
stack_name = "fullstackonazure"

# App Service
docker_registry_config = {
  image = "newdevpleaseignore/fullstackonazure:latest"
  url = "https://index.docker.io"
  username = "newdevpleaseignore"
}

app_config = {
  sku_name = "F1"
  health_check_path = "/health"
  port = 80
}

database_remote_ip_range = {
  start = "120.156.179.132"
  end = "120.156.179.132"
}

key_vault = {
  global_admin_id = "e5b33509-dcd0-4f94-851e-d69485c965e7"
  pipeline_sp_display_name = "eamsDevAzureDevops"
  vault_name = "fullstackonazureKV"
  vault_secret_names = {
    dbConnectionString = "dbConnectionString"
  }
}
