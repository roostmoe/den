set quiet := true

src := justfile_directory() / "src"
api := src / "Den.Api"
apphost := src / "Den.AppHost"
infra := src / "Den.Infrastructure"
migrations := src / "Den.MigrationService"

run *args:
  dotnet run --project {{ apphost }} {{ args }}

makemigrations name:
  dotnet ef migrations add {{ name }} \
    --project={{ infra }} \
    --output-dir={{ infra / "Migrations" }} \
    --startup-project={{ api }}

dbshell:
  docker exec \
    --interactive \
    --tty \
    --env "PGPASSWORD=$(cat ~/.microsoft/usersecrets/$(cat {{ apphost / "Den.AppHost.csproj" }} | grep Secrets | awk -F'>' '{ print $2; }' | awk -F'<' '{ print $1; }')/secrets.json | jq -r '.["Parameters:postgres-password"]')" \
    $(docker ps | grep postgres- | awk '{ print $1 }') \
    psql -U postgres postgresdb
