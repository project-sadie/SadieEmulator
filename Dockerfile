FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY . .
RUN dotnet build
ENTRYPOINT ["dotnet", "run", "--project=Sadie.Console"]