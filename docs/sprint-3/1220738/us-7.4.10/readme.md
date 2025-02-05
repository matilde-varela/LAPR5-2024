# US 7.4.10

<!-- TOC -->
- [US 7.4.10](#us-7.4.10)
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

**US 7.4.10:** As system administrator, I want the administrator to have SSH access to the virtual machine, by certificate only, without a password.

**Acceptance Criteria:**

**US 7.4.10.1:** SSH access to the virtual machine must require an administrator's private key.

**US 7.4.10.2:** Attempts to log in using a password must be rejected.

**US 7.4.10.3:** Invalid or untrusted keys must result in a denied connection.

**US 7.4.10.4:** A successful login must grant the administrator appropriate permissions to manage the VM.

**US 7.4.10.5:** All login attempts (both successful and failed) must be logged in the system's authentication logs (**/var/log/auth.log** or equivalent).

**Dependencies/References:**

**US 7.4.9 and 7.4.10:** They depend on this US as it creates the new type of access to the virtual machine.

**Client Clarifications:**

>**Question:** None yet.
>
>**Answer:** None yet.

## 3. Analysis

This User Story will use a public/private key pair for authentication. The administrator's public key will be stored in the VM’s **/.ssh/authorized_keys** file, and their private key will remain securely on their local machine.

The SSH server must be configured to:

1. Accept only public key authentication.
2. Deny password-based login attempts.

## 4. Design

### 4.1. Realization

The design for this user story involves configuring the virtual machine to allow SSH access exclusively via certificate-based authentication, disabling password authentication entirely. The administrator will generate a public-private key pair, with the private key securely stored on their local machine and the public key added to the VM's authorized_keys file in the SSH configuration directory.

The SSH server on the VM will be configured to disable password authentication by modifying the SSH daemon configuration file (sshd_config) and setting PasswordAuthentication to no. After making this change, the SSH service will be restarted to apply the configuration.

Security will be ensured by using a strong key pair with a recommended algorithm (such as RSA or ED25519) and enforcing strict permissions on the authorized_keys file. Regular audits will confirm that no unauthorized keys are present, and key rotation policies can be established as needed for additional security.

## 5. Implementation

For this process to work various changes were done or added.

First, a ssh security key pair is made to connect to the MDBackoffice VM and sent to said vm in the ssh/authorized_keys so the VM knows who to authenticate:

´´´´
##Firstly
ssh-keygen -t rsa -b 4096 -C "connectMDBackoffice"
##Secondly
ssh-copy-id -i ~/.ssh/id_ed25519.pub root@vs251.dei.isep.ipp.pt
´´´´

Secondly, the settings for the ssh server are changed to allow public key access, to allow only root login with certificates and to not allow password login:

´´´´
#       $OpenBSD: sshd_config,v 1.103 2018/04/09 20:41:22 tj Exp $

# This is the sshd server system-wide configuration file.  See
# sshd_config(5) for more information.

# This sshd was compiled with PATH=/usr/bin:/bin:/usr/sbin:/sbin

# The strategy used for options in the default sshd_config shipped with
# OpenSSH is to specify options with their default value where
# possible, but leave them commented.  Uncommented options override the
# default value.

Include /etc/ssh/sshd_config.d/*.conf

#Port 22
#AddressFamily any
#ListenAddress 0.0.0.0
#ListenAddress ::

#HostKey /etc/ssh/ssh_host_rsa_key
#HostKey /etc/ssh/ssh_host_ecdsa_key
#HostKey /etc/ssh/ssh_host_ed25519_key

# Ciphers and keying
#RekeyLimit default none

# Logging
#SyslogFacility AUTH
#LogLevel INFO

# Authentication:

#LoginGraceTime 2m
PermitRootLogin prohibit-password
#PermitRootLogin yes
#StrictModes yes
#MaxAuthTries 6
#MaxSessions 10

PubkeyAuthentication yes

# Expect .ssh/authorized_keys2 to be disregarded by default in future.
AuthorizedKeysFile      .ssh/authorized_keys .ssh/authorized_keys2

#AuthorizedPrincipalsFile none

#AuthorizedKeysCommand none
#AuthorizedKeysCommandUser nobody

# For this to work you will also need host keys in /etc/ssh/ssh_known_hosts
#HostbasedAuthentication no
# Change to yes if you don't trust ~/.ssh/known_hosts for
# HostbasedAuthentication
#IgnoreUserKnownHosts no
# Don't read the user's ~/.rhosts and ~/.shosts files
#IgnoreRhosts yes

# To disable tunneled clear text passwords, change to no here!
PasswordAuthentication yes
#PermitEmptyPasswords no

# Change to yes to enable challenge-response passwords (beware issues with
# some PAM modules and threads)
ChallengeResponseAuthentication no

# Kerberos options
#KerberosAuthentication no
#KerberosOrLocalPasswd yes
#KerberosTicketCleanup yes
#KerberosGetAFSToken no

# GSSAPI options
#GSSAPIAuthentication no
#GSSAPICleanupCredentials yes
#GSSAPIStrictAcceptorCheck yes
#GSSAPIKeyExchange no

# Set this to 'yes' to enable PAM authentication, account processing,
# and session processing. If this is enabled, PAM authentication will
# be allowed through the ChallengeResponseAuthentication and
# PasswordAuthentication.  Depending on your PAM configuration,
# PAM authentication via ChallengeResponseAuthentication may bypass
# the setting of "PermitRootLogin without-password".
# If you just want the PAM account and session checks to run without
# PAM authentication, then enable this but set PasswordAuthentication
# and ChallengeResponseAuthentication to 'no'.
UsePAM yes

#AllowAgentForwarding yes
#AllowTcpForwarding yes
#GatewayPorts no
X11Forwarding yes
#X11DisplayOffset 10
#X11UseLocalhost yes
#PermitTTY yes
PrintMotd no
#PrintLastLog yes
#TCPKeepAlive yes
#PermitUserEnvironment no
#Compression delayed
#ClientAliveInterval 0
#ClientAliveCountMax 3
#UseDNS no
#PidFile /var/run/sshd.pid
#MaxStartups 10:30:100
#PermitTunnel no
#ChrootDirectory none
#VersionAddendum none

# no default banner path
#Banner none

# Allow client to pass locale environment variables
AcceptEnv LANG LC_*

# override default of no subsystems
Subsystem       sftp    /usr/lib/openssh/sftp-server

# Example of overriding settings on a per-user basis
#Match User anoncvs
#       X11Forwarding no
#       AllowTcpForwarding no
#       PermitTTY no
#       ForceCommand cvs server
´´´´

## 6. Testing

//TO BE DONE