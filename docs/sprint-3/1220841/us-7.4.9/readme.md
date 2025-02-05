# US 7.4.9

<!-- TOC -->
- [US 7.4.9](#us-749)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [5. Implementation](#5-implementation)
<!-- TOC -->


## 1. Context

This user story is being implemented for the first time.

## 2. Requirements

**US 7.4.9:** As the organization's administrator, I want a clustering system to be implemented between the systems that implement the SPA in a justified manner.

## 3. Analysis

To implement a clustering system for the SPA, we need to configure load balancing and clustering mechanisms to distribute traffic efficiently among servers hosting the Single Page Application. This ensures high availability, scalability, and fault tolerance.

## 5. Implementation

* Added the following configuration to the file `/etc/haproxy/haproxy.cfg`:

```console
frontend http-in
                bind 10.9.10.38:80
                stats enable
                mode http
                option httpclose 
                option forwardfor
                default_backend servers
backend servers
                balance roundrobin
                server server1 10.9.23.5:80 check
                server server2 10.9.20.251:80 check
```
