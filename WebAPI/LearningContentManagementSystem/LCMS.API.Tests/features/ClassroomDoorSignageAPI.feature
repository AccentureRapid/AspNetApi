@todo
Feature: Classroom Door Signage API for X2O

Scenario: response a correct  Classroom Door Signage API 
	Given Classroom Door Signage is ready
	When X2O System trigger correct request action
	Then Reponse a correct JSON "/json"
	
Scenario: response an incorrect  Classroom Door Signage API 
	Given Classroom Door Signage is ready
	When X2O System trigger incorrect request action
	Then Reponse an error message JSON "/.json"
	

	
	
	





