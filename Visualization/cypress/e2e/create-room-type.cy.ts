describe('Create Room Type', () => {
    const goodRoomType = {
      code: 'BLCOP-T1',
      designation: 'Bloco Operatório',
      description: 'something'
    };
  
    const successMessage = 'Room Type: Bloco Operatório was successfully created.';
  
    beforeEach(() => {
      const mockToken = JSON.stringify({
        roles: ['Admin'],
        userInfo: {
          id: '123',
          name: 'Dr. Smith',
        }
      });
    
      cy.window().then((win) => {
        win.localStorage.setItem('user', mockToken);
      });
  
      cy.intercept('POST', '/api/RoomType', (req) => {
        req.reply({
          statusCode: 200,
          body: {
            code: goodRoomType.code,
            designation: goodRoomType.designation,
            description: goodRoomType.description
          }
        });
      }).as('createRoomType');
  
      cy.visit('/create-room-type');
    });
  
    it('should create a valid room type', () => {
      cy.get('input[name="code"]').type(goodRoomType.code);
      cy.get('input[name="designation"]').type(goodRoomType.designation);
      cy.get('input[name="description"]').type(goodRoomType.description);
  
      cy.get('button.add-button-submit').click();
  
      cy.wait('@createRoomType');
      cy.contains(successMessage).should('be.visible');
    });
  
  
    it('submit button should be invalid when surgery appointment form isn\'t valid', () => {
      cy.get('input[name="code"]').type(goodRoomType.code);
      cy.get('input[name="description"]').type(goodRoomType.description);
  
      //button should be disabled
      cy.get('button.add-button-submit').should('be.disabled');
    });
  
    it('should reset form after clicking clear button', () => {
        cy.get('input[name="code"]').type(goodRoomType.code);
        cy.get('input[name="designation"]').type(goodRoomType.designation);
        cy.get('input[name="description"]').type(goodRoomType.description);
  
        cy.get('.add-button-clear').click();
  
        cy.get('input[name="code"]').should('have.value', '');
        cy.get('input[name="designation"]').should('have.value', '');
        cy.get('input[name="description"]').should('have.value', '');
    });
  
    it('should disable form fields when appointment is being created', () => {
        cy.get('input[name="code"]').type(goodRoomType.code);
        cy.get('input[name="designation"]').type(goodRoomType.designation);
        cy.get('input[name="description"]').type(goodRoomType.description);
  
        cy.get('button.add-button-submit').click();

        cy.get('input[name="code"]').should('be.disabled');
        cy.get('input[name="designation"]').should('be.disabled');
        cy.get('input[name="description"]').should('be.disabled');
    });
  
  });
  