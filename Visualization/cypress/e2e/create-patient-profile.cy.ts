describe('Create Patient Profile', () => {
  
  beforeEach(() => {
    const mockToken = JSON.stringify({
      roles: ['Admin'],
      userInfo: {
          id: '12345',
          name: 'Admin',
      }
  });

  cy.window().then((win) => {
      win.localStorage.setItem('user', mockToken);
  });

  cy.intercept('GET', '/api/medicalCondition/get-all-medical-conditions', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          id : 'FB70.0',
          designation: 'Low back pain',
          description: 'Pain that occurs in the lower back, commonly due to strain, injury, or degenerative changes in the spine.',
          symptoms: 'Sharp or Dull Back Pain","Muscle Spasms","Stiffness","Difficulty Standing or Walking'
        }
      ]
    });
  }).as('fetchMedicalConditions');

  cy.intercept('GET', '/api/Allergy/get-all-allergies', (req) => {
    req.reply({
      statusCode: 200,
      body: [
        {
          code : 'BZ02.2',
          designation: 'Seafood Allergy',
          description: 'Green skin and mucus.'
        }
      ]
    });
  }).as('fetchAllergies');

    cy.intercept('POST', '/api/Patient/Create-PatientProfile', (req) => {
      req.reply ({
        statusCode: 200,
        body: {
          firstName : 'John',
          lastName : 'Doe',
          phone : '+351 910001234',
          email : 'john.doe@example.com',
          address : 'Portugal, 4590-445, Rua da Sardinha',
          emergencyContact : '+351 9100081234',
          gender : 'Male',
          dateBirth : '20/10/2004',
        }
      });
    }).as('createPatientProfile');


    cy.visit('/create-patient-profile');
  });

  it('should load the correct title and input fields', () => {
    cy.get('h1.main-title').should('contain', 'Create Patient Profile');
    cy.get('input[name="firstName"]').should('exist');
    cy.get('input[name="lastName"]').should('exist');
    cy.get('input[name="phone"]').should('exist');
    cy.get('input[name="emergencyContact"]').should('exist');
    cy.get('input[name="email"]').should('exist');
    cy.get('input[name="address"]').should('exist');
    cy.get('input[name="gender"]').should('exist');
    cy.get('input[name="dateBirth"]').should('exist');
    cy.get('input[name="description"]').should('exist');
    cy.get('p-dropdown[placeholder="Select Medical Condition"]').should('exist');
    cy.get('button[type="button"]').contains('Add').should('exist');
    cy.get('p-dropdown[placeholder="Select Allergies"]').should('exist');
    cy.get('button[type="button"]').contains('Add').should('exist');
    cy.get('button.add-button-submit').should('exist');
    cy.get('button.add-button-clear').should('exist');
  });


  it('should enable the submit button when the form is valid', () => {

    cy.get('input[name="firstName"]').type('John');
    cy.get('input[name="lastName"]').type('Doe');
    cy.get('input[name="phone"]').type('+351 910001234');
    cy.get('input[name="emergencyContact"]').type('+351 9100081234');
    cy.get('input[name="email"]').type('john.doe@example.com');
    cy.get('input[name="address"]').type('Portugal, 4590-445, Rua da Sardinha');
    cy.get('input[name="gender"]').type('Male');
    cy.get('input[name="dateBirth"]').type('20/10/2004');
    cy.get('input[name="description"]').type('some description');

    cy.get('p-dropdown[placeholder="Select Medical Condition"]').click().get('.p-dropdown-item').first().click();
    cy.get('button[type="button"]').contains('Add').click();

    cy.get('p-dropdown[placeholder="Select Allergies"]').click().get('.p-dropdown-item') .first().click();
    cy.get('button.add-button-allergy').click();

    cy.get('button.add-button-submit').should('not.be.disabled');
  });

  it('should add and remove medical conditions and allergies', () => {
    cy.get('p-dropdown[placeholder="Select Medical Condition"]').click();
    cy.get('.p-dropdown-item').contains('Low back pain').click(); 
    cy.get('button[type="button"]').contains('Add').click();
    
    cy.get('.blue-panel').should('contain', 'Designation: Low back pain')
      .and('contain', 'Description: Pain that occurs in the lower back, commonly due to strain, injury, or degenerative changes in the spine.')
      .and('contain', 'Symptoms: Sharp or Dull Back Pain","Muscle Spasms","Stiffness","Difficulty Standing or Walking');
    
    cy.get('.blue-panel').contains('Remove').click();
    cy.get('.blue-panel').should('not.exist');
  
    cy.get('p-dropdown[placeholder="Select Allergies"]').click();
    cy.get('.p-dropdown-item').contains('Seafood Allergy').click();
    cy.get('button.add-button-allergy').click();

    cy.get('.blue-panel')
    .should('contain', 'Designation: Seafood Allergy')
    .and('contain', 'Description: Green skin and mucus.');
  
    cy.get('.blue-panel').contains('Remove').click();
    cy.get('.blue-panel').should('not.exist');
  });
  

  it('should submit the form with valid data', () => {

    cy.get('input[name="firstName"]').type('John');
    cy.get('input[name="lastName"]').type('Doe');
    cy.get('input[name="phone"]').type('+351 910001234');
    cy.get('input[name="emergencyContact"]').type('+351 9100081234');
    cy.get('input[name="email"]').type('john.doe@example.com');
    cy.get('input[name="address"]').type('Portugal, 4590-445, Rua da Sardinha');
    cy.get('input[name="gender"]').type('Male');
    cy.get('input[name="dateBirth"]').type('20/10/2004');
    cy.get('input[name="description"]').type('some description');

    cy.get('p-dropdown[placeholder="Select Medical Condition"]').click().get('.p-dropdown-item').first().click();
    cy.get('button[type="button"]').contains('Add').click();

    cy.get('p-dropdown[placeholder="Select Allergies"]').click().get('.p-dropdown-item') .first().click();
    cy.get('button.add-button-allergy').click();

    cy.get('button.add-button-submit').should('not.be.disabled');
    
    cy.get('button.add-button-submit').click();

    cy.wait('@createPatientProfile');

    cy.get('app-message').should('contain', 'Patient profile: john.doe@example.com was successfully created.');
  });


  it('should clear the form when the "Clear Input" button is clicked', () => {
    cy.get('input[name="firstName"]').type('John');
    cy.get('input[name="lastName"]').type('Doe');
    cy.get('input[name="phone"]').type('+351 910001234');
    cy.get('input[name="emergencyContact"]').type('+351 9100081234');
    cy.get('input[name="email"]').type('john.doe@example.com');
    cy.get('input[name="address"]').type('Portugal, 4590-445, Rua da Sardinha');
    cy.get('input[name="gender"]').type('Male');
    cy.get('input[name="dateBirth"]').type('20/10/2004');
    cy.get('input[name="description"]').type('some description');

    cy.get('p-dropdown[placeholder="Select Medical Condition"]').click().get('.p-dropdown-item').first().click();
    cy.get('button[type="button"]').contains('Add').click();

    cy.get('p-dropdown[placeholder="Select Allergies"]').click().get('.p-dropdown-item') .first().click();
    cy.get('button.add-button-allergy').click();

    cy.get('button.add-button-clear').click();

    cy.get('input[name="firstName"]').should('have.value', '');
    cy.get('input[name="lastName"]').should('have.value', '');
    cy.get('input[name="phone"]').should('have.value', '');
    cy.get('input[name="emergencyContact"]').should('have.value', '');
    cy.get('input[name="email"]').should('have.value', '');
    cy.get('input[name="address"]').should('have.value', '');
    cy.get('input[name="gender"]').should('have.value', '');
    cy.get('input[name="dateBirth"]').should('have.value', '');
    cy.get('input[name="description"]').should('have.value', '');
  });

  it('should disable form input fields after submission', () => {
    cy.get('input[name="firstName"]').type('John');
    cy.get('input[name="lastName"]').type('Doe');
    cy.get('input[name="phone"]').type('+351 910051234');
    cy.get('input[name="emergencyContact"]').type('+351 9100581234');
    cy.get('input[name="email"]').type('john.doe@example.com');
    cy.get('input[name="address"]').type('Portugal, 4590-445, Rua da Sardinha');
    cy.get('input[name="gender"]').type('Male');
    cy.get('input[name="dateBirth"]').type('20/09/2004');
    cy.get('input[name="description"]').type('some description');
    cy.get('p-dropdown[placeholder="Select Medical Condition"]').click().get('.p-dropdown-item').first().click();
    cy.get('p-dropdown[placeholder="Select Allergies"]').click().get('.p-dropdown-item') .first().click();

    cy.get('button.add-button-submit').click();

    cy.wait('@createPatientProfile');

    cy.get('input[name="firstName"]').should('be.disabled');
    cy.get('input[name="lastName"]').should('be.disabled');
    cy.get('input[name="phone"]').should('be.disabled');
    cy.get('input[name="emergencyContact"]').should('be.disabled');
    cy.get('input[name="email"]').should('be.disabled');
    cy.get('input[name="address"]').should('be.disabled');
    cy.get('input[name="gender"]').should('be.disabled');
    cy.get('input[name="dateBirth"]').should('be.disabled');
    cy.get('input[name="description"]').should('be.disabled');
  });
});
