#Provisionando os recursos inicialmente necessarios

variable "subscription_id" {
  default = "FB54EEAA38ED3D1AB541611FD082D9A7D806FA25"
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

resource "azurerm_resource_group" "dio_flix" {
  name     = "dio-flix"
  location = "East US"
}

resource "azurerm_api_management" "apimgn_flix_dio" {
  name                = "apimgn-flix-dio"
  location            = azurerm_resource_group.dio_flix.location
  resource_group_name = azurerm_resource_group.dio_flix.name
  sku {
    name     = "Developer"
    capacity = 1
  }
  publisher_email = "flixdio@apimgnflixdio.com"
  organization_name = "flixdio4all"
}

resource "azurerm_cosmosdb_account" "flixdiodevoneinnovation" {
  name                = "flixdiodevoneinnovation"
  location            = azurerm_resource_group.dio_flix.location
  resource_group_name = azurerm_resource_group.dio_flix.name
  kind                = "GlobalDocumentDB"
  consistency_policy {
    consistency_level = "Session"
  }
  capabilities {
    enable_cassandra = false
    enable_gremlin = false
    enable_table = false
    enable_mongo_db = false
  }
  offerings {
    name = "Standard"
  }
}

resource "azurerm_storage_account" "saflixdioforstreams" {
  name                = "saflixdioforstreams"
  resource_group_name = azurerm_resource_group.dio_flix.name
  location            = azurerm_resource_group.dio_flix.location
  account_tier        = "Standard"
  account_replication_type = "LRS"

  static_website {
    index_document = "index.html"
    error_404_document = "404.html"
  }
}

resource "azurerm_storage_container" "public_container" {
  name                = "public-container"
  storage_account_name = azurerm_storage_account.saflixdioforstreams.name
  container_access_type = "blob"

  public_access {
    blob = "container"
  }
}

  
