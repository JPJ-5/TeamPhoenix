

sequenceDiagram 
#Team Phoenix Data Access Library Low Level Design for Recoverable Writing

  participant User;
  participant ml as Manager;
  participant sl as Service;
  participant mdb as MariaDB SQL;
  participant ds as Data Store;

  User ->> ml: the user wants to write and save information to database
  ml ->> sl: the right service must be prompted to get into the data store
  sl ->> mdb: the service calls a method to get into MariaDB and execute command
    Note over sl,mdb: IResponse MariaDbDAO.Write(SQL command) 
  mdb ->> ds: database then executes SQL command and stores it in data store
    Note over mdb,ds: int SQL command.ExecuteNonQuery() 
    Note over mdb,ds: //this is going to loop until the Response.isValid == true
    Note over mdb,ds: //or until Response.retryCount = 5 //default is 0
  ds ->> ds: codes runs

  ds --> mdb: code runs and returns error(s)
  mdb --> mdb: database starts checking for errors and creates new Response object
    Note over mdb,mdb: Response.hasError == true
    Note over mdb,mdb: Response.canTryAgain == true //default value
    Note over mdb,mdb: Response.returnValue //the value it got from running the code
  mdb --> sl: returns response from the SQL from response object
    Note over mdb,sl: if the Response.retryCount < 5, then increment the retryCount value by 1
    Note over mdb,sl: if the Response.retryCount > 5, then set canTryAgain value to false
  sl --> ml: returns response from the SQL from response object






