# US 5.1.21

<!-- TOC -->
- [US 5.1.21](#us-5121)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
    - [System Sequence Diagram](#system-sequence-diagram)
    - [Domain Model](#domain-model)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
    - [4.2. Class Diagram](#42-class-diagram)
    - [4.3. Applied Patterns](#43-applied-patterns)
    - [4.4. Tests](#44-tests)
  - [5. Implementation](#5-implementation)
  - [6. Integration/Demonstration](#6-integrationdemonstration)
  - [7. Observations](#7-observations)
<!-- TOC -->


## 1. Context

This is the first time this User Story is being worked on. 
This User Story is related to the Admin.

## 2. Requirements

**US 5.1.21:** As an Admin, I want to edit existing operation types, so that I can update or correct information about the procedure. 

**Acceptance Criteria:**

- **5.1.21.1. -** Admins can search for and select an existing operation type to edit. 

- **5.1.21.2. -** Editable fields include operation name, required staff by specialization, and estimated duration, both each phase and total. 

- **5.1.21.3. -** Changes are reflected in the system immediately for future operation requests. 

- **5.1.21.4. -** Historical data is maintained, but new operation requests will use the updated operation type information. 

**Dependencies/References:**

- **US 5.1.1. -** US 5.1.21. is dependent on this User Story as it depends on an existing account of a staff in the system.

- **US 5.1.20. -** US 5.1.21. is dependent on this User Story as it depends on an existing operation type in the system.

- **US 5.1.23. -** This User Story is dependent on US 5.1.21 as it needs it's functionality to work as intended.

**Client Clarifications:**

> None yet.

## 3. Analysis

This functionality talks about the updating or editting of an existing operation type, and after some analysis of the documentation (most common operation types), acceptance criterias and client clarifications, it is our understanding that this User Story would follow this order of actions:

- The admin would request to see the list of current available operation types.
- After choosing an operation type, it can choose to edit the operation's **editable data**.
- After having done so the system will update the operation requirements for future requests.
- Finally, the system will inform the Admin of a succeful change.

It is important to note that for the sake of this project, **editable data of the operation type is:** 
- **Operation Type's Name**
- **Required Staff and their Specialization**
- **Estimated Total Duration**
- **Estimated Phases Duration**

As explained by the acceptance criterias, **the changes only affect future requests** and all data relevant to old operations is still kept while the **new requests have updated information.**

### System Sequence Diagram

![SSD](Diagrams/SSD/system-sequence-diagram-admin.svg)

## 4. Design

### 4.1. Realization

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.
These diagrams can be found in the [generic views diagrams compilation file](../../team-decisions/views/general-views.md).

The process view levels are here represented as they represent a process specific to each user story.

#### Process View

The level 1 and 2 of this view was considered not to add more information in addition to the SSD shown above.
However level 3 is shown below.

##### Process View - Level 3

![Process View Level 3](Diagrams\Views\process-view-level-3.svg)

### 4.2. Domain Model Excerpt

![Domain Model Excerpt](Diagrams\Domain-Model\domain-model-excerpt.svg)

### 4.3. Applied Patterns

_// To do //_

### 4.4. Tests

_// To do - layout still in development //_ 

## 5. Implementation

_// To do //_

## 6. Integration/Demonstration

_// To do //_

## 7. Observations

_// To do //_