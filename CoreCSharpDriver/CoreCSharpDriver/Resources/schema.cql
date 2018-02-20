CREATE KEYSPACE driver_test 
  WITH REPLICATION = { 
   'class' : 'NetworkTopologyStrategy', 
   'DC1' : 1 
  } ;


CREATE TABLE driver_test.user (
	id uuid, 
	first_name text, 
	last_name text, 
	PRIMARY KEY (id)
);