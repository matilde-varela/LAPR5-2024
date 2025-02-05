# US 6.1.1

<!-- TOC -->
* [US 6.1.1](#us-611)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
      * [4.1.1 UI Layout](#411-ui-layout)
      * [4.1.2 Route Handling](#412-route-handling)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 6.1.1:** As user, I want to have an integrated UI for all modules of the system so that I don’t need to switch 
between application urls.

**Acceptance Criteria:**

- **6.1.1.1:** The UI should have every module in a unique page to avoid switching between URLs.

- **6.1.1.2:** The user permissions should control the visibility of each module in the integrated UI.

**Dependencies/References:**

**US 6.1.2 & 6.1.3 & 6.1.4 & 6.1.5:** The UI for all the modules should be created first so can be integrated on one UI.

This user story will depend on the Angular to develop the integrated UI.

## 3. Analysis

To implement this user story, a single-page application (SPA) will be developed using Angular. The integrated UI should
have the follow elements: 

  * module specific areas
  * shared layout

It's important to unsure that the modules access respect the user permissions, so it is only access by users with the
right permissions.

## 4. Design

### 4.1. Realization

To be able to have an integrated UI, as said in the analysis, it's needed to use a single-page application (SPA). Following
this idea, it is necessary to ensure that all the components and modules are presented on a single page.

#### 4.1.1 UI Layout

The UI layout will consist on:

  * **Header:** This section will have the logo, the application name and two buttons for login and logout.
  * **Main Content Area:** This area will be dynamically updated to show the content of the selected component, without 
using other URLs
  * **Sidebar:** The sidebar will contain the functionalities for each user type and will only appear, after the login was
made

#### 4.1.2 Route Handling

The Angular routing mechanism will be used to dynamic loading the components. A single route will be employed to load 
the relevant information, ensuring seamless navigation without changing URLs.
