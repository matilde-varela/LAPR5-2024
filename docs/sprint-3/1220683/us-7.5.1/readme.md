# US 7.5.1

<!-- TOC -->
* [US 7.5.1](#us-751)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
    * [Process View](#process-view)
      * [Level 1](#level-1)
      * [Level 2](#level-2)
      * [Level 3](#level-3)
      * [4.1.3 Development View](#413-development-view)
      * [4.1.4 Physical View](#414-physical-view)
  * [5. Implementation](#5-implementation)
  * [6. Integration/Demonstration](#6-integrationdemonstration)
  * [7. Observations](#7-observations)
<!-- TOC -->


## 1. Context

* This is the first time this user story is being requested.

## 2. Requirements

**US 7.5.1:** As a healthcare staff member, I want to select a room by left-clicking on the corresponding surgical table (object picking).
The camera should instantly move horizontally so that it targets the room center point.

**Acceptance Criteria:**

- **7.5.1.1.** | This component can be implemented in WebGL, or in a higher-level API such as three.js or Babylon.js.
- **7.5.1.2.** | The graphical user interface and animations may rely on dedicated APIs such as lil-gui and tween.js, respectively.

**Dependencies/References:**

This feature is related to the existence of a 3D simulation with models (US 6.5.1) with camara (US 6.5.4).

**Client Clarifications:**

> **Question:** 
>
> **Answer:** 


## 3. Analysis

The selection of the room and the response from the simulation is done through Three.js's raycaster (object picking),
allowing for the intersection of objects. The mouse position is obtained through an event listener, and the camara animation 
is done using tween.js.


## 4. Design

### 4.1. Realization

The logical, physical, development and scenario views diagrams are generic for all the use cases of the backoffice component.
These diagrams can be found in the [generic views diagrams compilation file](../../team-decisions/views/general-views.md).

The process view levels are here presented as they represent a process specific to each user story.

### Process View

#### Level 1

As this level does not add any additional information regarding the user story's execution flow from level 1's diagram,
it's diagram was deemed irrelevant.

#### Level 2

As this level does not add any additional information regarding the user story's execution flow from level 1's diagram, 
it's diagram was deemed irrelevant.

#### Level 3

- _Visualization_<br>
> TBD


#### 4.1.3 Development View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#3-development-view).

#### 4.1.4 Physical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#4-physical-view).


## 5. Implementation

> TBD

## 6. Integration/Demonstration

> TBD

## 7. Observations

* [Raycaster Three.js Documentation](https://threejs.org/docs/#api/en/core/Raycaster)
* [Tween.js Documentation](https://github.com/tweenjs/tween.js)