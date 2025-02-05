# US 7.1.4

<!-- TOC -->
* [US 7.1.4](#us-714)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
      * [Logical View](#logical-view)
      * [Development View](#development-view)
      * [Physical View](#physical-view)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 7.1.4:** As Admin I want the information about healthcare staff, operation types, and operation requests used in 
the planning module is in sync with the information entered in the backoffice module.

**Acceptance Criteria:**

- **7.1.4.1:** Any change to healthcare staff, operation requests or operation types in the backoffice module must be 
synchronized with the planning module, to unsure it's used the latest information.

- **7.1.4.2:** In case of failure, a notification error should be presented.

**Dependencies/References:**

**US 6.3.1 & 6.3.2 & 6.3.3:** The planning module must be working so the information can be in sync with the information
from the backoffice.

**US 6.2.10:** There needs to exist information about healthcare staff.

**US 6.2.14:** There needs to exist information about operation requests.

**US 6.2.18:** There needs to exist information about operation type.


## 3. Analysis

To achieve the objective of this user story, it is needed to integrate the planning module with the backoffice module.
Having this in mind, it's necessary to exist the followed information in the backoffice module:

  * healthcare staff
  * operation types
  * operation requests


## 4. Design

### 4.1. Realization

To ensure integration between the information about healthcare staff, operation types and operation requests from the
backoffice module and the planning module, it is necessary to verify if it was created new instances, updated or deleted
instances.

In order to have all the information coherent between the modules, any changes made in user stories from the backoffice,
that imply changes on the Database need to be fetched by the planning module and the data must remain up to date in real
time, with changes from the backoffice module being reflected in the planning module without the need to refresh the page.

If an error occurs during the synchronization process, such as a failure in API communication, the system will present a
notification in the planning module to alert the Admin. This notification will describe the issue.

This approach will be more effectively demonstrated through the process view diagrams in the user stories: 
[us-6.3.1](../../us-6.3.1/readme.md) and [us-6.3.3](../../us-6.3.3/readme.md)

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.

#### Logical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#1-logical-view).


#### Development View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#3-development-view).

#### Physical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#4-physical-view).
