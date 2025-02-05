describe('Login', () => {
    beforeEach(() => {
        cy.intercept('POST', '/api/login-internal', {
            statusCode: 200,
            body: { token: 'mocked_token' }, // Mocked token response
        }).as('login');

        cy.visit('/login');
    });

    it('has the correct title', () => {
        cy.get('h1.main-title').should('contain', 'Login User');
    });

    it('the email and password boxes are empty', () => {
        cy.get('[name="email"]').should('be.empty');
        cy.get('[name="password"]').should('be.empty');
    });
    it('should load the submit button and be disabled when form is invalid', () => {
        cy.get('button.add-button-submit')
            .should('be.visible')
            .and('be.disabled');
    });

    it('should load the clear button', () => {
        cy.get('button.add-button-clear').should('be.visible').and('have.text', 'Clear Input');
    });
    it('should load the Google login button', () => {
        cy.get('button.google-login-button')
            .should('be.visible')
            .and('have.text', 'Login with Google');
    });

    it('should load the reset password button', () => {
        cy.get('button[routerLink="/reset-password"]')
            .should('be.visible')
            .and('have.text', 'Reset Password');
    });

    it('should load the app-message component', () => {
        cy.get('app-message').should('exist');
    });

    it('should successfully login with valid credentials', () => {
        cy.visit('/login');
        cy.get('[name="email"]').type('test@user.app');
        cy.get('[name="password"]').type('Abcde12345!');
        cy.get('button.add-button-submit').click();
        cy.wait('@login');
    });

    it('should show error messages on invalid login', () => {
        cy.intercept('POST', '/api/login-internal', {
            statusCode: 400,
            body: {
                message: 'Invalid password.'
            }
        }).as('loginInvalid');

        // Fill in the form with invalid credentials
        cy.get('[name="email"]').type('test@user.app');
        cy.get('[name="password"]').type('WrongPassword!');
        cy.get('button.add-button-submit').click();

        // Wait for the intercepted login request
        cy.wait('@loginInvalid');

        // Check that the error message appears in the app-message element
        cy.get('app-message')  // Adjust the selector to match your app's error message container
            .should('be.visible')  // Make sure the message is visible
            .and('contain', 'Bad request: Invalid password.')
            .and('contain', 'Login failed.');
    });


    

    it('should redirect to admin page when user has admin role', () => {
        cy.intercept('GET', '/api/decode-token*', (req) => {
            const token = req.query["token"];  // Extract token from the query parameters
            req.reply({
                statusCode: 200,
                body: {
                    email: 'test@patient.app',
                    token: token,
                    roles: ['Admin'], // Mocked Patient role
                },
            });
        }).as('decodeToken');

        cy.visit('/login');
        cy.get('[name="email"]').type('test@admin.app');
        cy.get('[name="password"]').type('Abcde12345!');
        cy.get('button.add-button-submit').click();

        cy.wait('@login');
        cy.wait('@decodeToken');

        cy.url().should('include', '/admin');
    });

    it('should redirect to doctor page when user has doctor role', () => {
        cy.intercept('GET', '/api/decode-token*', (req) => {
            const token = req.query["token"]; 
            req.reply({
                statusCode: 200,
                body: {
                    email: 'test@patient.app',
                    token: token,
                    roles: ['Doctor'], 
                },
            });
        }).as('decodeToken');

        cy.visit('/login');
        cy.get('[name="email"]').type('test@doctor.app');
        cy.get('[name="password"]').type('Abcde12345!');
        cy.get('button.add-button-submit').click();

        cy.wait('@login');
        cy.wait('@decodeToken');

        cy.url().should('include', '/doctor');
    });

    it('should redirect to staff page when user has staff role', () => {
        cy.intercept('GET', '/api/decode-token*', (req) => {
            const token = req.query["token"]; 
            req.reply({
                statusCode: 200,
                body: {
                    email: 'test@patient.app',
                    token: token,
                    roles: ['Nurse'],
                },
            });
        }).as('decodeToken');

        cy.visit('/login');
        cy.get('[name="email"]').type('test@staff.app');
        cy.get('[name="password"]').type('Abcde12345!');
        cy.get('button.add-button-submit').click();

        cy.wait('@login');
        cy.wait('@decodeToken');

        cy.url().should('include', '/staff');
    });

    it('should redirect to patient page when user has patient role', () => {
        cy.intercept('GET', '/api/decode-token*', (req) => {
            const token = req.query["token"]; 
            req.reply({
                statusCode: 200,
                body: {
                    email: 'test@patient.app',
                    token: token,
                    roles: ['Patient'],
                },
            });
        }).as('decodeToken');

        cy.visit('/login');
        cy.get('[name="email"]').type('test@patient.app');
        cy.get('[name="password"]').type('Abcde12345!');

        cy.get('button.add-button-submit').click();

        cy.wait('@login');
        cy.wait('@decodeToken');

        cy.url().should('include', '/patient');

    });
})