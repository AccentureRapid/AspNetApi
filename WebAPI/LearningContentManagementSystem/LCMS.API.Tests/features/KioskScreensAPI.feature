@todo
Feature: Kiosk screens API for X2O

Scenario: response a correct  Kiosk screens API 
	Given Kiosk screens is ready
	When X2O System trigger correct request action
	Then Reponse a correct JSON "/json"
	
Scenario: response an incorrect  Kiosk screens API 
	Given Kiosk screens is ready
	When X2O System trigger incorrect request action
	Then Reponse an error message JSON "/.json"
	

	
	
	





