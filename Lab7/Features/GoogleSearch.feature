Feature: Google Search Functionality
	As a user
	I want to search on Google
	So that I can find relevant information

Background:
	Given I navigate to Google search page

Scenario: Verify Google page opens after entering URL
	When I enter the URL "https://www.google.co.in"
	Then the Google search page should be displayed

Scenario: Verify number of search results displayed on a page
	When I search for "SpecFlow"
	Then I should see search results displayed
	And I should count the number of results on the current page
	And the number of results should be "9"

Scenario: Verify search button behavior with empty input
	When I do not enter any search term
	And I click on the search button
	Then nothing should happen
	And I should remain on the Google search page

Scenario: Verify "Did you mean" suggestion appears for irrelevant search
	When I search for "infigo"
	Then I should see the "Did you mean" link displayed

