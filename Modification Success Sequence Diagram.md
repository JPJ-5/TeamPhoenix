sequenceDiagram 
#Team Phoenix Modification Sequence Diagram

  participant User
  participant AuthZ Layer
  participant Logging Layer
  participant Client Layer
  participant Server Layer
  participant Database Layer

  User ->> Client: Requests username or password change
  Client ->> Server: Sends request for Modification 
  Server ->> Database: Query Data for Modification
  Database -->> Server: Return Data
  Server -->> Client: Send Data for Modification
  Client ->> Server: Confirm Modification
  Server ->> Database: Apply Modification
  Database -->> Server: Modification Success
  Server -->> Client: Modification Successful
  Client -->> User: Modification Complete


