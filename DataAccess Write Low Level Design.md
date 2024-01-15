

sequenceDiagram 
#Team Phoenix Data Access Library Low Level Design for Writing

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
    Note over mdb,ds: int SQL comman.ExecuteNonQuery()
  ds ->> ds: codes runs

  ds --> mdb: code has successfully run and new data was successfully written
  mdb --> mdb: "Response" object is created to know any errors or warnings that may have popped up
    Note over mdb,mdb: Response.isValid == false //default value
    Note over mdb,mdb: Response.canTryAgain == false //default value
    Note over mdb,mdb: Response.returnValue //the value it got from running the code
  mdb --> sl: returns response from the SQL from repsponse object
  sl --> ml: returns response from the SQL from response object






