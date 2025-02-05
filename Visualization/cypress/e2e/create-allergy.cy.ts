describe('CreateAllergyComponent', () => {
    const goodAllergy = {
        code: 'BZ02.2',
        designation: 'Seafood Allergy',
        description: 'Green skin and mucus.'
    };

    const successMessage = 'Allergy: ' + goodAllergy.designation + ' was successfully created.'

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

        cy.intercept('POST', '/api/Allergy/create-allergy', (req) => {
            req.reply({
                statusCode: 200,
                body: {
                    code: goodAllergy.code,
                    designation: goodAllergy.designation,
                    description: goodAllergy.description
                }
            });
        }).as('createAllergy');

        cy.visit('/create-allergy');
    });

    it('should load the correct title and all fields', () => {
        cy.get('h1.main-title').should('contain', 'Create an Allergy');
        cy.get('p').should('contain', 'Insert the following information before submitting:');
        cy.get('input[name="code"]').should('exist');
        cy.get('input[name="designation"]').should('exist');
        cy.get('input[name="description"]').should('exist');
        cy.get('button.add-button-submit').should('exist');
        cy.get('button.add-button-clear').should('exist');
    });

    it('should enable the submit button when the form is valid', () => {
        cy.get('input[name="code"]').type(goodAllergy.code);
        cy.get('input[name="designation"]').type(goodAllergy.designation);
        cy.get('input[name="description"]').type(goodAllergy.description);

        cy.get('button.add-button-submit').should('not.be.disabled');
    });

    it('should submit the form with valid data', () => {
        cy.get('input[name="code"]').type(goodAllergy.code);
        cy.get('input[name="designation"]').type(goodAllergy.designation);
        cy.get('input[name="description"]').type(goodAllergy.description);
        cy.get('button.add-button-submit').should('not.be.disabled');
    
        cy.get('button.add-button-submit').click();

        cy.wait('@createAllergy').its('request.body').should('deep.equal', {
            code: goodAllergy.code,
            designation: goodAllergy.designation,
            description: goodAllergy.description
        });

        cy.get('form[name="allergyForm"]').should('not.exist');
        cy.contains(successMessage).should('be.visible');
    });

    it('should submit the form with valid data (without description)', () => {
        cy.get('input[name="code"]').type(goodAllergy.code);
        cy.get('input[name="designation"]').type(goodAllergy.designation);
        cy.get('button.add-button-submit').should('not.be.disabled');
    
        cy.get('button.add-button-submit').click();

        cy.wait('@createAllergy');

        cy.get('form[name="allergyForm"]').should('not.exist');
        cy.contains(successMessage).should('be.visible');
    });

    it('submit button should be invalid when code is missing', () => {
        cy.get('input[name="designation"]').type(goodAllergy.designation);
        cy.get('input[name="description"]').type(goodAllergy.description);
    
        cy.get('button.add-button-submit').should('be.disabled');
      });

      it('submit button should be invalid when designation is missing', () => {
        cy.get('input[name="code"]').type(goodAllergy.code);
        cy.get('input[name="description"]').type(goodAllergy.description);
    
        cy.get('button.add-button-submit').should('be.disabled');
      });

    it('should not submit the form when fields are invalid', () => {
        cy.get('button.add-button-submit').should('be.disabled');
        cy.contains(successMessage).should('not.exist');
    });

    it('should clear the form when the clear button is clicked', () => {
        cy.get('input[name="code"]').type(goodAllergy.code);
        cy.get('input[name="designation"]').type(goodAllergy.designation);
        cy.get('input[name="description"]').type(goodAllergy.description);

        cy.get('button.add-button-clear').click();

        cy.get('input[name="code"]').should('have.value', '');
        cy.get('input[name="designation"]').should('have.value', '');
        cy.get('input[name="description"]').should('have.value', '');
    });

    it('should disable form input fields after submission', () => {
        cy.get('input[name="code"]').type(goodAllergy.code);
        cy.get('input[name="designation"]').type(goodAllergy.designation);
        cy.get('input[name="description"]').type(goodAllergy.description);

        cy.get('button.add-button-submit').click();

        cy.wait('@createAllergy').its('request.body').should('deep.equal', {
            code: goodAllergy.code,
            designation: goodAllergy.designation,
            description: goodAllergy.description
        });

        cy.get('input[name="code"]').should('be.disabled');
        cy.get('input[name="designation"]').should('be.disabled');
        cy.get('input[name="description"]').should('be.disabled');
    });
});
