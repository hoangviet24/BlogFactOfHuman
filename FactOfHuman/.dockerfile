# Sử dụng hình ảnh .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy toàn bộ mã nguồn vào container
COPY . .

# Restore các dependency và build ứng dụng
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Sử dụng hình ảnh .NET runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose cổng mà ứng dụng sẽ chạy (ví dụ: 5000)
EXPOSE 5000

# Lệnh để chạy ứng dụng
ENTRYPOINT ["dotnet", "FactOfHuman.dll"]
