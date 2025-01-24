FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /CleanTemplate/src
COPY ["./CleanTemplate/src/CleanTemplate.Api/CleanTemplate.Api.csproj", "CleanTemplate.Api/"]
COPY ["./CleanTemplate/src/CleanTemplate.Application/CleanTemplate.Application.csproj", "CleanTemplate.Application/"]
COPY ["./CleanTemplate/src/CleanTemplate.Contracts/CleanTemplate.Contracts.csproj", "CleanTemplate.Contracts/"]
COPY ["./CleanTemplate/src/CleanTemplate.Domain/CleanTemplate.Domain.csproj", "CleanTemplate.Domain/"]
COPY ["./CleanTemplate/src/CleanTemplate.Infrastructure/CleanTemplate.Infrastructure.csproj", "CleanTemplate.Infrastructure/"]
WORKDIR /
COPY ["./SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["./SharedKernel/Directory.Build.props", "SharedKernel/"]
COPY ["./SharedKernel.Infrastructure/SharedKernel.Infrastructure.csproj", "SharedKernel.Infrastructure/"]
COPY ["./SharedKernel.Infrastructure/Directory.Build.props", "SharedKernel.Infrastructure/"]
COPY ["./CleanTemplate/src/Directory.Build.props", "CleanTemplate/src/"]
COPY ["./Directory.Build.props", "/"]

WORKDIR /
RUN dotnet restore "SharedKernel/SharedKernel.csproj"
RUN dotnet restore "SharedKernel.Infrastructure/SharedKernel.Infrastructure.csproj"
WORKDIR /
RUN dotnet restore "CleanTemplate/src/CleanTemplate.Domain/CleanTemplate.Domain.csproj"
RUN dotnet restore "CleanTemplate/src/CleanTemplate.Contracts/CleanTemplate.Contracts.csproj"
RUN dotnet restore "CleanTemplate/src/CleanTemplate.Application/CleanTemplate.Application.csproj"
RUN dotnet restore "CleanTemplate/src/CleanTemplate.Infrastructure/CleanTemplate.Infrastructure.csproj"
RUN dotnet restore "CleanTemplate/src/CleanTemplate.Api/CleanTemplate.Api.csproj"

WORKDIR /CleanTemplate/src
COPY ["./CleanTemplate/src/CleanTemplate.Api/.", "CleanTemplate.Api/"]
COPY ["./CleanTemplate/src/CleanTemplate.Application/.", "CleanTemplate.Application/"]
COPY ["./CleanTemplate/src/CleanTemplate.Contracts/.", "CleanTemplate.Contracts/"]
COPY ["./CleanTemplate/src/CleanTemplate.Infrastructure/.", "CleanTemplate.Infrastructure/"]
COPY ["./CleanTemplate/src/CleanTemplate.Domain/.", "CleanTemplate.Domain/"]
WORKDIR /
COPY ["./SharedKernel/.", "SharedKernel/"]
COPY ["./SharedKernel.Infrastructure/.", "SharedKernel.Infrastructure/"]

WORKDIR "/CleanTemplate/src/CleanTemplate.Api"
RUN dotnet build "CleanTemplate.Api.csproj" -c Release -o /app/build
RUN dotnet publish "CleanTemplate.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM --platform=linux/amd64/v8 mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanTemplate.Api.dll"]
