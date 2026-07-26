FROM mcr.microsoft.com/powershell:7.4-debian-12

RUN apt-get update \
 && apt-get install -y --no-install-recommends bash openssh-server \
 && rm -rf /var/lib/apt/lists/*

# sshd needs this directory and host keys
RUN mkdir -p /run/sshd && ssh-keygen -A

# allow password login
RUN echo "PasswordAuthentication yes" >> /etc/ssh/sshd_config

# test account: tester / Passw0rd!
RUN useradd -ms /bin/bash tester \
 && echo 'tester:Passw0rd!' | chpasswd

CMD ["/usr/sbin/sshd", "-D", "-e"]