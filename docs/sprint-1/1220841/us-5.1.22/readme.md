# US 5.1.22

<!-- TOC -->
- [US 5.1.22](#us-5122)
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
    - [4.3. Applied Patterns](#43-applied-patterns)
    - [4.4. Tests](#44-tests)
  - [5. Implementation](#5-implementation)
  - [6. Integration/Demonstration](#6-integrationdemonstration)
  - [7. Observations](#7-observations)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 5.1.22:** As an Admin, I want to remove obsolete or no longer performed operation types, so that the system stays current with hospital practices.

**Acceptance Criteria:**

- 5.1.22.1: Admins can search for and mark operation types as inactive (rather than deleting them) to preserve historical records.

- 5.1.22.2: Inactive operation types are no longer available for future scheduling but remain in historical data.

- 5.1.22.3: A confirmation prompt is shown before deactivating an operation type.

**Dependencies/References:**

This user story is related to US-5.1.23, as it involves selecting an operation type for removal.

**Client Clarifications:**

> **Question:** The acceptance criteria refer to deactivating the operation type. In this context, is "removing" being used to describe the deactivation of a specific operation type, or is there a mix-up?
>
> **Answer:** Yes, deactivating makes the operation type unavailable for future use but retains historical data.

## 3. Analysis

This feature primarily involves deactivating a specific operation type, preventing it from being selected when creating future operation requests. However, if there are existing operations of that type, no changes will be made, thereby preserving the historical data.

Keeping this in mind, all active operation types will be displayed for the admin to select from. Additionally, before proceeding with the deactivation, a confirmation prompt will appear, showing the selected operation type to verify whether the admin truly wishes to deactivate it. Upon confirmation, the deactivation process will proceed.

## 4. Design

### 4.1. Realization

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.

#### Logical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#1-logical-view).

#### Process View

##### Level 1

![Process View - Level 1](diagrams/level-1-process-view.svg)

##### Level 2

_[This diagram is not relevant.]_

##### Level 3

![Process View - Level 3](diagrams/level-3-process-view.svg)


#### Development View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#3-development-view).

#### Physical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#4-physical-view).
