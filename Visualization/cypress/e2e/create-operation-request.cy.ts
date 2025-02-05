describe('Create Operation Request', () => {
  const goodOperationRequest1 = {
    deadLineDate: '2024-12-31',
    priority: 'Emergency',
    dateOfRequest: '2024-11-23',
    status: 'Requested',
    staffId: 'D202400001',
    description: 'Test operation request',
    patientId: '202411000002',
    operationTypeId: 'Open ACL Reconstruction Surgery 2'
  };

  const successMessage = 'Operation Request successfully created';

  beforeEach(() => {
    cy.intercept('GET', '/api/OperationTypes', (req) => {
      req.reply({
        statusCode: 200,
        body: [
          {
            id: "type1-id",
            name: "type1",
            estimatedDuration: 1,
            status: true,
            requiredStaff: [],
            phases: []
          }
        ]
      });
    }).as('getOpTypes');

    cy.intercept('POST', '/api/OperationRequest', (req) => {
      req.reply({
        statusCode: 200,
        body: {
          id: 'test-id',
          deadLineDate: goodOperationRequest1.deadLineDate,
          priority: goodOperationRequest1.priority,
          dateOfRequest: goodOperationRequest1.dateOfRequest,
          status: goodOperationRequest1.status,
          staffId: goodOperationRequest1.staffId,
          description: goodOperationRequest1.description,
          patientId: goodOperationRequest1.patientId,
          operationTypeId: goodOperationRequest1.operationTypeId
        }
      });
    }).as('createOperationRequest');
    cy.visit('/create-operation-request');
  });

  it('should create a valid operation request', () => {
    // Fill out the form with valid data
    cy.get('input[name="deadLineDate"]').type(goodOperationRequest1.deadLineDate);
    cy.get('input[name="priority"]').type(goodOperationRequest1.priority);
    cy.get('input[name="dateOfRequest"]').type(goodOperationRequest1.dateOfRequest);
    cy.get('input[name="status"]').type(goodOperationRequest1.status);
    cy.get('input[name="staffId"]').type(goodOperationRequest1.staffId);
    cy.get('input[name="description"]').type(goodOperationRequest1.description);
    cy.get('input[name="patientId"]').type(goodOperationRequest1.patientId);

    cy.wait('@getOpTypes');
    cy.get('select[name="operationTypeId"]').select('type1');

    cy.get('button.add-button-submit').click();

    // Wait for the request and verify the response
    cy.wait('@createOperationRequest');
    // Verify success message is shown on the page
    cy.contains(successMessage).should('be.visible');
  });

  it('should show Add to Patient form after successful creation', () => {
    // Submit the form with valid data
    cy.get('input[name="deadLineDate"]').type(goodOperationRequest1.deadLineDate);
    cy.get('input[name="priority"]').type(goodOperationRequest1.priority);
    cy.get('input[name="dateOfRequest"]').type(goodOperationRequest1.dateOfRequest);
    cy.get('input[name="status"]').type(goodOperationRequest1.status);
    cy.get('input[name="staffId"]').type(goodOperationRequest1.staffId);
    cy.get('input[name="description"]').type(goodOperationRequest1.description);
    cy.get('input[name="patientId"]').type(goodOperationRequest1.patientId);
    cy.wait('@getOpTypes');
    cy.get('select[name="operationTypeId"]').select('type1');

    // Submit the form
    cy.get('button.add-button-submit').click();

    // Verify Add to Patient button is shown
    cy.contains("Add to Patient's Medical History").should('be.visible');

    cy.get('button.add-button-patient').click();

    cy.contains("Click on the button to submit to add the operation request created to the patient medical history!").should('be.visible');

    cy.get('button.add-button-add-patient').click();

    cy.contains("Operation request successfully add to patient history.").should('be.visible');
  });

  it('should return error creating an invalid operation request (OperationRequestName)', () => {

    // Fill out the form with valid data
    cy.get('input[name="deadLineDate"]').type(goodOperationRequest1.deadLineDate);
    cy.get('input[name="priority"]').type(goodOperationRequest1.priority);
    cy.get('input[name="dateOfRequest"]').type(goodOperationRequest1.dateOfRequest);
    cy.get('input[name="status"]').type(goodOperationRequest1.status);
    cy.get('input[name="staffId"]').type(goodOperationRequest1.staffId);
    cy.get('input[name="description"]').type(goodOperationRequest1.description);
    cy.get('input[name="patientId"]').type("753f307853");

    // Submit the form
    cy.get('button.add-button-submit').should('be.disabled');

    // Wait for the request and verify the response
  });

  it('should reset form after clicking clear button', () => {
    // Fill the form with some values
    cy.get('input[name="deadLineDate"]').type('2024-12-31');
    cy.get('input[name="priority"]').type('Emergency');

    // Click on the clear button
    cy.get('.add-button-clear').click();

    // Verify the form is cleared
    cy.get('input[name="deadLineDate"]').should('have.value', '');
    cy.get('input[name="priority"]').should('have.value', '');
  });

  it('should disable form fields after submission', () => {
    // Fill out and submit the form
    cy.get('input[name="deadLineDate"]').type(goodOperationRequest1.deadLineDate);
    cy.get('input[name="priority"]').type(goodOperationRequest1.priority);
    cy.get('input[name="dateOfRequest"]').type(goodOperationRequest1.dateOfRequest);
    cy.get('input[name="status"]').type(goodOperationRequest1.status);
    cy.get('input[name="staffId"]').type(goodOperationRequest1.staffId);
    cy.get('input[name="description"]').type(goodOperationRequest1.description);
    cy.get('input[name="patientId"]').type(goodOperationRequest1.patientId);
    cy.wait('@getOpTypes');
    cy.get('select[name="operationTypeId"]').select('type1');
    // Submit the form
    cy.get('button.add-button-submit').click();

    // Verify the fields are disabled after submission
    cy.get('input[name="deadLineDate"]').should('be.disabled');
    cy.get('input[name="priority"]').should('be.disabled');
    cy.get('input[name="dateOfRequest"]').should('be.disabled');
    cy.get('input[name="status"]').should('be.disabled');
    cy.get('input[name="staffId"]').should('be.disabled');
    cy.get('input[name="description"]').should('be.disabled');
    cy.get('input[name="patientId"]').should('be.disabled');
    cy.get('select[name="operationTypeId"]').should('be.disabled');
  });
});
