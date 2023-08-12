#!/usr/bin/env bash


reset_pg() {
  export PGUSER=postgres
  export PGPASSWORD=hackme
  export PGHOST=localhost
  export DBNAME=mixyboos
  export ASPNETCORE_Environment=Development
  
  dropdb -f --if-exists ${DBNAME}
  createdb ${DBNAME}
}

reset_mssql() {
  #!/usr/bin/env bash
  source $HOME/.prv/env
  
  HOST=localhost
  USER=sa
  PASSWORD=$MSSQLPASSWORD
  
  echo "Closing db connections"
  sqlcmd \
      -S $HOST \
      -U $USER \
      -P $PASSWORD \
      -d master \
      -i ./scripts/sql/drop.sql

  echo "Creating dev db"
  sqlcmd \
      -S $HOST \
      -d master \
      -U $USER \
      -P $PASSWORD \
      -i ./scripts/sql/create_db.sql
}
reset_mssql
./scripts/reset.sh
