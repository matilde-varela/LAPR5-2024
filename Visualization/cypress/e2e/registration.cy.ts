describe('Registration', () => {
  beforeEach(() => {
    cy.intercept('POST', '/api/create-patient', {
      statusCode: 200,
      body: { message: 'Patient created successfully' },
    }).as('createPatient');

    cy.visit('/patient/registration');
  });


  it('should have the correct title', () => {
    cy.get('h1.main-title')
      .should('contain', 'Registration Process');
  });


  it('should load the registration form with empty fields', () => {
    cy.get('[name="email"]').should('be.empty');
    cy.get('[name="password"]').should('be.empty');
    cy.get('[name="phone"]').should('be.empty');
  });


  it('should load the submit button and be disabled when form is invalid', () => {
    cy.get('button.add-button-submit')
      .should('be.visible')
      .and('be.disabled');
  });


  it('should load the clear button', () => {
    cy.get('button.add-button-clear').should('be.visible').and('have.text', 'Clear Input');
  });


  it('should load the app-message component', () => {
    cy.get('app-message').should('exist');
  });


  it('should successfully register a patient with valid details', () => {
    cy.get('[name="email"]').type('zeafonso2004@gmail.com');
    cy.get('[name="password"]').type('Abcde12345!');
    cy.get('[name="phone"]').type('+351 919919817');
    cy.get('button.add-button-submit').click();
    cy.wait('@createPatient');


    cy.contains('Patient created successfully')
      .should('be.visible');
  });


  it('should show error messages on invalid registration when email is invalid', () => {
    cy.intercept('POST', '/api/create-patient', {
      statusCode: 400,
      body: {
        message: 'Bad Request',
      }
    }).as('createPatientInvalid');


    cy.get('[name="email"]').type('invalid-email');
    cy.get('[name="password"]').type('Abcde12345!');
    cy.get('[name="phone"]').type('+351 919919817');
    cy.get('button.add-button-submit').click();

    cy.wait('@createPatientInvalid');


    cy.contains('Create user patient failed: Http failure response for https://localhost:5001/api/create-patient: 400 Bad Request')
      .should('be.visible');
  });


  it('should show error messages on invalid registration when phone is invalid', () => {
    cy.intercept('POST', '/api/create-patient', {
      statusCode: 400,
      body: {
        message: 'Bad Request',
      }
    }).as('createPatientInvalid');


    cy.get('[name="email"]').type('zeafonso2004@gmail.com');
    cy.get('[name="password"]').type('Abcde12345!');
    cy.get('[name="phone"]').type('123');
    cy.get('button.add-button-submit').click();

    cy.wait('@createPatientInvalid');


    cy.contains('Create user patient failed: Http failure response for https://localhost:5001/api/create-patient: 400 Bad Request')
      .should('be.visible');
  });

  it('should show error messages on invalid registration when password is invalid', () => {
    cy.intercept('POST', '/api/create-patient', {
      statusCode: 400,
      body: {
        message: 'Bad Request',
      }
    }).as('createPatientInvalid');


    cy.get('[name="email"]').type('zeafonso2004@gmail.com');
    cy.get('[name="password"]').type('short');
    cy.get('[name="phone"]').type('+351 919919817');
    cy.get('button.add-button-submit').click();

    cy.wait('@createPatientInvalid');


    cy.contains('Create user patient failed: Http failure response for https://localhost:5001/api/create-patient: 400 Bad Request')
      .should('be.visible');
  });


  it('should show error messages on invalid registration when is invalid', () => {
    cy.intercept('POST', '/api/create-patient', {
      statusCode: 400,
      body: {
        message: 'Bad Request',
      }
    }).as('createPatientInvalid');


    cy.get('[name="email"]').type('invalid-email');
    cy.get('[name="password"]').type('short');
    cy.get('[name="phone"]').type('123');
    cy.get('button.add-button-submit').click();

    cy.wait('@createPatientInvalid');


    cy.contains('Create user patient failed: Http failure response for https://localhost:5001/api/create-patient: 400 Bad Request')
      .should('be.visible');
  });


});
