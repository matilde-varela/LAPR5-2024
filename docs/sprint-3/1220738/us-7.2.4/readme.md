# US 7.2.4

<!-- TOC -->
- [US 7.2.4](#us-7.2.4)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
    - [3.1 System Sequence Diagram](#31-system-sequence-diagram)
    - [3.2 Domain Model](#32-domain-model)
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

This is the first time this US is being worked on.
It relates to the Admin.

## 2. Requirements

**US 7.2.4:** As an Admin, I want to add new Medical Condition, so that the Doctors can use it to update the Patient Medical Record.

**Acceptance Criteria:**

**US 7.2.4.1:** The functionality must have a Angular UI to comply with the specifications of the US 7.1.1.

**US 7.2.4.2:** The functionality must have an implementation on the .NET server in oder to function correctly.

**US 7.2.4.3:** The functionality must be related to the patient's medical record as it needs to eventually add the searched allergy to the referred document.

**US 7.2.4.4:** The functionality must add the medical condition to the patient's medical record and then inform the user of it's success/failure.

**US 7.2.4.5:** The functionality must not be allowed to add the already added medical conditions. 

**Dependencies/References:**

**US 7.1.#:** Is dependent on all these US as they pertain to integrating different modules of the system.

**US 7.2.1:** Is dependent on this US because it has the database that will hold all the medical conditions that will be used to search for the desired one.

**US 7.2.5:** Is dependent on this US because it will have the list of medical conditions that can be added. 

**Client Clarifications:**

>**Question:** The medical condition consist in what? Just a name or are there more fields?
>
>**Answer:** It consists of a code (for example following ICD (International Classification of Diseases)), a designation and a longer description as well a list of common symptoms.

>**Question:** What do you define as Medical Condition? Is it an allergy?
>
>**Answer:** A Medical Condition represents a diagnosed health issue or disease. Examples: Diabetes, Hypertension, Asthma, etc.

>**Question:** The medical condition consist in what? Just a name or are there more fields?
>
>**Answer:** It consists of a code (for example following ICD (International Classification of Diseases)), a designation and a longer description as well a list of common symptoms.

>**Question:** Earlier, you said the medical condition needed a code. Is this code automatic or is writen by the admin?
>
>**Answer:** It must conform with the classficiation system you select, for instance, SNOMED CT (Systematized Nomenclature of Medicine - Clinical Terms) or ICD-11 (International Classification of Diseases, 11th Revision).

>**Question:** Gostaria de lhe perguntar se existe alguma lista de medical conditions que prefere que utilizemos no sistema por default, se sim, quais? Também gostariamos de perguntar se quando diz "I want to search for Medical Conditions, so that I can use it to update the Patient Medical Record" o que é que implica a ultima parte? Que a procura é feita para adicionar imediatamente ao perfil de paciente ou se é apenas uma procura feita para ir buscar a informação sobre uma medical condition por exemplo?
>
>**Answer:** default medical conditions (ICD-11):
A04.0: Cholera
A08.0: Rotavirus enteritis
B20: Human Immunodeficiency Virus (HIV) disease
B50: Plasmodium falciparum malaria
2A20.0: Malignant neoplasm of lung
2F44.0: Malignant neoplasm of the breast
3A01.1: Iron deficiency anemia
4A44: Hereditary hemochromatosis
5A11: Type 1 diabetes mellitus
5B55: Obesity
6A80: Major depressive disorder
6C40: Generalized anxiety disorder
FB20.1: Osteoporosis with pathological fracture
FB81.1: Osteoarthritis of the knee
FB81.2: Osteoarthritis of the hip
FB80.1: Rheumatoid arthritis
FA24.0: Fracture of femur
FA22.0: Fracture of radius and ulna
FA21.0: Dislocation of shoulder
FB70.0: Low back pain

## 3. Analysis

This User Story asks to implement: 

- Firstly, the back-end implementation of a request that returns a list of medical conditions from the new MongoDB database that is implemented by the US 7.2.1. 

- Secondly, to implement the front end part of this US, in order to comply with the US's 7.2.4.1 acceptance criteria.  

This User Story will follow the same system as previous USs with the objective to add certain objects to the database.

## 4. Design

### 4.1. Realization

**The medical condition will be described as an object with 4 attributes:**

&emsp;**1.** An id, following ICD (**International Classification of Diseases**);

&emsp;**2.** A designation, limited to a maximum of 100 characters;

&emsp;**3.** A description, also limited to a maximum of 2048 characters;

&emsp;**4.** A list of symptoms;

We will have a default number of medical conditions that will be used by the doctor to add them to a patient's medical record.

**The functionality in itself will be follow the following workflow:**

&emsp;**1.** The Doctor will be met with a list of the current existing medical conditions in the system when they get into the functionality.

&emsp;**2.** The Doctor will have the following actions available to him:

&emsp;&emsp;**2.1.** **To add a medical condition to the system.** 

&emsp;&emsp;**2.2.** **To search medical conditions in the system using their designation/name.** (**US 7.2.5**)

#### Views

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.
These diagrams can be found in the [generic views diagrams compilation file](../../team-decisions/views/general-views.md).

The process view levels are here represented as they represent a process specific to each user story.

##### Process View

The level 1 and 2 of this view was considered not to add more information in addition to the SSD shown above.
However level 3 is shown below.

###### Process View - Level 1

![Process View Level 1](diagrams\views\process-view-level-1.svg)

###### Process View - Level 2

![Process View Level 2](diagrams\views\process-view-level-2.svg)

###### Process View - Level 3

![Process View Level 3 - Visualization](diagrams\views\process-view-level-3-visualization.svg)

![Process View Level 3 - MDBackoffice](diagrams\views\process-view-level-3-mdpatientmanagement.svg)

### 4.2. Domain Model Excerpt

![Domain Model Excerpt](diagrams\domain-model\domain-model-simplification.svg)

### 4.3. Applied Patterns

> #### **Repository Pattern**
>
>* **Components:** MedicalConditionRepository
>
> The repositories handle data access and retrieval, isolating the database interaction logic from services and other
> layers. This approach abstracts the persistence logic, promoting separation of concerns.


> #### **DTO (Data Transfer Object) Pattern**
>
>* **Components:** MedicalConditionDTO
>
> DTOs are utilized to transfer data between layers, particularly from the controller layer to the service layer and
> vice versa. Their main purpose is to convey data in a structured and decoupled manner without revealing the internal
> representations of entities. Additionally, this pattern is not required to adhere to business rules.


> #### **Facade Pattern**
>
>* **Components:** MedicalConditionService
>
> These services function as a facade, simplifying the interaction with lower-level components such as repositories.
> The controller communicates with these service facades, concealing the complexity from the upper layers.

## 5. Implementation

The implementation followed the design.

To fully experience the implementation, the user must first login with an admin account since it's the only role allowed to do such a request:

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.4/diagrams/implementation/image-1.png)

After doing so, the user must choose the Medical Condition segment of the sidebar and choose "Create Medical Condition":

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.4/diagrams/implementation/image-2.png)

After that, the admin must put the correct code, name, description and symptoms that are to be stored in the database:

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.4/diagrams/implementation/image.png)

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.4/diagrams/implementation/image-3.png)

## 6. Testing

//TO BE DONE