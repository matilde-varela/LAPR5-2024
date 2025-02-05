# US 6.2.8

<!-- TOC -->
- [US 6.2.8](#us-628)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
      - [Logical View](#logical-view)
      - [Process View](#process-view)
        - [Level 1](#level-1)
        - [Level 2](#level-2)
        - [Level 3](#level-3)
      - [Development View](#development-view)
      - [Physical View](#physical-view)
<!-- TOC -->

## 1. Context

This is the first time this US is being worked on.
This US pertains to the Admin.

## 2. Requirements

**US 6.2.8:** As an Admin, I want to delete a patient profile, so that I can remove patients who are no longer under care.

**Acceptance Criteria:**

- **6.2.8.1. -** Admins can search for a patient profile and mark it for deletion. 

- **6.2.8.2. -** Before deletion, the system prompts the admin to confirm the action. 

- **6.2.8.3. -** Once deleted, all patient data is permanently removed from the system within a predefined time frame. 

- **6.2.8.4. -** The system logs the deletion for audit and GDPR compliance purposes. 

- **6.2.8.5:** The menu should be done in Angular.

**Dependencies/References:**

- **US 5.1.3. -** US 6.2.8. is dependent on this User Story as it depends on an existing profile of a patient in the 
system.

- **US 5.1.6. -** US 6.2.8. is dependent on this User Story as it depends on an existing account of a staff in the system with the right permissions.

- **US 5.1.11. -** This User Story is dependent on US 6.2.8 as it needs it's functionality to work as intended.

**Client Clarifications:**

> None yet.

## 3. Analysis

After reviewing the documentation and carefully analyzing the acceptance criteria, we agreed upon an order of events of this User Story as follows:

- The admin will request to delete a patient's profile.
- The admin will see a list of all patients and will be able to choose one and select to delete said profile.
- They will be given the option to confirm or cancel said action. 
- If the request goes forward then the patient's profile will be tagged for deletion and after a predetermined amount of time, it will be removed permenantely from the system.
- When the data is removed the system will log the deletion and add it to the database as said in acceptance criteria 5.1.10.4.

The predefined amount of time for the data to be deleted permanently and the extent of the data that will be deleted is a subject that will further explored in the Design section as it also pertains to GDPR regulation.

## 4. Design

### 4.1. Realization

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.
These diagrams can be found in the [generic views diagrams compilation file](../../team-decisions/views/general-views.md).

The process view levels are here represented as they represent a process specific to each user story.

#### Process View

##### Process View - Level 1

![Process View Level 1](Diagrams\Views\process-view-level-1.svg)

##### Process View - Level 2

![Process View Level 2](Diagrams\Views\process-view-level-2.svg)

##### Process View - Level 3

![Process View Level 3 - Visualization](Diagrams\Views\process-view-level-3-visualization.svg)

![Process View Level 3 - MDBackoffice](Diagrams\Views\process-view-level-3.svg)

### 4.2. Domain Model Excerpt

![Domain Model Excerpt](Diagrams\Domain-Model\domain-model-excerpt.svg)

### 4.3. Applied Patterns

> #### **Repository Pattern**
>
>* **Components:** UserRepository, PatientRepository, LogRepository
>
> The repositories handle data access and retrieval, isolating the database interaction logic from services and other
> layers. This approach abstracts the persistence logic, promoting separation of concerns.


> #### **DTO (Data Transfer Object) Pattern**
>
>* **Components:** IdPassDto
>
> DTOs are utilized to transfer data between layers, particularly from the controller layer to the service layer and
> vice versa. Their main purpose is to convey data in a structured and decoupled manner without revealing the internal
> representations of entities. Additionally, this pattern is not required to adhere to business rules.


> #### **Facade Pattern**
>
>* **Components:** UserService, PatientService, LogService
>
> These services function as a facade, simplifying the interaction with lower-level components such as repositories.
> The controller communicates with these service facades, concealing the complexity from the upper layers.



