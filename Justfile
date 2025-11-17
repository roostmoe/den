set quiet := true

apphost := justfile_directory() / "src" / "Den.AppHost"
infra := justfile_directory() / "src" / "Den.Infrastructure"
migrations := justfile_directory() / "src" / "Den.MigrationService"

run *args:
  dotnet run --project {{ apphost }} {{ args }}

makemigrations name:
  dotnet ef migrations add {{ name }} \
    --project={{ infra }} \
    --output-dir={{ infra / "Migrations" }} \
    --startup-project={{ migrations }}

dbshell:
  docker exec \
    --interactive \
    --tty \
    --env "PGPASSWORD=$(cat ~/.microsoft/usersecrets/$(cat {{ apphost / "Den.AppHost.csproj" }} | grep Secrets | awk -F'>' '{ print $2; }' | awk -F'<' '{ print $1; }')/secrets.json | jq -r '.["Parameters:postgres-password"]')" \
    $(docker ps | grep postgres- | awk '{ print $1 }') \
    psql -U postgres postgresdb
