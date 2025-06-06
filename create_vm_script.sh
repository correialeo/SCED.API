#!/bin/bash

# variáveis de configuração
resource_group="rg-devops-gs"
vm_name="vm-devops-appgs"
location="eastus"
vm_size="Standard_B1s"  # tamanho econômico para testes
vm_image="Ubuntu2204"
admin_username="azureuser"
nsg_name="nsg-devops-appgs"

# cores para output
red='\033[0;31m'
green='\033[0;32m'
yellow='\033[1;33m'
nc='\033[0m' # no color

echo -e "${green}🚀 iniciando criação da vm para devops gs...${nc}"

# 1. criar resource group
echo -e "${yellow}📁 criando resource group...${nc}"
az group create \
    --name $resource_group \
    --location $location

# 2. criar network security group e regras
echo -e "${yellow}🔒 criando network security group...${nc}"
az network nsg create \
    --resource-group $resource_group \
    --name $nsg_name

# regra ssh (porta 22)
echo -e "${yellow}🔑 configurando regra ssh (porta 22)...${nc}"
az network nsg rule create \
    --resource-group $resource_group \
    --nsg-name $nsg_name \
    --name ssh \
    --protocol tcp \
    --priority 1000 \
    --destination-port-range 22 \
    --access allow

# regra para api (porta 8080)
echo -e "${yellow}🌐 configurando regra para api (porta 8080)...${nc}"
az network nsg rule create \
    --resource-group $resource_group \
    --nsg-name $nsg_name \
    --name api \
    --protocol tcp \
    --priority 1001 \
    --destination-port-range 8080 \
    --access allow

# regra para porta 8081 (se necessário)
echo -e "${yellow}🌐 configurando regra porta 8081...${nc}"
az network nsg rule create \
    --resource-group $resource_group \
    --nsg-name $nsg_name \
    --name port_8081 \
    --protocol tcp \
    --priority 1002 \
    --destination-port-range 8081 \
    --access allow

# regra para mysql (porta 3306) - apenas se precisar acesso externo
echo -e "${yellow}🗄️ configurando regra mysql (porta 3306)...${nc}"
az network nsg rule create \
    --resource-group $resource_group \
    --nsg-name $nsg_name \
    --name mysql \
    --protocol tcp \
    --priority 1003 \
    --destination-port-range 3306 \
    --access allow

# 3. criar vm
echo -e "${yellow}💻 criando virtual machine...${nc}"
az vm create \
    --resource-group $resource_group \
    --name $vm_name \
    --image $vm_image \
    --admin-username $admin_username \
    --generate-ssh-keys \
    --size $vm_size \
    --nsg $nsg_name \
    --public-ip-sku standard

# 4. obter ip público da vm
echo -e "${yellow}🔍 obtendo ip público da vm...${nc}"
public_ip=$(az vm show \
    --resource-group $resource_group \
    --name $vm_name \
    --show-details \
    --query publicIps \
    --output tsv)

echo -e "${green}✅ vm criada com sucesso!${nc}"
echo -e "${green}📍 ip público: $public_ip${nc}"

# 5. aguardar vm ficar pronta
echo -e "${yellow}⏳ aguardando vm ficar totalmente disponível...${nc}"
sleep 30

echo -e "${green}🎉 vm criada com sucesso!${nc}"
echo ""
echo -e "${green}📋 informações da vm:${nc}"
echo -e "resource group: $resource_group"
echo -e "vm name: $vm_name"
echo -e "public ip: $public_ip"
echo -e "username: $admin_username"
echo -e "ssh command: ssh $admin_username@$public_ip"
echo ""
echo -e "${green}🔗 portas abertas:${nc}"
echo -e "ssh: 22"
echo -e "api: 8080"
echo -e "porta adicional: 8081"
echo -e "mysql: 3306"
echo ""
echo -e "${yellow}⚠️ próximos passos:${nc}"
echo -e "${yellow}1. conecte-se na vm para instalar docker:${nc}"
echo -e "   ssh $admin_username@$public_ip"
echo ""
echo -e "${yellow}2. execute os comandos de instalação na vm${nc}"
echo -e "${yellow}3. configure a service connection no azure devops${nc}"
echo -e "${yellow}4. use o ip público ($public_ip) na sua pipeline de release${nc}"