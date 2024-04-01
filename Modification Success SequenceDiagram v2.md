sequenceDiagram 
#Team Phoenix Modification Success Sequence Diagram

  participant User #person trying to modify the account
  participant ml as Manager Layer
  participant az as AuthZ Layer
  participant lg as Logging Layer
  participant cl as Client
  participant mdb as MariaDBServerDAO Layer
  participant db as Database

  
  ml ->> az: Username and password are entered
  az ->> lg: successfully authenticated
  lg ->> cl: log recorded
  cl ->> mdb: requests to change username, password, or email
  mdb ->> db: writes to SQL by executing SQL command
  db --> db: user has successfully modified data
  db --> mdb: database tells which rows have been updated
  mdb --> cl: database displays new username, password, or email