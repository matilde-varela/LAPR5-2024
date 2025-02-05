# US 6.4.3

<!-- TOC -->
* [US 6.4.3](#us-643)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 6.4.3:** As system administrator, I want the clients listed in the requirement 6.4.2 to be able to be defined by 
simply changing a text file.

**Acceptance Criteria:**

- **6.4.3.1:** Only the clients on the DEI's internal network should have access to the solution.

- **6.4.3.2:** The list of clients should be defined on a text file, using the list obtained on the user story 6.4.2.

**Dependencies/References:**

**6.4.2:** This user story needs to implemented, so the clients can be listed.

**Client Clarifications:**

> **Question:** US 6.4.3 - "As system administrator, I want the clients listed in the requirement 6.3.2 to be able to be defined by simply changing a text file".
> US 6.3.2 - "As an Admin, I want to know till what dimension in terms of number of surgeries is possible to ask for the better solution".
> Dear Client,
> The relation between US 6.4.3 and US 6.3.2 doesn't make sense. Is it possible to provide additional information?
> Thank you in advance.
>
> **Answer:** Regarding US 6.4.3, please consider the 6.4.2, that is, "As system administrator, I want the clients listed in the requirement 6.4.2 to be able to be defined by simply changing a text file"."


> **Question:** Can you clarify what it means in US 6.4.3 to define by a file the users in 6.4.2 (access to the solution)?
>
> **Answer:** I want that the definition of allowed users to access not to be hardcoded but defined in a file.


## 3. Analysis

The main goal of this user story is to allow the manage the list of client by simply editing a text file. To implement
this, it will be needed:

  * text file configuration
  * DEI Network

It's important to be sure that only the system administrator has the permission to edit the file.


## 4. Design

### 4.1. Realization

The first step to implement this user story, will be to create the "clients.txt" file and, within this file, the IP's
for the clients will be defined.

Then will be created a script "update_clients_ips.sh", were the following steps will happen, for each client on the file:
  1) Use the iptables -A, to add a new rule to the iptables, to accept connections with the IP from that line;
  2) Show a message on the command line to inform that the client was allowed. 



Once all entries are processed, the list of authorized clients will be defined.
