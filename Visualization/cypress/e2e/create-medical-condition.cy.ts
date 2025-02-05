describe('Create Medical Condition', () => {

    const mockMedicalCondition = {
        id: '12345',
        designation: 'Test Condition',
        description: 'This is a test description',
        symptoms: ['fever', 'headache']
    };

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
        // Intercept the API call and mock the response
        cy.intercept('POST', '/api/medicalCondition/create', (req) => {
            req.reply({
                statusCode: 200,
                body: {
                    id: '12345',
                    designation: 'Test Condition',
                    description: 'This is a test description',
                    symptoms: ['fever', 'headache']
                }
            });
        }).as('createMedicalCondition');

        cy.visit('/create-medical-condition'); // Visit the component page
    });

    it('has the correct title', () => {
        cy.get('h1.main-title').should('contain', 'Create a Medical Condition');
    });

    it('the fields should be empty', () => {
        cy.get('input[name="code"]').should('be.empty');
        cy.get('input[name="designation"]').should('be.empty');
        cy.get('input[name="description"]').should('be.empty');
        cy.get('input[name="symptoms"]').should('be.empty');
    });
    it('should load the clear button', () => {
        cy.get('button.add-button-clear')
        .should('be.visible')
        .invoke('text')
        .should('contain', 'Clear Input');
    });
    it('should load the submit button', () => {
        cy.get('button.add-button-submit')
        .should('be.visible')
        .invoke('text')
        .should('contain', 'Submit');
    });
    it('should display the form fields correctly', () => {
        cy.get('input[name="code"]').should('be.visible');
        cy.get('input[name="designation"]').should('be.visible');
        cy.get('input[name="description"]').should('be.visible');
        cy.get('input[name="symptoms"]').should('be.visible');
    });
    it('should load the app-message component', () => {
        cy.get('app-message').should('exist');
    });
    it('should submit the form with valid data', () => {
        cy.get('input[name="code"]').type(mockMedicalCondition.id);
        cy.get('input[name="designation"]').type(mockMedicalCondition.designation);
        cy.get('input[name="description"]').type(mockMedicalCondition.description);
        cy.get('input[name="symptoms"]').type(mockMedicalCondition.symptoms.join(','));

        cy.get('button[type="submit"]').click();

        cy.wait('@createMedicalCondition');

        cy.get('app-message').should('contain', 'Medical Condition was successfully created.');
    });
    it('should show validation errors for missing fields', () => {
        // Ensure the form is initially invalid (empty form fields)
        cy.get('input[name="code"]').clear();
        cy.get('input[name="designation"]').clear();
        cy.get('input[name="description"]').clear();
        cy.get('input[name="symptoms"]').clear();
    
        // Check if the invalid fields have the 'invalid-placeholder' class
        cy.get('input[name="code"]').should('have.class', 'invalid-placeholder');
        cy.get('input[name="designation"]').should('have.class', 'invalid-placeholder');
    });
    

    it('should clear the form when the Clear Input button is clicked', () => {
        cy.get('input[name="code"]').type(mockMedicalCondition.id);
        cy.get('input[name="designation"]').type(mockMedicalCondition.designation);
        cy.get('input[name="description"]').type(mockMedicalCondition.description);
        cy.get('input[name="symptoms"]').type(mockMedicalCondition.symptoms.join(','));

        cy.get('button[type="button"]').click(); // Click the "Clear Input" button

        // Assert that the form fields are reset
        cy.get('input[name="code"]').should('have.value', '');
        cy.get('input[name="designation"]').should('have.value', '');
        cy.get('input[name="description"]').should('have.value', '');
        cy.get('input[name="symptoms"]').should('have.value', '');
    });

    it('should disable form fields when the form is submitted', () => {
        cy.get('input[name="code"]').type(mockMedicalCondition.id);
        cy.get('input[name="designation"]').type(mockMedicalCondition.designation);
        cy.get('input[name="description"]').type(mockMedicalCondition.description);
        cy.get('input[name="symptoms"]').type(mockMedicalCondition.symptoms.join(','));

        cy.get('button[type="submit"]').click();

        // Check if the inputs are disabled
        cy.get('input[name="code"]').should('be.disabled');
        cy.get('input[name="designation"]').should('be.disabled');
        cy.get('input[name="description"]').should('be.disabled');
        cy.get('input[name="symptoms"]').should('be.disabled');
    });

});

