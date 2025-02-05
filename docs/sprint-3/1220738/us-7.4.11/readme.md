# US 7.4.11

<!-- TOC -->
- [US 7.4.11](#us-7.4.11)
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
It relates to the System Administrator.

## 2. Requirements

**US 7.4.11:** As a system administrator, I would like to create a public file share, in SMB/CIFS or NFS format, to speed things up between the various teams.

**Acceptance Criteria:**

**US 7.4.11.1:** A designated directory on the VM must serve as the shared file location.

**US 7.4.11.2:** The shared directory must have appropriate ownership and permissions to allow team access.

**US 7.4.11.3:** The file share must be accessible to internal network users without requiring credentials for basic access.

**US 7.4.11.4:** Protocol configurations must follow best practices to prevent unauthorized access or data breaches.

**Dependencies/References:**

**US 7.4.9 and 7.4.10:** They depend on this US as it creates the new type of access to the virtual machine.

**Client Clarifications:**

>**Question:** None yet.
>
>**Answer:** None yet.

## 3. Analysis

This User Story will touch topics like:

1. **SMB/CIFS:** Commonly used for file sharing in Windows environments, but also supported on Linux through Samba.
2. **NFS:** Typically used in Linux and UNIX environments for network file sharing.

As the VM is a Linux system we will use **NFS**.

Although the file share is public, access should be limited to internal networks to prevent unauthorized external access.

## 4. Design

### 4.1. Realization

The design for this user story involves setting up a public file share accessible via SMB/CIFS or NFS protocols to facilitate faster collaboration between teams. The file share will be created on a designated server with sufficient storage capacity and configured to support either protocol based on the teams' needs and compatibility with their operating systems.

For SMB/CIFS, a Samba server will be configured with a public share definition, ensuring appropriate permissions to allow seamless access without the need for individual user credentials. For NFS, an export will be created in the NFS server configuration file, specifying the shared directory and access rules, such as IP-based restrictions.

Permissions on the shared directory will be configured to allow read/write access for all teams, with options for advanced access controls if necessary. Network security will be ensured by restricting access to specific subnets or IP ranges and enabling firewall rules to allow only the relevant protocols. Logs and monitoring will be implemented to track file share usage and identify any unauthorized activities.

#### Views

//TO BE DONE

### 4.2. Domain Model Excerpt

//TO BE DONE

### 4.3. Applied Patterns

//TO BE DONE

## 5. Implementation

//TO BE DONE

## 6. Testing

//TO BE DONE