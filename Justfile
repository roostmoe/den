# set quiet := true

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
