FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /AuthenticationApi/src
COPY ["./AuthenticationApi/src/AuthApi.Api/AuthApi.Api.csproj", "AuthApi.Api/"]
COPY ["./AuthenticationApi/src/AuthApi.Application/AuthApi.Application.csproj", "AuthApi.Application/"]
COPY ["./AuthenticationApi/src/AuthApi.Contracts/AuthApi.Contracts.csproj", "AuthApi.Contracts/"]
COPY ["./AuthenticationApi/src/AuthApi.Domain/AuthApi.Domain.csproj", "AuthApi.Domain/"]
COPY ["./AuthenticationApi/src/AuthApi.Infrastructure/AuthApi.Infrastructure.csproj", "AuthApi.Infrastructure/"]
WORKDIR /
COPY ["./SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["./SharedKernel/Directory.Build.props", "SharedKernel/"]
COPY ["./SharedKernel.Infrastructure/SharedKernel.Infrastructure.csproj", "SharedKernel.Infrastructure/"]
COPY ["./SharedKernel.Infrastructure/Directory.Build.props", "SharedKernel.Infrastructure/"]
COPY ["./AuthenticationApi/src/Directory.Build.props", "AuthenticationApi/src/"]
COPY ["./Directory.Build.props", "/"]

WORKDIR /
RUN dotnet restore "SharedKernel/SharedKernel.csproj"
RUN dotnet restore "SharedKernel.Infrastructure/SharedKernel.Infrastructure.csproj"
WORKDIR /
RUN dotnet restore "AuthenticationApi/src/AuthApi.Domain/AuthApi.Domain.csproj"
RUN dotnet restore "AuthenticationApi/src/AuthApi.Contracts/AuthApi.Contracts.csproj"
RUN dotnet restore "AuthenticationApi/src/AuthApi.Application/AuthApi.Application.csproj"
RUN dotnet restore "AuthenticationApi/src/AuthApi.Infrastructure/AuthApi.Infrastructure.csproj"
RUN dotnet restore "AuthenticationApi/src/AuthApi.Api/AuthApi.Api.csproj"

WORKDIR /AuthenticationApi/src
COPY ["./AuthenticationApi/src/AuthApi.Api/.", "AuthApi.Api/"]
COPY ["./AuthenticationApi/src/AuthApi.Application/.", "AuthApi.Application/"]
COPY ["./AuthenticationApi/src/AuthApi.Contracts/.", "AuthApi.Contracts/"]
COPY ["./AuthenticationApi/src/AuthApi.Infrastructure/.", "AuthApi.Infrastructure/"]
COPY ["./AuthenticationApi/src/AuthApi.Domain/.", "AuthApi.Domain/"]
WORKDIR /
COPY ["./SharedKernel/.", "SharedKernel/"]
COPY ["./SharedKernel.Infrastructure/.", "SharedKernel.Infrastructure/"]

WORKDIR "/AuthenticationApi/src/AuthApi.Api"
RUN dotnet build "AuthApi.Api.csproj" -c Release -o /app/build
RUN dotnet publish "AuthApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM --platform=linux/amd64/v8 mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthApi.Api.dll"]
