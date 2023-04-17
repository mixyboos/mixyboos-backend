#!/usr/bin/env bash

source ~/.prv/env

export PGUSER=$MIXYBOOS_DB_USER
export PGPASSWORD=$MIXYBOOS_DB_PASSWORD
export PGHOST=$MIXYBOOS_DB_HOST
export DBNAME=postgres
export ASPNETCORE_Environment=Production

export ConnectionStrings__MixyBoos="Host=${PGHOST};Database=${DBNAME};Username=${PGUSER};Password=${PGPASSWORD}"

psql -c "DROP SCHEMA oid CASCADE"
psql -c "DROP SCHEMA public CASCADE"
psql -c "DROP SCHEMA extensions CASCADE"
psql -c "DROP SCHEMA mixyboos CASCADE"

./scripts/reset.sh
