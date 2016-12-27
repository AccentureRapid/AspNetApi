@todo
Feature: Announcements News Ticker API for X2O

Scenario: response a correct  Announcements News Ticker API 
	Given Announcements News Ticker is ready
	When X2O System trigger correct request action
	Then Reponse a correct JSON "/json"
	
Scenario: response an incorrect  Announcements News Ticker API 
	Given Announcements News Ticker is ready
	When X2O System trigger incorrect request action
	Then Reponse an error message JSON "/.json"
	

	
	
	





