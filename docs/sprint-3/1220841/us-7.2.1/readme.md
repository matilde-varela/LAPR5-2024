# US 7.2.1

<!-- TOC -->
- [US 7.2.1](#us-721)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
      - [Logical View](#logical-view)
      - [Development View](#development-view)
      - [Physical View](#physical-view)
<!-- TOC -->

## 1. Context

This is the first time this user story is being developed.

## 2. Requirements

**US 7.2.1:** As Software Architect, I want to adopt a decentralized architecture of the backoffice module, so that it is composed of the existing SPA frontend and the .Net backend, and a new module responsible for managing, namely, Patient’s Medical Records, Allergies and Medical Conditions. This module must be implemented in Node.js, Express and MongoDB SGBD.

**Acceptance Criteria:**

- 7.2.1.1: The module must use Node.Js, Express and MongoDG SGBD.
- 7.2.1.2: This module must only manage Patient’s Medical Records, Allergies and Medical Conditions.

**Dependencies/References:**

None.

**Client Clarifications:**

> **Question:** We would like to know if the new module that we are asked for will be just a database or will it be a new UI that would then connect to this new module that uses a new database in MongoDB.
>
> **Answer:** _To be answered_

## 3. Analysis

For this user story a new module must be setup so that it will manage the Patient’s Medical Records, Allergies and Medical Conditions.

With this intent we will use the following technologies: Node.Js, Express and MongoDB. As a base project we will use the [bulletproof-nodejs-ddd](https://bitbucket.org/nunopsilva/bulletproof-nodejs-ddd/src/master/) project.


## 4. Design

### 4.1. Realization

The logical, physical, development and scenario views diagrams are generic for all the use cases.

#### Logical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#1-logical-view).

#### Development View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#3-development-view).

#### Physical View

The diagrams can be found in the [team decision views folder](../../team-decisions/views/general-views.md#4-physical-view).
