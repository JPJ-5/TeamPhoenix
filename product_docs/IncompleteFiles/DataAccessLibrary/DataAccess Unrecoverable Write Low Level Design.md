

sequenceDiagram 
#Team Phoenix Data Access Library Low Level Design for Unrecoverable Writing

  participant User;
  participant ml as Manager;
  participant sl as Service;
  participant mdb as MariaDB SQL;
  participant ds as Data Store;

  User ->> ml: the user wants to write and save information to database
  ml ->> sl: the right service must be prompted to get into the data store
  sl ->> mdb: the service calls a method to get into MariaDB and execute command
    Note over sl,mdb: ISqlDAO SqlDAO.Write(SQL command) 
  mdb ->> ds: database then executes SQL command and stores it in data store
    Note over mdb,ds: int SQL command.ExecuteNonQuery() 
  ds ->> ds: codes runs

  ds --> mdb: code runs and returns error(s)
  mdb --> mdb: database acknowledges errors then creates Result object
    Note over mdb,mdb: result.hasError == true
    Note over mdb,mdb: result.canTryAgain == false
  mdb --> sl: MariaDB returns result object
  sl --> ml: Service layer returns result object






