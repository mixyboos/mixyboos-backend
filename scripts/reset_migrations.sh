#!/usr/bin/env bash
export PGPASSWORD=hackme
SQLITEDB="$HOME/dev/mixyboos/mixyboos-backend/mixyboos-api/mixyboos.sqlite"

if [ -f "$SQLITEDB" ]; then
    echo deleting $SQLITEDB
    rm "$SQLITEDB"
fi

dropdb -f --if-exists -h localhost -U postgres mixyboos
createdb -h localhost -U postgres mixyboos

rm -rf /srv/dev/mixyboos/mixyboos-backend/mixyboos-api/Migrations/*
cd /srv/dev/mixyboos/mixyboos-backend/mixyboos-api || exit

dotnet ef migrations add "Initial"
dotnet ef database update

psql -h localhost -U postgres mixyboos -c \
    "SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema IN ('auth', 'public') ORDER BY table_schema, table_name"
