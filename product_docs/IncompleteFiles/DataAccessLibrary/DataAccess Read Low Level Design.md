

sequenceDiagram 
#Team Phoenix Data Access Library Low Level Design for Successful Reading

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
  ds ->> ds: codes runs

  ds --> mdb: code has successfully run with no interruptions
  mdb --> mdb: Result object is created to know any errors or warnings that may have popped up
    Note over mdb,mdb: result.isValid == false //default value
    Note over mdb,mdb: result.canTryAgain == false //default value
    Note over mdb,mdb: result.returnValue //the value it got from running the code
  mdb --> sl: MariaDB returns result object
  sl --> ml: Service Layer returns result object






