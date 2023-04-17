#!/usr/bin/env bash

export PGUSER=postgres
export PGPASSWORD=hackme
export PGHOST=localhost
export DBNAME=mixyboos
export ASPNETCORE_Environment=Development

dropdb -f --if-exists ${DBNAME}
createdb ${DBNAME}

./scripts/reset.sh
