Feature: Register

  @positive
  Scenario: Successful user registration from homepage
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    And the register fields should be visible
    When I enter "Test User" in the Name field
    And I enter "testuser@test.com" in the Email field
    And I enter "123456789" in the Password field
    And I enter "123456789" in the Confirm Password field
    And I click the Register Sign Up button
    Then the registration should be submitted

  @negative
  Scenario: Verify register with invalid email
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I enter "Test User" in the Name field
    And I enter "invalid-email" in the Email field
    And I enter "123456789" in the Password field
    And I enter "123456789" in the Confirm Password field
    And I click the Register Sign Up button
    Then the registration should fail with an error message

  @negative
  Scenario: Verify register with empty form
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I click the Register Sign Up button
    Then the registration should fail with an error message

  @negative
  Scenario: Verify registration fails when the confirm password does not match
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I enter "Test User" in the Name field
    And I enter "testuser@test.com" in the Email field
    And I enter "123456789" in the Password field
    And I enter "differentpassword" in the Confirm Password field
    And I click the Register Sign Up button
    Then the registration should fail with an error message

  @social
  Scenario: Verify login via Facebook
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I click on the Facebook button
    Then I should be redirected to Facebook login

  @negative
  Scenario: Verify registration with only the Name field filled
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I enter "Test User" in the Name field
    And I click the Register Sign Up button
    Then the registration should fail with an error message

  @negative
  Scenario: Verify registration with only the name and email fields filled
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I enter "Test User" in the Name field
    And I enter "testuser@test.com" in the Email field
    And I click the Register Sign Up button
    Then the registration should fail with an error message

  @negative
  Scenario: Verify registration only with email, without name
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I enter "testuser@test.com" in the Email field
    And I enter "123456789" in the Password field
    And I enter "123456789" in the Confirm Password field
    And I click the Register Sign Up button
    Then the registration should fail with an error message

  @negative
  Scenario Outline: Verify registration with invalid email formats
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I fill the registration form with the following data:
      | Name            | Email              | Password    | Confirm Password |
      | <Name>          | <Email>            | <Password>  | <ConfirmPassword>|
    And I click the Register Sign Up button
    Then the registration should fail with an error message

    Examples:
      | Name      | Email                | Password    | ConfirmPassword |
      | John Doe  | invalid-email        | 123456789   | 123456789       |
      | Jane Smith| noatsign.com         | 123456789   | 123456789       |
      | Bob Wilson| @nodomain.com        | 123456789   | 123456789       |
      | Alice Brown| missingdomain@      | 123456789   | 123456789       |
      | Tom Davis | spaces in@email.com  | 123456789   | 123456789       |

  @positive
  Scenario Outline: Verify successful registration with valid users formats
    Given I am on the homepage
    When I click on the Sign Up button
    Then the register popup should be displayed
    When I fill the registration form with the following data:
      | Name            | Email              | Password    | Confirm Password |
      | <Name>          | <Email>            | <Password>  | <ConfirmPassword>|
    And I click the Register Sign Up button
    Then the registration should be successful

    Examples:
      | Name          | Email                | Password    | ConfirmPassword |
      | Alice Johnson | alice@example.com    | SecurePass1 | SecurePass1      |
      | Bob Martinez  | bob.martinez@test.io | MyPass123   | MyPass123        |
      | Carol White   | carol.white@demo.net | Test1234!   | Test1234!        |