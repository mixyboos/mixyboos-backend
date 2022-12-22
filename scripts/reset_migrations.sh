#!/usr/bin/env bash

SQLITEDB="$HOME/dev/mixyboos/mixyboos-backend/mixyboos-api/mixyboos.sqlite"
echo $SQLITEDB

if [ -f "$SQLITEDB" ]; then 
    echo deleting $SQLITEDB
    rm "$SQLITEDB"
fi

dropdb -f --if-exists -h localhost -U postgres mixyboos
createdb -h localhost -U postgres mixyboos

rm -rf $HOME/dev/mixyboos/mixyboos-backend/mixyboos-api/Migrations/*
cd $HOME/dev/mixyboos/mixyboos-backend/mixyboos-api
dotnet ef migrations add "Initial"
dotnet ef database update
