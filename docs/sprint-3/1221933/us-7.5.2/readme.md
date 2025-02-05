# US 7.5.2

<!-- TOC -->
* [US 7.5.2](#us-752)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
<!-- TOC -->


## 1. Context

This is the first time this US is being worked on.

## 2. Requirements

**US 7.5.2:** As a healthcare staff member, whenever I press the “i” key, I want to display/hide an overlay containing 
updated information about the selected room.

**Acceptance Criteria:**

**US 7.5.2.1:** The information should be always updated.

**Dependencies/References:**

**US 6.5.1:** The 3D model should be created.

**Client Clarifications:**

>**Question:** Is US 7.5.2 related to 7.5.1, which refers to selecting a room with picking and then opening an overlay 
> with the "i" key in 7.5.2?
>Or, for example, our "player" enters a room and presses "i," and it shows the information?
>
>**Answer**: The project requirements do not reference any "player," but you imagine it could refer to the robot from 
> the "Thumb Raiser" project or something equivalent. The requirements state that the selection of a surgery room 
> should be made using the picking technique (7.5.1), and when the user presses the "i" key, an overlay with information
> about the selected room should appear (7.5.2).
>
> You should start by implementing what these two requirements establish. Then, considering that you've decided to 
> include a "player" in the scene, you should also implement an alternative room selection method: when the "player" 
> enters a room, it becomes the new selected room, and pressing the "i" key will display information related to it.
 

>**Question:** In the US "As a healthcare staff member, whenever I press the “i” key, I want to display/hide
> an overlay containing updated information about the selected room."
>
>**Answer**: Room identification
>Status
>If occupied, patient and surgery details

## 3. Analysis

The main objective is to enable healthcare staff to quickly view or hide updated information about a selected room using
the "i" key. The overlay must display real-time data relevant to the selected room and remain in sync with the 3D model.

Main points to address are:

* The "i" key should toggle the overlay without conflicting with other shortcuts.
* The overlay must only appear when a room is selected, showing an appropriate message otherwise.
* Data displayed should update dynamically to ensure accuracy.

## 4. Design

### 4.1. Realization

In this functionality, pressing the "i" key toggles the display of an overlay that shows updated information about the 
currently selected room.

If a room is selected, the overlay provides real-time data such as:
* the room number
* status
* assigned staff
* appointments

If no room is selected, the overlay displays a message prompting the user to select a room. The information 
updates dynamically to ensure accuracy, and the overlay integrates seamlessly with the 3D model, reflecting any changes 
in selection.
