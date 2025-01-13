FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY GuestService/GuestService.csproj GuestService/
WORKDIR /src/GuestService
RUN dotnet restore

# Copy the rest of the source code
WORKDIR /src
COPY . .

# Build the application
WORKDIR /src/GuestService
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install OpenSSL for generating certificates
RUN apt-get update && apt-get install -y openssl

# Create directory for certificates
RUN mkdir -p /https

# Generate self-signed certificates
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /https/aspnetcore.key \
    -out /https/aspnetcore.crt \
    -subj "/CN=localhost" \
 && openssl pkcs12 -export -out /https/aspnetcore.pfx -inkey /https/aspnetcore.key \
    -in /https/aspnetcore.crt -password pass:YourCertificatePassword

# Copy the build output
COPY --from=build /app .

# Expose the service ports
EXPOSE 8082 
EXPOSE 8442

# Set environment variables for Kestrel to use the generated certificate
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetcore.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourCertificatePassword

# Start the application
ENTRYPOINT ["dotnet", "GuestService.dll", "migrate"]
