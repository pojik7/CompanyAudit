# Build stage
FROM microsoft/dotnet:2.1-sdk AS build

# set working directory
WORKDIR /app

# Restore
COPY src/Service/Service.csproj ./src/Service/
COPY nuget/Companies.Client.1.0.0.nupkg ./nuget/ 
COPY nuget/Companies.Interface.1.0.0.nupkg ./nuget/ 
RUN dotnet restore src/Service/Service.csproj  -s ./nuget -s https://api.nuget.org/v3/index.json
COPY src/Process/Process.csproj ./src/Process/
RUN dotnet restore src/Process/Process.csproj
COPY test/Service.Test/Service.Test.csproj ./test/Service.Test/
RUN dotnet restore test/Service.Test/Service.Test.csproj

# Copy src
COPY . .

# Test
#RUN dotnet test tests/Service.Test/Service.Test.csproj

# Publish
RUN dotnet publish src/Process/Process.csproj -o /obj

# Runtime stage
FROM microsoft/dotnet:2.1-runtime

# set working directory
WORKDIR /app

# Copy compiled binaries
COPY --from=build /obj ./bin

# Copy configuration
COPY config/*.* ./config/

ENV MONGO_SERVICE_URI ""
ENV MONGO_SERVICE_HOST mongo
ENV MONGO_SERVICE_PORT 27017
ENV MONGO_DB app

EXPOSE 8081

CMD ["dotnet", "./bin/run.dll", "-c", "./config/config.yml"]
