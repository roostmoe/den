set quiet := true

apphost := justfile_directory() / "src" / "Den.AppHost"

run *args:
  dotnet run --project {{ apphost }} {{ args }}
