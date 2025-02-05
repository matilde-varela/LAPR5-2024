# US 6.2.12

<!-- TOC -->
- [US 6.2.12](#us-6212)
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

**US 6.2.12:** As Admin, I want to deactivate a staff's profile, so that I can remove them from the hospitals active roster without losing their historical data.

**Acceptance Criteria:**

- **6.2.12.1. -** Admins can search for and select a staff profile to deactivate. 

- **6.2.12.2. -** Deactivating a staff profile removes them from the active roster, but their historical data (e.g., appointments) 
remains accessible. 

- **6.2.12.3. -** The system confirms deactivation and records the action for audit purposes. 

- **6.2.12.4:** The menu should be done in Angular.

**Dependencies/References:**

- **US 5.1.1. -** US 6.2.12. is dependent on this User Story as it depends on an existing account of a staff in the system.

- **US 5.1.6. -** US 6.2.12. is dependent on this User Story as it depends on an existing account of a staff in the system with the right permissions.

- **US 5.1.12. -** US 6.2.12. is dependent on this User Story as it depends on an existing profile of a staff in the system.

- **US 5.1.15. -** This User Story is dependent on US 6.2.12 as it needs it's functionality to work as intended.

**Client Clarifications:**

> None yet.

## 3. Analysis

This functionality talks about the deactivation of the profile of a staff member, and after some analysis of the documentation, acceptance criterias and client clarifications, it is our understanding that this User Story would follow this order of actions:

- The admin would go to a management page and would request to see a list of the active roster of staff and their profiles.
- After analyzing the list and deciding on the staff profile, the admin will select the profile and deactivate it.
- The system will then deactivate the profile and save that change onto the database, while also removing the staff in question from the active roster as to not cause further confusion to the Planning module.
- After having deactivated the staff account, the system will log that action for audit purposes as explained by acceptance criteria 6.2.12.3

It is important to note that for the sake of this project, **historical data** is considered to be:
**All types of appointments and/or requests made and/or taken part by the staff.**

## 4. Design

### 4.1. Realization

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.
These diagrams can be found in the [generic views diagrams compilation file](../../team-decisions/views/general-views.md).

The process view levels are here represented as they represent a process specific to each user story.

#### Process View - Level 1

![Process View Level 1](Diagrams\Views\process-view-level-1.svg)

#### Process View - Level 2

![Process View Level 2](Diagrams\Views\process-view-level-2.svg)

#### Process View - Level 3

![Process View Level 3 - Visualization](Diagrams\Views\process-view-level-3-visualization.svg)

![Process View Level 3 - MDBackoffice](Diagrams\Views\process-view-level-3.svg)

### 4.2. Domain Model Excerpt

![Domain Model Excerpt](Diagrams\Domain-Model\domain-model-excerpt.svg)

#### 4.3. Applied Patterns

> #### **Repository Pattern**
>
>* **Components:** UserRepository, StaffRepository, LogRepository
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
>* **Components:** UserService, StaffService, LogService
>
> These services function as a facade, simplifying the interaction with lower-level components such as repositories. 
> The controller communicates with these service facades, concealing the complexity from the upper layers.

