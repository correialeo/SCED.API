
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG DATABASE__SERVER
ARG DATABASE__NAME
ARG DATABASE__PORT
ARG DATABASE__USER
ARG DATABASE__PASSWORD

ENV DATABASE__SERVER $DATABASE__SERVER
ENV DATABASE__NAME $DATABASE__NAME
ENV DATABASE__PORT $DATABASE__PORT
ENV DATABASE__USER $DATABASE__USER
ENV DATABASE__PASSWORD $DATABASE__PASSWORD

WORKDIR /App

ENV PATH=$PATH:/root/.dotnet/tools
RUN dotnet tool install --global dotnet-ef

COPY . ./
RUN dotnet restore SCED.API.sln

RUN dotnet publish SCED.API.sln -c Release -o out

RUN groupadd -r appuser && useradd -r -g appuser -m appuser

USER appuser
ENV PATH=$PATH:/home/appuser/.dotnet/tools
RUN dotnet tool install --global dotnet-ef

USER root

WORKDIR /App/out

RUN chown -R appuser:appuser /App/out

EXPOSE 8080
EXPOSE 8081

USER appuser

ENTRYPOINT ["dotnet", "SCED.API.dll"]