# US 6.4.2

<!-- TOC -->
* [US 6.4.2](#us-642)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
  * [5. Implementation](#5-implementation)
<!-- TOC -->


## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 6.4.2:** As a system administrator, I want only customers from DEI's internal network (wired or via VPN) to access the solution.

**Acceptance Criteria:**

- 6.4.2.1: Only DEI internal network addresses are allowed.

**Dependencies/References:**

This user story has no dependencies.

**Client Clarifications:**

> **Question:** Regarding the access of the users to the solution through DEI's network, do you have any specifications 
regarding said access? Like allowing ssh, http, https connections or anything like X amount of accesses in an hour?
>
> **Answer:** The permitted accesses must be those necessary to access the solution, as stated in the US.

> **Question:** Can you tell us what network addresses ranges do we have to consider for DEI network and VPN accesses? They're submasks too, please.
> 
> **Answer:** That is not important, as network addresses can vary. As long as you can change your configuration to allow the IP address obtained at that time, the proof of concept is evident.

> **Question:** Can you clarify what it means in US 6.4.3 to define by a file the users in 6.4.2 (access to the solution)?
>
> **Answer:** I want that the definition of allowed users to access not to be hardcoded but defined in a file.

> **Question:** Are we supposed to set up a Windows or Linux machine? When it says "access the solution," is it referring to the VM that will run the module for US 6.4.1? Should access be configured to restrict by IPs or by domain users?
>
> **Answer:** Again, I’m not specifying the OS to use; it depends on the solution you’ve designed. When it says “access the solution,” it means exactly that: accessing and using the solution you designed to respond to the RFP. The prompt states, “only want clients on DEI's internal network (wired or via VPN) to be able to access the solution,” and it doesn’t specify domain users.


## 3. Analysis

To allow only the customers from DEI's internal network to access the solution provided some limitations must be implemented
regarding the network packet traffic - a firewall. Taking into account the information retrieved, this filtering mechanism
must impose the following restrictions:

- Allow packets from DEI's internal network IP addresses range
- Allow packets from VPN IP addresses range
- Allow Localhost (Loopback) access
- Drop all other incoming traffic

By applying these limitations we can guarantee that only the chosen users can access the solution.

## 4. Design

### 4.1. Realization

For the definition of the firewall policies and chains, the command `iptables` will be used. The filtering of the access to
the solution is made on the table `filter`.

Taking into consideration the information processed in the [analysis](#3-analysis), we can establish correspondence criteria
for the accesses we allow.

Most of them will refer to the built-in chain `INPUT`, which applies ACCEPT/DROP rules on packets incoming to the node.

* `INPUT -j ACCEPT` on `tcp` connections with DEI's internal network IP range
* `INPUT -j ACCEPT` on `tcp` connections with VPN IP range
* `INPUT -j ACCEPT` on `loopback` connection
* `INPUT -j DROP` on any other incoming traffic to the node

The `-j` parameter indicates the existence of a target for said chain.

* `-j ACCEPT`: allows the packet to pass through, and stops further processing of the chain for that packet.
* `-j DROP`: blocks the packet, and stops further processing of the chain for that packet.

The criteria `-p` refers to the protocol designation defined within the packet. It can be `tcp`, `udp`, `icmp` ou `icmpv6`.

The criteria `--dport` refers to the packet's destination port.

## 5. Implementation

The solution implemented is the following one:

```
#!/bin/bash

# Flush existing rules (and built-in ones too)
iptables -F
iptables -X

# Allow loopback interface
iptables -A INPUT -i lo -j ACCEPT

# Allow established and related connections
iptables -A INPUT -m state --state ESTABLISHED, RELATED -j ACCEPT

# Allow SSH access from DEI VPN
iptables -A INPUT -s 10.8.0.0/16 -p tcp --dport 22 -j ACCEPT

# Block everything else
iptables -A INPUT -j DROP
```
