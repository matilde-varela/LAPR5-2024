describe('Create Surgery Appointment', () => {
  const goodSurgeryAppointment = {
    operationRequestId: '2024-12-31',
    roomNumber: 'Emergency',
    startTime: '2024-11-23',
    endTime: 'Requested',
    startDate: 'D202400001',
    endDate: 'Test operation request',
    staffList: ['202411000002'],
  };

  const successMessage = 'Surgery Appointment was successfully created.';

  beforeEach(() => {
    const mockToken = JSON.stringify({
      roles: ['Doctor'],
      userInfo: {
        id: '123',
        name: 'Dr. Smith',
      }
    });
  
    cy.window().then((win) => {
      win.localStorage.setItem('user', mockToken);
    });

    cy.intercept('GET', '/api/Room/Get-All', (req) => {
      req.reply({
        statusCode: 200,
        body: [
          {
            roomNumber: '101',
            type: 'Surgical',
            capacity: 2,
            availableEquipment: ['Scalpel', 'Monitor'],
            currentStatus: 'Available',
            maintenanceSlots: []
          }
        ]
      });
    }).as('fetchRooms');

    cy.intercept('GET', '/api/OperationRequest/Get-AllOpRequests', (req) => {
      req.reply({
        statusCode: 200,
        body: [
          {
            id: '1',
            deadLineDate: '2023-12-01',
            priority: 'High',
            dateOfRequest: '2023-11-01',
            status: 'Pending',
            staffId: '123',
            description: 'Appendectomy',
            patientId: '456',
            operationTypeId: '789'
          }
        ]
      });
    }).as('fetchOperationRequests');

    cy.intercept('GET', '/api/Staff/Get-ActiveStaffProfiles', (req) => {
      req.reply({
        statusCode: 200,
        body: [
          {
            id: '1',
            name: 'Dr. Smith',
            phone: '123456789',
            email: 'dr.smith@example.com',
            function: 'Surgeon',
            address: '123 Main St',
            specializationId: '456',
            slots: [],
            status: 'Active'
          }
        ]
      });
    }).as('fetchStaff');

    cy.intercept('POST', '/api/Appointment', (req) => {
      req.reply({
        statusCode: 200,
        body: {
          id: 'test-id',
          deadLineDate: goodSurgeryAppointment.operationRequestId,
          priority: goodSurgeryAppointment.roomNumber,
          dateOfRequest: goodSurgeryAppointment.startTime,
          status: goodSurgeryAppointment.endTime,
          staffId: goodSurgeryAppointment.startDate,
          description: goodSurgeryAppointment.endDate,
          patientId: goodSurgeryAppointment.staffList.join(', ')
        }
      });
    }).as('createSurgeryAppointment');

    cy.visit('/create-surgery-appointment');
  });

  it('should create a valid surgery appointment', () => {
    cy.get('p-tableRadioButton[name="operationRadio"]').first().click();
    cy.get('p-tableRadioButton[name="roomRadio"]').first().click();
    cy.get('p-tableCheckbox[name="staffCheck"]').first().click();

    cy.get('input[name="startTime"]').type(goodSurgeryAppointment.startTime);
    cy.get('input[name="endTime"]').type(goodSurgeryAppointment.endTime);
    cy.get('input[name="startDate"]').type(goodSurgeryAppointment.startDate);
    cy.get('input[name="endDate"]').type(goodSurgeryAppointment.endDate);

    cy.get('button.add-button-submit').click();

    cy.wait('@createSurgeryAppointment');
    cy.contains(successMessage).should('be.visible');
  });


  it('submit button should be invalid when surgery appointment form isn\'t valid', () => {

    cy.get('p-tableRadioButton[name="operationRadio"]').first().click();
    //missing room selection on purpose
    cy.get('p-tableCheckbox[name="staffCheck"]').first().click();

    cy.get('input[name="startTime"]').type(goodSurgeryAppointment.startTime);
    cy.get('input[name="endTime"]').type(goodSurgeryAppointment.endTime);
    cy.get('input[name="startDate"]').type(goodSurgeryAppointment.startDate);
    cy.get('input[name="endDate"]').type(goodSurgeryAppointment.endDate);

    //button should be disabled
    cy.get('button.add-button-submit').should('be.disabled');
  });

  it('should reset form after clicking clear button', () => {
    cy.get('input[name="startTime"]').type(goodSurgeryAppointment.startTime);
    cy.get('input[name="endTime"]').type(goodSurgeryAppointment.endTime);
    cy.get('input[name="startDate"]').type(goodSurgeryAppointment.startDate);
    cy.get('input[name="endDate"]').type(goodSurgeryAppointment.endDate);

    cy.get('.add-button-clear').click();

    cy.get('input[name="startTime"]').should('have.value', '');
    cy.get('input[name="endTime"]').should('have.value', '');
  });

  it('should disable form fields when appointment is being created', () => {
    cy.get('p-tableRadioButton[name="operationRadio"]').first().click();
    cy.get('p-tableRadioButton[name="roomRadio"]').first().click();
    cy.get('p-tableCheckbox[name="staffCheck"]').first().click();

    cy.get('input[name="startTime"]').type(goodSurgeryAppointment.startTime);
    cy.get('input[name="endTime"]').type(goodSurgeryAppointment.endTime);
    cy.get('input[name="startDate"]').type(goodSurgeryAppointment.startDate);
    cy.get('input[name="endDate"]').type(goodSurgeryAppointment.endDate);

    cy.get('button.add-button-submit').click();
    
    cy.get('input[name="startTime"]').should('be.disabled');
    cy.get('input[name="endTime"]').should('be.disabled');
    cy.get('input[name="startDate"]').should('be.disabled');
    cy.get('input[name="endDate"]').should('be.disabled');
  });

});
