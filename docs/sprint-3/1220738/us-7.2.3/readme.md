# US 7.2.3

<!-- TOC -->
- [US 7.2.3](#us-7.2.3)
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
It relates to the Doctor.

## 2. Requirements

**US 7.2.3:** As a Doctor, I want to search for Allergies, so that I can use it to update the Patient Medical Record.

**Acceptance Criteria:**

**US 7.2.3.1:** The functionality must have a Angular UI to comply with the specifications of the US 7.1.1.

**US 7.2.3.2:** The functionality must have an implementation on the .NET server in oder to function correctly.

**US 7.2.3.3:** The functionality must be related to the patient's medical record as it needs to eventually add the searched allergy to the referred document.

**US 7.2.3.4:** The functionality must have a way to cancel the search when the doctor changes their mind, effectively dividing this US's functionality into 2, searching and then possibly adding.

**US 7.2.3.5:** The functionality must implement a rudimentary search system to show the allergies that can be added.

**US 7.2.3.6:** The functionality must not show the already added allergies OR inform the user they already have been added.

**Dependencies/References:**

**US 7.1.#:** Is dependent on all these US as they pertain to integrating different modules of the system.

**US 7.2.1:** Is dependent on this US because it has the database that will hold all allergies that will be used to search for the desired one.

**Client Clarifications:**

>**Question:** Gostariamos de lhe perguntar se existe alguma lista de alergias que prefere que utilizemos no sistema por default,  se sim, quais? Também gostariamos de perguntar se quando diz "I want to search for Allergies, so that I can use it to update the Patient Medical Record" o que é que implica a ultima parte? Que a procura é feita para adicionar imediatamente ao perfil de paciente ou se é apenas uma procura feita para ir buscar a informação sobre uma alergia por exemplo?
>
>**Answer**: Considerem a seguinte lista exemplo de alergias:
>1. Peanut Allergy
>2. Shellfish Allergy (e.g., shrimp, lobster)
>3. Milk Allergy (Dairy products)
>4. Egg Allergy
>5. Tree Nut Allergy (e.g., almonds, walnuts)
>6. Wheat Allergy
>7. Penicillin Allergy
>8. Sulfa Drugs Allergy (e.g., sulfamethoxazole)
>9. Aspirin Allergy
>10. Local Anesthetics Allergy (e.g., Lidocaine)
>11. Pollen Allergy (e.g., grass, ragweed)
>12. Dust Mite Allergy
>13. Mold Allergy
>14. Cat Dander Allergy
>15. Dog Dander Allergy
>16. Latex Allergy
>17. Nickel Allergy (common in jewelry or metal objects)
>18. Bee Sting Allergy
>19. Fire Ant Sting Allergy
>20. Perfume Allergy (fragrance sensitivity)

>**Question:** Gostariamos de lhe perguntar se existe alguma lista de alergias que prefere que utilizemos no sistema por default,  se sim, quais? Também gostariamos de perguntar se quando diz "I want to search for Allergies, so that I can use it to update the Patient Medical Record" o que é que implica a ultima parte? Que a procura é feita para adicionar imediatamente ao perfil de paciente ou se é apenas uma procura feita para ir buscar a informação sobre uma alergia por exemplo?
>
>**Answer**: Em relação à segunda parte da pergunta, o que se pretende é que o médico possa indicar quais as alergias que o paciente tem seleccionando-as da lista de alergias existentes no sistema. notem que um paciente pode ter mais que uma alergia.

>**Question:** Qual seria o tamanho máximo de uma designação e descrição de uma alergia? 
>
>**Answer**: Designação, max 100 caracteres; descrição, máximo 2048 caracteres

## 3. Analysis

This User Story asks to implement: 

- Firstly, the back-end implementation of a request that returns a list of allergies from the new MongoDB database that is implemented by the US 7.2.1. 

- Secondly, to implement the front end part of this US, in order to comply with the US's 7.2.3.1 acceptance criteria.  

- Thirdly, to add the possibility to add new allergies and also add them to a patient's medical records.

This User Story will follow the same system as previous USs with the objective to list certain objects.

## 4. Design

### 4.1. Realization

**The allergy will be described as an object with two attributes:**

&emsp;**1.** A name, limited to a maximum of 100 characters;

&emsp;**2.** A description, also limited to a maximum of 2048 characters;

We will have a default number of allergies that will be used by the doctor to add them to a patient's medical record.

**The functionality in itself will be follow the following workflow:**

&emsp;**1.** The Doctor will be met with a list of the current existing allergies in the system when they get into the functionality. The list will show the allergy's ID and their name.

&emsp;**2.** The Doctor will have the following actions available to him:

&emsp;&emsp;**2.1.** **To add an allergy to the system.** (**US 7.2.2**)

&emsp;&emsp;**2.2.** **To search allergies in the system using their designation/name.**

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
>* **Components:** AllergyRepository
>
> The repositories handle data access and retrieval, isolating the database interaction logic from services and other
> layers. This approach abstracts the persistence logic, promoting separation of concerns.


> #### **DTO (Data Transfer Object) Pattern**
>
>* **Components:** AllergyListDTO
>
> DTOs are utilized to transfer data between layers, particularly from the controller layer to the service layer and
> vice versa. Their main purpose is to convey data in a structured and decoupled manner without revealing the internal
> representations of entities. Additionally, this pattern is not required to adhere to business rules.


> #### **Facade Pattern**
>
>* **Components:** AllergyService
>
> These services function as a facade, simplifying the interaction with lower-level components such as repositories.
> The controller communicates with these service facades, concealing the complexity from the upper layers.

## 5. Implementation

The implementation followed the design.

To fully experience the implementation, the user must first login with an admin account since it's the only role allowed to do such a request:

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.3/diagrams/implementation/image-0.png)

After doing so, the user must choose the Allergy segment of the sidebar and choose "List Allergies":

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.3/diagrams/implementation/image.png)

After that, the admin will be presented with all the allergies available, or it can also search for specific allergies using filters, like code or designation:

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.3/diagrams/implementation/image-1.png)

One filter:

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.3/diagrams/implementation/image-2.png)

Multiple filters:

![alt text](/sem5pi-2425-dg38/docs/sprint-3/1220738/us-7.2.3/diagrams/implementation/image-3.png)

## 6. Testing

//TO BE DONE