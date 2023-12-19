sequenceDiagram 
#Team Phoenix Modification Success Sequence Diagram

participant User #person trying to modify the account 
participant ml as Manager Layer 
participant az as AuthZ Layer 
participant lg as Logging Layer 
participant um as User Modification Layer
participant mdb as MariaDBServerDAO Layer
participant ds as Data Store

User ->> ml: request to change account information
Note over User, ml: function 'updateProfile(FirstName, LastName, Password)' is called: string

ml ->> az: Username and password are entered 
Note over ml, az: function 'isAuthZ(username, pw)' is called: bool

az ->> lg: successfully authorized
Note over az, lg: function 'loggedData()' is called to store the logs into datastore: string

lg ->> um: log recorded 
Note over lg, um: 'isLogged(log)' is called as confirmation to client that log was stored: bool

um ->> mdb: requests to change username, password, or email 
Note over um,mdb: 'requestChange(username| pw | email)' is called: string

mdb ->> ds: writes to SQL after username, password, or email is changed
Note over mdb, ds: 'writeToDB(username, pw, email)' is called: string

ds --> ds: user has successfully modified and updated data 
Note over ds,ds: 'isModified()' is called to let the user know that their account has been changed and updated: bool

ds --> mdb: datastore tells which rows have been updated 
Note over ds,mdb: 'rowsChanged(username | pw | email)' is called to let the datastore know which of the rows have been updated: string

mdb --> User: datastore displays new username, password, or email
Note over mdb,User: 'displayChanges()' is called to show the user the changes they made to their account: string