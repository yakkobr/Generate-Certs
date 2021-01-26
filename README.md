


# Generate Self-Signed SSL/TLS Certificates

### What is this?
This is a Windows/Linux/Mac app that generates self-signed SSL/TLS certificates, including the Root CA cert.

### Files Created

**Option #1 - 1 certificate:**

![Generate-Certs Result - 1 cert](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/4.png)

**Option #2 - 2 certificates:**

![Generate-Certs Result - 2 certs](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/5.png)

### Why Use Generate-Certs?
1 minute to use this program compared to 30 minutes entering in and adjusting all the commands and files

### Notes

- [ECSDA certificates](https://blog.cloudflare.com/ecdsa-the-digital-signature-algorithm-of-a-better-internet/) by design (more secure than RSA) using the **secp256p1** curve
- **If you want PFX and PEM files created** (for all files), you MUST set a Root CA Cert password.
- The **SUBJECT** name is usually the main hostname that you're connecting to.
- The **SUBJECT ALTERNATIVE NAME** is usually an alternate name, or an IP address.
	- **Generate-Certs** supports the detection of IP addresses in the SAN field.
- Certificates expire **3650 days** after day of certificate generation.
- Files are created in the **SSL_Certs_Out** subdirectory from whatever directory you're in when running **Generate-Certs**.  The full path will be displayed upon starting the program.

## Install

### Windows

Not sure which architecture? Try the x86 version as it should work with most Windows machines.

**win-x64:**

[Download and install .NET 5 for Windows x64](https://dotnet.microsoft.com/download/dotnet/current/runtime)

[Download Generate-Certs_win-x64.exe](<https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_win-x64.exe>), then run `Generate-Certs_win-x64.exe`

**win-x86:**

[Download and install .NET 5 for Windows x86](https://dotnet.microsoft.com/download/dotnet/current/runtime)

[Download Generate-Certs_win-x86.exe](<https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_win-x86.exe>), then run `Generate-Certs_win-x86.exe`

**win-arm64:**

[Download and install .NET 5 for Windows arm64](https://dotnet.microsoft.com/download/dotnet/5.0) (Runtime column, under ".NET Runtime 5.x.x", arm64  Installer)

[Download Generate-Certs_win-arm64.exe](<https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_win-arm64.exe>), then run `Generate-Certs_win-arm64.exe`

### Linux

**PREREQUISITE - Install .NET 5 for Linux if it's not already installed:**
```
wget dot.net/v1/dotnet-install.sh -O dotnet.sh
bash dotnet.sh -c 5.0
```

**linux-64:**
```
wget https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_linux-x64
chmod +x Generate-Certs_linux-x64
./Generate-Certs_linux-x64
```

**linux-arm:**
```
wget https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_linux-arm
chmod +x Generate-Certs_linux-arm
./Generate-Certs_linux-arm
```

**linux-arm64:**
```
wget https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_linux-arm64
chmod +x Generate-Certs_linux-arm64
./Generate-Certs_linux-arm64
```

### Mac

**PREREQUISITE** - [Download and install .NET 5 for Mac x64](https://dotnet.microsoft.com/download/dotnet/5.0) (Runtime column, under ".NET Runtime 5.x.x", Mac x64 Installer)

**osx-x64:**
```
wget https://github.com/asheroto/Generate-Certs/releases/latest/download/Generate-Certs_osx-x64
chmod +x Generate-Certs_osx-x64
./Generate-Certs_osx-x64
```


# Screenshots

## Info/Warning
![Generate-Certs Initial Screen](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/1.png)

## Configuration
![Generate-Certs Configuration](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/2.png)

## Final Result
![Final Result](https://github.com/asheroto/Generate-Certs/blob/master/screenshots/3.png)