

sequenceDiagram 
#Team Phoenix Data Access Library Low Level Design for Recoverable Reading

  participant User;
  participant ml as Manager;
  participant sl as Service;
  participant mdb as MariaDB SQL;
  participant ds as Data Store;

  User ->> ml: the user wants to read and save information to database
  ml ->> sl: the right service must be prompted to get into the data store
  sl ->> mdb: the service calls a method to get into MariaDB and execute command
    Note over sl,mdb: ISqlDAO SqlDAO.Read(SQL command) 
  mdb ->> ds: database then executes SQL command and stores it in data store
    Note over mdb,ds: int SQL command.ExecuteNonQuery() 
    Note over mdb,ds: //this is going to loop until the result.Success == true
    Note over mdb,ds: //or until result.retryCount = 5 
  ds ->> ds: codes runs

  ds --> mdb: reading code returns error(s)
  mdb --> mdb: database acknowledges errors then creates new Result object
    Note over mdb,mdb: result.Success == true
    Note over mdb,mdb: result.canTryAgain == true
  mdb --> sl: MariaDB returns result from the SQL from result object
    Note over mdb,sl: if the result.retryCount < 5, then increment the retryCount value by 1
    Note over mdb,sl: if the result.retryCount >= 5, then set canTryAgain value to false
  sl --> ml: Service layer returns result object






