# US 6.4.7

<!-- TOC -->
* [US 6.4.7](#us-647)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 6.4.7:** As system administrator I want to define a public folder for all users registered on the system, where 
they can read whatever is placed there.

**Acceptance Criteria:**

- **6.4.7.1:** A public folder needs to be assigned to used as a public directory.

- **6.4.7.2:** The public folder needs to have all the users registered on the system, which will needs to give them the
permission to read all the content on that folder.

**Dependencies/References:**

This user story does not have dependencies.

## 3. Analysis

In this user story, a public folder will be created, where all the registered users on the system can read all the
information placed there.

For this, it will be needed to set up a public folder and then manage the access control to this folder.

## 4. Design

### 4.1. Realization

To set up a public folder accessible to all users registered on the system, only with read permissions, the following
steps will be done, while being registered as root on the virtual machine. This configuration will 
allow for future addition of important documents to be seen by all the users:

1) Create a folder, for example "/public_folder" using `mkdir`
2) Assign read-only permissions for all the users registered, using `chmod -R a+r`
3) Create a file inside the folder to be used as test, with `nano`
4) Check the permissions of the test file with the command `ls -l`
5) Verify, with a registered user, if it's possible to read the file 

If the last step was right, try to edit the file with the same user and to create a new file inside the "/public_folder".

