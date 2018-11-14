configs for cluster pods

## - these two stand up the service and persistant storage in the cluster (kubectl apply -f mindcraftserver-aks-config.yml)
mindcraftserver-aks-config.yml

## - These files configure the azure file and the roles
azure-file-sc.yaml
azure-pvc-roles.yaml

## - This file was used to apply the rbac for the initial cluster creation
rbac.yaml
