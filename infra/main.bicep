targetScope = 'resourceGroup'

@description('Azure region for the Container Apps resources.')
param location string = 'centralus'

@description('Container App name.')
param containerAppName string = 'fire-safety-equipment-manager-prod-app'

@description('Container Apps managed environment name.')
param containerEnvironmentName string = 'fire-safety-equipment-manager-prod-app-env'

@description('Log Analytics workspace name.')
param logAnalyticsWorkspaceName string = 'fire-safety-equipment-manager-prod-app-logs'

@description('Fully qualified container image, for example ghcr.io/owner/repository:sha.')
param containerImage string

@description('Container registry server.')
param containerRegistryServer string = 'ghcr.io'

@description('Container registry username. Required when the registry is private.')
param containerRegistryUsername string = ''

@secure()
@description('Container registry password or PAT. Required when the registry is private.')
param containerRegistryPassword string = ''

@description('Application environment name exposed to ASP.NET Core.')
param aspNetCoreEnvironment string = 'Development'
var hasRegistryCredentials = !empty(containerRegistryUsername) && !empty(containerRegistryPassword)

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource containerEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: containerEnvironmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

var databaseConnectionSecrets = [
]

var customEnvironmentSecrets = [
]

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: containerEnvironment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        allowInsecure: false
      }
      secrets: concat(hasRegistryCredentials ? [
        {
          name: 'container-registry-password'
          value: containerRegistryPassword
        }
      ] : [], databaseConnectionSecrets, customEnvironmentSecrets)
      registries: hasRegistryCredentials ? [
        {
          server: containerRegistryServer
          username: containerRegistryUsername
          passwordSecretRef: 'container-registry-password'
        }
      ] : []
    }
    template: {
      containers: [
        {
          name: 'web'
          image: containerImage
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: aspNetCoreEnvironment
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 1
      }
    }
  }
}

output containerAppUrl string = 'https://${containerApp.properties.configuration.ingress.fqdn}'
