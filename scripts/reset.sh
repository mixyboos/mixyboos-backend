#!/usr/bin/env bash

rm -rf /srv/dev/mixyboos/mixyboos-backend/mixyboos-api/Migrations/*
cd /srv/dev/mixyboos/mixyboos-backend/mixyboos-api || exit

dotnet ef migrations add "Initial"
dotnet ef database update

psql $DBNAME -c \
  "SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema IN ('auth', 'public') ORDER BY table_schema, table_name"

